using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using UnityEngine;


namespace KourageousTourists
{

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class KourageousTouristsAddOn : MonoBehaviour
	{

		public const String cfgRoot = "KOURAGECONFIG";
		public const String cfgLevel = "LEVEL";
		public const String debugLog = "debuglog";

		private readonly String audioPath = KSPe.GameDB.Asset<KourageousTouristsAddOn>.Solve("Sounds", "shutter");

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
		public static bool noSkyDiving = false;
		internal static float paraglidingChutePitch = 1.1f;
		internal static float paraglidingDeployDelay = 5f;
		public static float paraglidingMaxAirspeed = 100f;
		public static float paraglidingMinAltAGL = 1500f;

		bool highGee = false;

		public static EventVoid selfieListeners = new EventVoid("Selfie");

		public KourageousTouristsAddOn ()
		{
		}

		public void Awake()
		{
			Log.dbg("entered KourageousTourists Awake scene:{0}", HighLogic.LoadedScene);

			bool forceTouristsInSandbox = false;

			ConfigNode config = Settings.Read();

			if (config == null)
			{
				Log.warn("No config nodes!");
				return;
			}
			String debugState = config.GetValue("debug");
			String noDiving = config.GetValue("noSkyDiving");
			String forceInSandbox = config.GetValue("forceTouristsInSandbox");

			try
			{
				paraglidingChutePitch = float.Parse(config.GetValue("paraglidingChutePitch"));
				paraglidingDeployDelay = float.Parse(config.GetValue("paraglidingDeployDelay"));
				paraglidingMaxAirspeed = float.Parse(config.GetValue("paraglidingMaxAirpseed"));
				paraglidingMinAltAGL = float.Parse(config.GetValue("paraglidingMinAltAGL"));
				Log.detail("paragliding params: pitch: {0}, delay: {1}, speed: {2}, alt: {3}", paraglidingChutePitch, paraglidingDeployDelay, paraglidingMaxAirspeed, paraglidingMinAltAGL);
			}
			catch (Exception) {
				Log.detail("Failed parsing paragliding tweaks!");
			}

			Log.detail("debug: {0}; nodiving: {1}; forceInSB: {2}", debugState, noDiving, forceInSandbox);

			debug = debugState != null &&
			        (debugState.ToLower().Equals ("true") || debugState.Equals ("1"));
			noSkyDiving = noDiving != null &&
			        (noDiving.ToLower().Equals ("true") || noDiving.Equals ("1"));
			forceTouristsInSandbox = forceInSandbox != null &&
			              (forceInSandbox.ToLower().Equals ("true") || forceInSandbox.Equals ("1"));

			Log.detail("debug: {0}; nodiving: {1}; forceInSB: {2}", debug, noSkyDiving, forceTouristsInSandbox);
			Log.detail("highlogic: {0}", HighLogic.fetch);
			Log.detail("game: {0}", HighLogic.CurrentGame);

			// Ignore non-career game mode
			if (HighLogic.CurrentGame == null ||
			    (!forceTouristsInSandbox && HighLogic.CurrentGame.Mode != Game.Modes.CAREER))
			{
				return;
			}
			Log.detail("scene: {0}", HighLogic.LoadedScene);

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
			GameEvents.onVesselLoaded.Add (OnVesselLoad);
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
				if (null == FlightGlobals.VesselsLoaded) return;
				Log.dbg("VesselsLoaded: {0}", FlightGlobals.VesselsLoaded);
				foreach (Vessel v in FlightGlobals.VesselsLoaded) {
					if (null == v) continue; // Weird behaviour on KSP 1.10?
					Log.dbg("restoring vessel {0}", v.name);
					List<ProtoCrewMember> crewList = v.GetVesselCrew();
					if (null == v.GetVesselCrew()) continue; // Weird behaviour on KSP 1.10?
					foreach (ProtoCrewMember crew in crewList) {
						if (null == crew) continue; // Weird behaviour on KSP 1.10?
						Log.dbg("restoring crew={0}", crew.name);
						if (Tourist.isTourist(crew))
							crew.type = ProtoCrewMember.KerbalType.Tourist;
					}
				}
			}
			catch(Exception e) {
				Log.error(e, "Got Exception while attempting to access loaded vessels");
			}

			GameEvents.onVesselRecovered.Remove(OnVesselRecoveredOffGame);
			GameEvents.onKerbalLevelUp.Remove(OnKerbalLevelUp);
			GameEvents.onCrewOnEva.Remove(OnEvaStart);
			GameEvents.onCrewBoardVessel.Remove(OnCrewBoardVessel);
			GameEvents.onVesselWillDestroy.Remove(OnVesselWillDestroy);
			GameEvents.onVesselLoaded.Add(OnVesselLoad);
			GameEvents.onVesselChange.Remove(OnVesselChange);
			GameEvents.onVesselCreate.Remove(OnVesselCreate);
			GameEvents.onNewVesselCreated.Remove(OnNewVesselCreated);

			GameEvents.onAttemptEva.Remove(OnAttemptEVA);
			GameEvents.onFlightReady.Remove(OnFlightReady);
			GameEvents.onVesselGoOffRails.Remove (OnVesselGoOffRails);

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
			Log.dbg("roster: {0}", this.tourists.Keys);
			Tourist t;
			if (!tourists.TryGetValue(crew.name, out t))
				return;

			if (Tourist.isTourist(crew))
				Log.dbg("tourist: {0}", t);
			else
			{
				Log.error("{0} is not a Tourist. Aborting!", crew);
				return;
			}

			if (!t.hasAbility ("Jetpack"))
			{
				evaData.to.RequestResource (v.evaController.propellantResourceName, v.evaController.propellantResourceDefaultAmount);
				// Set propellantResourceDefaultAmount to 0 for EVAFuel to recognize it.
				v.evaController.propellantResourceDefaultAmount = 0.0;
			}

			// SkyDiving...
			if (t.looksLikeSkydiving(v)) {
				Log.info("skydiving: {0}, situation: {1}", t.looksLikeSkydiving(v), v.situation);
				v.evaController.ladderPushoffForce = 50;
				v.evaController.autoGrabLadderOnStart = false;
				StartCoroutine (this.deployChute (v));
				return;
			}

			if (0f == v.evaController.propellantResourceDefaultAmount)
				ScreenMessages.PostScreenMessage (String.Format(
					"<color=orange>Jetpack propellant drained as tourists of level {0} are not allowed to use it</color>",
					t.level));
		}

