using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace KourageousTourists
{

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class KourageousTouristsAddOn : MonoBehaviour
	{
		
		public const String cfgRoot = "KOURAGECONFIG";
		public const String cfgLevel = "LEVEL";
		public const String debugLog = "debuglog";

		private const String audioPath = "KourageousTourists/Sounds/shutter";

		private TouristFactory factory = null;
		// We keep every kerbal in scene in here just to make every one of them smile
		// on photo; some, however, clearly are not tourists
		public Dictionary<String, Tourist> tourists = null;

		public DateTime selfieTime;

		public Vector3 savedCameraPosition;
		public Quaternion savedCameraRotation;
		public Transform savedCameraTarget;

		bool smile = false;
		bool taken = false;
		private FXGroup fx = null;

		public double RCSamount;
		public double RCSMax;
		internal static bool debug = true;

		bool highGee = false;

		public static EventVoid selfieListeners = new EventVoid("Selfie");

		public KourageousTouristsAddOn ()
		{
		}

		public void Awake()
		{
			ConfigNode config = GameDatabase.Instance.GetConfigNodes(
				KourageousTouristsAddOn.cfgRoot).FirstOrDefault();
			String debugState = config.GetValue ("debug");
			debug = debugState != null && 
				(debugState.ToLower ().Equals ("true") || debugState.Equals ("1"));
		

			Log.dbg("entered KourageousTourists Awake scene:{0}", HighLogic.LoadedScene);

			GameEvents.OnVesselRecoveryRequested.Add (OnVesselRecoveryRequested);

			if (!HighLogic.LoadedSceneIsFlight)
				return;

			if (factory == null)
				factory = new TouristFactory ();
			if (tourists == null)
				tourists = new Dictionary<String, Tourist> ();

			selfieTime = DateTime.Now;

			Log.dbg("Setting handlers");

			GameEvents.onVesselGoOffRails.Add (OnVesselGoOffRails);
			GameEvents.onFlightReady.Add (OnFlightReady);
			GameEvents.onAttemptEva.Add (OnAttemptEVA);

			GameEvents.onNewVesselCreated.Add (OnNewVesselCreated);
			GameEvents.onVesselCreate.Add (OnVesselCreate);
			GameEvents.onVesselChange.Add (OnVesselChange);
			GameEvents.onVesselWillDestroy.Add (OnVesselWillDestroy);
			GameEvents.onCrewBoardVessel.Add (OnCrewBoardVessel);
			GameEvents.onCrewOnEva.Add (OnEvaStart);
			GameEvents.onKerbalLevelUp.Add (OnKerbalLevelUp);
			GameEvents.onVesselRecovered.Add (OnVesselRecoveredOffGame);

			//reinitCrew (FlightGlobals.ActiveVessel);
		}

		public void OnDestroy() {

			// Switch tourists back
			Log.dbg("entered OnDestroy");
			try {
				if (FlightGlobals.VesselsLoaded == null)
					return;
				Log.dbg("VesselsLoaded: {0}", FlightGlobals.VesselsLoaded);
				foreach (Vessel v in FlightGlobals.VesselsLoaded) {
					Log.dbg("restoring vessel {0}", v.name);
					List<ProtoCrewMember> crewList = v.GetVesselCrew ();
					foreach (ProtoCrewMember crew in crewList) {
						Log.dbg("restoring crew={0}", crew.name);
						if (Tourist.isTourist(crew))
							crew.type = ProtoCrewMember.KerbalType.Tourist;
					}
				}
			}
			catch(Exception e) {
				Log.err(e, "Got Exception while attempting to access loaded vessels");
			}

			GameEvents.onVesselGoOffRails.Remove (OnVesselGoOffRails);
			GameEvents.onFlightReady.Remove (OnFlightReady);
			GameEvents.onAttemptEva.Remove (OnAttemptEVA);
			GameEvents.onVesselChange.Remove (OnVesselChange);
			GameEvents.onVesselWillDestroy.Remove (OnVesselWillDestroy);

			tourists = null;
			factory = null; // do we really need this?
			smile = false;
			taken = false;
			fx = null;
		}

		private void OnEvaStart(GameEvents.FromToAction<Part, Part> evaData) {
			
			Log.dbg("entered; KourageousTourists OnEvaStart Parts: {0}; {1}; active vessel: {2}", evaData.from, evaData.to, FlightGlobals.ActiveVessel);
			Vessel v = evaData.to.vessel;
			if (!v || !v.evaController)
				return;
			Log.dbg("vessel: {0}; evaCtl: {1}", v, v.evaController);
			ProtoCrewMember crew = v.GetVesselCrew () [0];
			Log.dbg("crew: {0}", crew);
			if (this.tourists == null) {
				// Why we get here twice with the same data?
				Log.dbg("for some reasons tourists is null");
				return;
			}
#if DEBUG
			foreach(KeyValuePair<String, Tourist> pair in this.tourists)
				Log.dbg("{0}={1}", pair.Key, pair.Value);
#endif
			Log.dbg("roster: {0}", this.tourists);
			Tourist t;
			if (!tourists.TryGetValue(crew.name, out t))
				return;
			Log.dbg("tourist: {0}", t);
			if (!Tourist.isTourist (crew) || t.hasAbility ("Jetpack"))
				return;

			evaData.to.RequestResource (v.evaController.propellantResourceName, 
				v.evaController.propellantResourceDefaultAmount);
			// Set propellantResourceDefaultAmount to 0 for EVAFuel to recognize it.
			v.evaController.propellantResourceDefaultAmount = 0.0;
			
			ScreenMessages.PostScreenMessage (String.Format(
				"<color=orange>Jetpack propellant drained as tourists of level {0} are not allowed to use it</color>", 
				t.level));
		}

		private void OnAttemptEVA(ProtoCrewMember crewMemeber, Part part, Transform transform) {

			// Can we be sure that all in-scene kerbal tourists were configured?

			Log.dbg("entered KourageousTourists OnAttemptEVA");

			Tourist t;
			if (!tourists.TryGetValue (crewMemeber.name, out t))
				return;

			if (!Tourist.isTourist (crewMemeber)) // crew always can EVA
				return;

			Vessel v = FlightGlobals.ActiveVessel;
			Log.dbg("Body: {0}; situation: {1}", v.mainBody.GetName(), v.situation);
			EVAAttempt attempt = t.canEVA(v);
			if (!attempt.status) {
				ScreenMessages.PostScreenMessage ("<color=orange>" + attempt.message + "</color>");
				FlightEVA.fetch.overrideEVA = true;
			}
		}

		private void OnNewVesselCreated(Vessel vessel)
		{
			Log.dbg("OnNewVesselCreated name=" + vessel.GetName ());
		}

		private void OnVesselCreate(Vessel vessel)
		{
			if (vessel == null)
				return;
			
			Log.dbg("OnVesselCreate name=" + vessel.GetName ());

			reinitVessel (vessel);
			reinitEvents (vessel);

			if (vessel.evaController == null)
				return;
			if (!Tourist.isTourist (vessel.GetVesselCrew () [0]))
				return;
		}

		private void OnVesselWillDestroy(Vessel vessel) {
			Log.dbg("OnVesselWillDestroy name=" + vessel.GetName());

			if (vessel == null || vessel.evaController == null)
				return;

			Log.dbg("eva name = " + vessel.evaController.name);
			Tourist t;
			if (!tourists.TryGetValue(vessel.evaController.name, out t))
				return;

			t.smile = false;
			t.taken = false;
		}

		private void OnCrewBoardVessel(GameEvents.FromToAction<Part, Part> fromto) {
			Log.dbg("from = {0}; to = {1}; active vessel: {2}", fromto.from.name,fromto.to.name, FlightGlobals.ActiveVessel.name);
			reinitVessel (fromto.to.vessel);
		}

		private void OnKerbalLevelUp(ProtoCrewMember kerbal) {
		
			if (tourists == null || !tourists.ContainsKey (kerbal.name))
				return;
			Log.dbg("Leveling up {0}", kerbal.name); 
			// Re-create tourist
			tourists[kerbal.name] = factory.createForLevel (kerbal.experienceLevel, kerbal);
		}

		private void checkApproachingGeeLimit() {
			
			if (FlightGlobals.ActiveVessel != null && 
					FlightGlobals.ActiveVessel.geeForce < 4.0) {// Can there be any tourist with Gee force tolerance below that?
			
				if (highGee) {
					reinitVessel (FlightGlobals.ActiveVessel);
					highGee = false;
					ScreenMessages.PostScreenMessage ("EVA prohibition cleared");
				}
				return;
			}
			if (tourists == null)
				return;
			foreach (ProtoCrewMember crew in FlightGlobals.ActiveVessel.GetVesselCrew()) {
				if (!tourists.ContainsKey (crew.name) || 			// not among tourists
					!Tourist.isTourist(crew) || 					// not really a tourist
					crew.type != ProtoCrewMember.KerbalType.Crew)   // was probably unpromoted
						continue;

				if (crew.gExperienced / ProtoCrewMember.GToleranceMult(crew) > 50000) { // Magic number. At 60000 kerbal passes out
					Log.warn("Unpromoting {0} due to high gee", crew.name);
					crew.type = ProtoCrewMember.KerbalType.Tourist;
					ScreenMessages.PostScreenMessage (String.Format (
						"{0} temporary prohibited from EVA due to experienced high Gee forces", crew.name));
					highGee = true;
				}
			}
		}

		private void reinitVessel(Vessel vessel) {
			Log.dbg("entered reinitVessel for {0}", vessel.name);
			foreach (ProtoCrewMember crew in vessel.GetVesselCrew()) {
				Log.dbg("crew = {0}", crew.name);
				if (Tourist.isTourist (crew)) {
					crew.type = ProtoCrewMember.KerbalType.Crew;
					Log.dbg("Tourist promotion: {0}", crew.name);
				}

				if (tourists == null) {
					// TODO: Find out while half of the time we are getting this message
					Log.dbg("for some reason tourists are null");
					continue;
				}
				if (tourists.ContainsKey (crew.name))
					continue;

				Log.dbg("Creating tourist from cfg; lvl: {0}, crew: {1}", crew.experienceLevel, crew);
				Tourist t = factory.createForLevel (crew.experienceLevel, crew);
				this.tourists.Add (crew.name, t);
				Log.dbg("Added: {0} ({1})", crew.name, this.tourists);
			}
			Log.dbg("crew count: {0}", vessel.GetVesselCrew().Count);
			if (vessel.isEVA) {
				// ???
			}
		}

		private void reinitEvents(Vessel v) {
			Log.dbg("entered reinitEvents");
			if (v.evaController == null)
				return;
			KerbalEVA evaCtl = v.evaController;

			ProtoCrewMember crew = v.GetVesselCrew () [0];
			String kerbalName = crew.name;
			Log.dbg("evCtl found; checking name: {0}", kerbalName);
			Tourist t;
			if (!tourists.TryGetValue(kerbalName, out t))
				return;

			Log.dbg("among tourists: {0}", kerbalName);
			t.smile = false;
			t.taken = false;

			if (!Tourist.isTourist(v.GetVesselCrew()[0])) {
				Log.dbg("...but is a crew, not a tourist!");
				return; // not a real tourist
			}

			// Change crew type right away to avoid them being crew after recovery
			crew.type = ProtoCrewMember.KerbalType.Tourist;

			BaseEventList pEvents = evaCtl.Events;
			foreach (BaseEvent e in pEvents) {
				Log.dbg("disabling event {0}", e.guiName);
				e.guiActive = false;
				e.guiActiveUnfocused = false;
				e.guiActiveUncommand = false;
			}
			// Adding Selfie button
			BaseEventDelegate slf = new BaseEventDelegate(TakeSelfie);
			KSPEvent evt = new KSPEvent ();
			evt.active = true;
			evt.externalToEVAOnly = true;
			evt.guiActive = true;
			evt.guiActiveEditor = false;
			evt.guiActiveUnfocused = false;
			evt.guiActiveUncommand = false;
			evt.guiName = "Take Selfie";
			evt.name = "TakeSelfie";
			BaseEvent selfie = new BaseEvent(pEvents, "Take Selfie", slf, evt);
			pEvents.Add (selfie);
			selfie.guiActive = true;
			selfie.active = true;

			foreach (PartModule m in evaCtl.part.Modules) {

				if (!m.ClassName.Equals ("ModuleScienceExperiment"))
					continue;
				Log.dbg("science module id: {0}", ((ModuleScienceExperiment)m).experimentID);
				// Disable all science
				foreach (BaseEvent e in m.Events) {
					e.guiActive = false;
					e.guiActiveUnfocused = false;
					e.guiActiveUncommand = false;
				}

				foreach (BaseAction a in m.Actions)
					a.active = false;
			}

			Log.dbg("Initializing sound");
			// Should we always invalidate cache???
			fx = null;
			getOrCreateAudio (evaCtl.part.gameObject);
		}

		private void OnVesselGoOffRails(Vessel vessel)
		{
			Log.dbg("entered OnVesselGoOffRails");

			reinitVessel (vessel);
			reinitEvents (vessel);
		}

		private void OnVesselChange(Vessel vessel)
		{
			Log.dbg("entered OnVesselChange");
			if (vessel.evaController == null)
				return;
			// OnVesselChange called after OnVesselCreate, but with more things initialized
			OnVesselCreate(vessel);
		}

		private void OnFlightReady() 
		{
			Log.dbg("entered OnFlightReady");
			foreach (Vessel v in FlightGlobals.VesselsLoaded)
				reinitVessel (v);
		}

		private void OnVesselRecoveryRequested(Vessel vessel) 
		{
			Log.dbg("entered; vessel: {0}", vessel.name );
			// Switch tourists back to tourists
			List<ProtoCrewMember> crewList = vessel.GetVesselCrew ();
			foreach (ProtoCrewMember crew in crewList) {
				Log.dbg("crew={0}", crew.name);
				if (Tourist.isTourist(crew))
					crew.type = ProtoCrewMember.KerbalType.Tourist;
			}
		}

		private void OnVesselRecoveredOffGame(ProtoVessel vessel, bool wtf)
		{
			Log.dbg("entered; vessel: {0}; wtf: {1}", vessel.vesselName, wtf);
			// Switch tourists back to tourists
			List<ProtoCrewMember> crewList = vessel.GetVesselCrew ();
			foreach (ProtoCrewMember crew in crewList) {
				Log.dbg("crew={0}", crew.name);
				if (Tourist.isTourist(crew))
					crew.type = ProtoCrewMember.KerbalType.Tourist;
			}
		}

		public void FixedUpdate() {

			if (!HighLogic.LoadedSceneIsFlight)
				return;

			checkApproachingGeeLimit ();

			if (!smile)
				return;
			
			int sec = (DateTime.Now - selfieTime).Seconds;
			if (!taken && sec > 1) {

				Log.dbg("Getting snd");
				FXGroup snd = getOrCreateAudio(FlightGlobals.ActiveVessel.evaController.gameObject);
				if (snd != null) {
					snd.audio.Play ();
				}
				else Log.warn("snd is null");

				String fname = "../Screenshots/" + generateSelfieFileName ();
				Log.detail("writing file " + fname);
				ScreenCapture.CaptureScreenshot (fname);
				taken = true;
			}

			if (sec > 5) {
				smile = false;
				taken = false;

				/*FlightCamera camera = FlightCamera.fetch;
				camera.transform.position = savedCameraPosition;
				camera.transform.rotation = savedCameraRotation;
				camera.SetTarget (savedCameraTarget, FlightCamera.TargetMode.Transform);*/

				//FlightGlobals.ActiveVessel.evaController.part.Events ["TakeSelfie"].active = true;
				GameEvents.onShowUI.Fire ();
				ScreenMessages.PostScreenMessage ("Selfie taken!");
			}
			else
				Smile ();

		}

		public void TakeSelfie() {

			ScreenMessages.PostScreenMessage ("Selfie...!");
			smile = true;
			selfieTime = DateTime.Now;
			foreach (Tourist t in tourists.Values)
				t.generateEmotion ();

			//FlightGlobals.ActiveVessel.evaController.part.Events ["TakeSelfie"].active = false;
			GameEvents.onHideUI.Fire();
			Log.detail("Selfie ");

			/*FlightCamera camera = FlightCamera.fetch;
			savedCameraPosition = camera.transform.position;
			savedCameraRotation = camera.transform.rotation;
			savedCameraTarget = camera.Target;
			camera.SetTargetNone ();*/

			selfieListeners.Fire ();
		}

		private void Smile() {

			foreach (Vessel v in FlightGlobals.Vessels) {

				if (v.evaController == null)
					continue;
				
				KerbalEVA eva = v.evaController;
				kerbalExpressionSystem expression = getOrCreateExpressionSystem (eva);

				if (expression != null) {

					Tourist t;
					if (!tourists.TryGetValue (v.GetVesselCrew()[0].name, out t))
						continue;
					
					expression.wheeLevel = t.whee;
					expression.fearFactor = t.fear;

					/*FlightCamera camera = FlightCamera.fetch;
					camera.transform.position = eva.transform.position + Vector3.forward * 2;
					camera.transform.rotation = eva.transform.rotation;*/

				} else {
					Log.warn("Slf: No expression system");
				}
			}
		}


		// TODO: Refactor this - now we create audio every time active kerbal is changed
		private FXGroup getOrCreateAudio(GameObject obj) {

			if (obj == null) {
				Log.warn("GameObject is null");
				return null;
			}

			if (fx != null) {
				Log.warn("returning audio from cache");
				return fx;
			}

			fx = new FXGroup ("SelfieShutter");

			fx.audio = obj.AddComponent<AudioSource> ();
			Log.detail("created audio source: {0}", fx.audio);
			fx.audio.volume = GameSettings.SHIP_VOLUME;
			fx.audio.rolloffMode = AudioRolloffMode.Logarithmic;
			fx.audio.dopplerLevel = 0.0f;
			fx.audio.maxDistance = 30;
			fx.audio.loop = false;
			fx.audio.playOnAwake = false;
			if (GameDatabase.Instance.ExistsAudioClip (audioPath)) {
				fx.audio.clip = GameDatabase.Instance.GetAudioClip(audioPath);
				Log.detail("Attached clip: {0}", fx.audio.clip);
			} else
				Log.detail("No clip found with path {0}", audioPath);

			return fx;
		}

		private String generateSelfieFileName() {

			// KerbalName-CelestialBody-Time
			Vessel v = FlightGlobals.ActiveVessel;
			ProtoCrewMember crew = v.GetVesselCrew () [0];
			return crew.name + "-" + v.mainBody.name + "-" + DateTime.Now.ToString("yy-MM-dd-HH:mm:ss") + ".png";
		}

		private String dumper<T>(T obj) {
			if (obj == null)
				return "null";
			StringBuilder sb = new StringBuilder();
			try {
				var t = typeof(T);
				var props = t.GetProperties();
				if (props == null)
					return "type: " + t.ToString () + "; props=null";
				
				foreach (var item in props)
				{
					sb.Append($"{item.Name}:{item.GetValue(obj,null)}; ");
				}
				sb.AppendLine();
			}
			catch (Exception e) {
				sb.Append ("Exception while trying to dump object: " + e.ToString ());
			}
			return sb.ToString ();
		}

		private kerbalExpressionSystem getOrCreateExpressionSystem(KerbalEVA p) {

			kerbalExpressionSystem e = p.part.GetComponent<kerbalExpressionSystem>();
			/*printDebug ("expr. system: " + dumper(e));
			printDebug ("kerbalEVA: " + dumper(p));
			printDebug ("part: " + dumper(p.part));*/

			if (e == null) {

				AvailablePart evaPrefab = PartLoader.getPartInfoByName ("kerbalEVA");
				//printDebug ("eva prefab: " + dumper (evaPrefab));
				Part prefabEvaPart = evaPrefab.partPrefab;
				//printDebug ("eva prefab part: " + prefabEvaPart);

				ProtoCrewMember protoCrew = FlightGlobals.ActiveVessel.GetVesselCrew () [0];
				//printDebug ("proto crew: " + protoCrew);

				//var prefabExpr = prefabEva.GetComponent<kerbalExpressionSystem> ();

				Animator a = p.part.GetComponent<Animator> ();
				if (a == null) {
					Log.dbg("Creating Animator...");
					var prefabAnim = prefabEvaPart.GetComponent<Animator> ();
					//printDebug ("animator prefab: " + dumper(prefabAnim));
					a = p.part.gameObject.AddComponent<Animator> ();
					//printDebug ("animator component: " + dumper(a));

					a.avatar = prefabAnim.avatar;
					a.runtimeAnimatorController = prefabAnim.runtimeAnimatorController;

					a.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
					a.rootRotation = Quaternion.identity;
					a.applyRootMotion = false;

					//Animator.rootPosition = new Vector3(0.4f, 1.5f, 0.4f);
					//Animator.rootRotation = new Quaternion(-0.7f, 0.5f, -0.1f, -0.5f);
				}

				Log.dbg("Creating kerbalExpressionSystem...");
				e = p.part.gameObject.AddComponent<kerbalExpressionSystem> ();
				e.evaPart = p.part;
				e.animator = a;
				e.protoCrewMember = protoCrew;
				//printDebug ("expression component: " + dumper (e));
			}
			return e;
		}

	}
}