		public IEnumerator deployChute(Vessel v) {
			Log.detail("Priming chute");

			if (ChuteSupport.INSTANCE.hasChute(v))
				yield return ChuteSupport.INSTANCE.deployChute(v, paraglidingDeployDelay, paraglidingChutePitch);
			else
			{
				ScreenMessages.PostScreenMessage ("<color=orange>I think I'm missing something...</color>");
				for (int i = 60; i > 0; --i) yield return null;
				ScreenMessages.PostScreenMessage ("<color=orange>No, really, there's something missing here...</color>");
				for (int i = 90; i > 0; --i) yield return null;
				ScreenMessages.PostScreenMessage ("<color=red>Wait!!!</color>");
				for (int i = 30; i > 0; --i) yield return null;
				ScreenMessages.PostScreenMessage ("<color=red>WHERE</color>");
				for (int i = 30; i > 0; --i) yield return null;
				ScreenMessages.PostScreenMessage ("<color=red>IS</color>");
				for (int i = 30; i > 0; --i) yield return null;
				ScreenMessages.PostScreenMessage ("<color=red>MY PARACHUTES???</color>");
			}
			yield break;
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
				if (!tourists.ContainsKey(crew.name) || // not among tourists
				    !Tourist.isTourist(crew) || // not really a tourist
				    crew.type != ProtoCrewMember.KerbalType.Crew)
				{
					// was probably unpromoted
					continue;
				}

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
				List<ProtoCrewMember> roster = vessel.GetVesselCrew ();
				if (0 == roster.Count) return;
				if (!tourists.TryGetValue(roster[0].name, out Tourist t)) return;
				if (!Tourist.isTourist(roster[0])) return;
				if (!t.hasAbility("EVA")) EVASupport.INSTANCE.equipHelmet(vessel);
			}
		}

		private void reinitEvents(Vessel v) {
			Log.dbg("entered reinitEvents for vessel {0}", v);
			foreach (Part p in v.Parts)
			{
				if (!"kerbalEVA".Equals(p.name)) continue;
				this.reinitEvents(p);
			}

			KerbalEVA evaCtl = v.evaController;
			if (null == evaCtl) return;

			List<ProtoCrewMember> roster = v.GetVesselCrew ();
			if (0 == roster.Count)
			{
				Log.dbg("Vessel has no crew.");
				return;
			}

			ProtoCrewMember crew = roster[0];
			String kerbalName = crew.name;
			Log.dbg("evCtl found; checking name: {0}", kerbalName);

			if (!tourists.TryGetValue(kerbalName, out Tourist t)) return;

			Log.dbg("among tourists: {0}", kerbalName);
			t.smile = false;
			t.taken = false;

			if (!Tourist.isTourist(crew)) {
				Log.dbg("...but is a crew, not a tourist!");
				return; // not a real tourist
			}

			// Change crew type right away to avoid them being crew after recovery
			crew.type = ProtoCrewMember.KerbalType.Tourist;

			EVASupport.INSTANCE.disableEvaEvents(v, t.hasAbility("EVA"));
			this.addSelfie(evaCtl);

			Log.dbg("Initializing sound");
			// Should we always invalidate cache???
			fx = null;
			getOrCreateAudio (evaCtl.part.gameObject);
		}

		// Adding Selfie button
		private void addSelfie(KerbalEVA evaCtl)
		{
			Log.dbg("Adding Selfie to {0}", evaCtl.GUIName);
			BaseEventList pEvents = evaCtl.Events;
			BaseEventDelegate slf = new BaseEventDelegate(TakeSelfie);
			KSPEvent evt = new KSPEvent
			{
				active = true,
				externalToEVAOnly = true,
				guiActive = true,
				guiActiveEditor = false,
				guiActiveUnfocused = false,
				guiActiveUncommand = false,
				guiName = "Take Selfie",
				name = "TakeSelfie"
			};
			BaseEvent selfie = new BaseEvent(pEvents, "Take Selfie", slf, evt);
			pEvents.Add (selfie);
			selfie.guiActive = true;
			selfie.active = true;
		}

		private void reinitEvents(Part p) {
			Log.dbg("entered reinitEvents for part {0}", p.name);
			if (null == p.Modules) return;
			if (null == p.protoModuleCrew || 0 == p.protoModuleCrew.Count) return;

			foreach (ProtoCrewMember crew in p.protoModuleCrew)
			{
				String kerbalName = crew.name;
				Log.dbg("found crew {0}", kerbalName);
				if (!tourists.TryGetValue(kerbalName, out Tourist t)) continue;
				Log.dbg("crew {0} {1}", kerbalName, t.abilities);
				EVASupport.INSTANCE.disableEvaEvents(p, t.hasAbility("EVA"));
				if (p.Modules.Contains<KerbalEVA>()) this.addSelfie(p.Modules.GetModule<KerbalEVA>());
			}
		}

		private void OnVesselGoOffRails(Vessel vessel)
		{
			Log.dbg("entered OnVesselGoOffRails={0}", vessel.name);

			reinitVessel (vessel);
			reinitEvents (vessel);
		}

		private void OnVesselChange(Vessel vessel)
		{
			Log.dbg("entered OnVesselChange={0}", vessel.name);
			if (vessel.evaController == null)
				return;

			// OnVesselChange called after OnVesselCreate, but with more things initialized
			reinitVessel (vessel);
			reinitEvents (vessel);
		}

		private void OnVesselLoad(Vessel vessel)
		{
			if (vessel == null) return;
			Log.dbg("entered OnVesselLoad={0}", vessel.name);

			// That's the problem - somewhere in the not so near past, KSP implemented a stunt called
			// UpgradePipeline. This thing acts after the PartModule's OnLoad handler, and it injects
			// back default values from prefab into the part on loading. This was intended to allow older
			// savegames to be loaded on newer KSP (as it would inject default values on missing atributes
			// present only on the new KSP version - or to reset new defaults when things changed internally),
			// but also ended up trashing changes and atributes only available on runtime for some custom modules.

			// The aftermath is that default (stock) KerbalEVA settings are always bluntly restored on load, and
			// we need to reapply them again by brute force.

			// Interesting enough, Kerbals on Seats are autonomous, runtime created Part attached to seat.

			reinitVessel (vessel);
			reinitEvents (vessel);

		}

		private void OnFlightReady()
		{
			Log.dbg("entered OnFlightReady");
			foreach (Vessel v in FlightGlobals.VesselsLoaded)
			{
				reinitVessel(v);
				reinitEvents(v);
			}
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

		private String pathname = null;
		public void FixedUpdate() {

			if (!HighLogic.LoadedSceneIsFlight)
				return;

			checkApproachingGeeLimit ();

			if (!smile)
				return;

			int sec = (DateTime.Now - selfieTime).Seconds;
			if (!taken && 0 == sec)
			{
				String fname = this.generateSelfieFileName();
				Log.info("Saving selfie to {0}", fname);
				this.pathname = KSPe.IO.Hierarchy.SCREENSHOT.Solve(fname);
			}
			// The emotions are taking more than a second to be executed by the Kerbal on KSP 1.7.3 . TODO: check other versions! It's something I did on KSPe, perhaps?
			if (!taken && 2 == sec)
			{
				Log.dbg("Getting snd");
				FXGroup snd = getOrCreateAudio(FlightGlobals.ActiveVessel.evaController.gameObject);
				if (snd != null) {
					snd.audio.Play ();
				}
				else Log.warn("snd is null");
			}

			if (!taken && 3 == sec)
			{ // The emotions are taking more than a second to be executed by the Kerbal on KSP 1.7.3 . TODO: check other versions!
				KSPe.Util.Image.Screenshot.Capture(this.pathname);
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
			Log.detail("Selfie...!");

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
				ProtoCrewMember crew = v.GetVesselCrew()[0];
				if (expression != null) {
					if (tourists.TryGetValue (crew.name, out Tourist t))
					{
						expression.wheeLevel = t.whee;
						expression.fearFactor = t.fear;
					}
					else // Allows crew members to always behave nicely!
					{
						expression.wheeLevel = 1f;
						expression.fearFactor = 0f;
					}

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
#if DEBUG
			if (obj == null)
				return "null";
			StringBuilder sb = new StringBuilder();
			try {
				Type t = typeof(T);
				System.Reflection.PropertyInfo[] props = t.GetProperties();
				if (props == null)
					return "type: " + t.ToString () + "; props=null";

				foreach (System.Reflection.PropertyInfo item in props)
				{
					sb.Append($"{item.Name}:{item.GetValue(obj,null)}; ");
				}
				sb.AppendLine();
			}
			catch (Exception e) {
				sb.Append ("Exception while trying to dump object: " + e.ToString ());
			}
			return sb.ToString ();
#else
			return "";
#endif
		}

		private kerbalExpressionSystem getOrCreateExpressionSystem(KerbalEVA p) {

			kerbalExpressionSystem e = p.part.GetComponent<kerbalExpressionSystem>();
			Log.dbg("expr. system: {0}", dumper(e));
			Log.dbg("kerbalEVA: {0}", dumper(p));
			Log.dbg("part: {0}", dumper(p.part));

			if (e == null) {

				AvailablePart evaPrefab = PartLoader.getPartInfoByName ("kerbalEVA");
				Log.dbg("eva prefab: {0}", dumper(evaPrefab));
				Part prefabEvaPart = evaPrefab.partPrefab;
				Log.dbg("eva prefab part: {0}", prefabEvaPart);

				ProtoCrewMember protoCrew = FlightGlobals.ActiveVessel.GetVesselCrew () [0];
				Log.dbg("proto crew: {0}", protoCrew);

				//kerbalExpressionSystem prefabExpr = prefabEva.GetComponent<kerbalExpressionSystem> ();

				Animator a = p.part.GetComponent<Animator> ();
				if (a == null) {
					Log.dbg("Creating Animator...");
					Animator prefabAnim = prefabEvaPart.GetComponent<Animator> ();
					Log.dbg("animator prefab: {0}", dumper(prefabAnim));
					a = p.part.gameObject.AddComponent<Animator> ();
					Log.dbg("animator component: {0}", dumper(a));

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
				Log.dbg("expression component: {0}", dumper(e));
			}
			return e;
		}

	}
}
