using System;
using System.Linq;
using System.Collections.Generic;
using Contracts;
using FinePrint.Contracts.Parameters;

namespace KourageousTourists
{

	public class KourageousAnomaly
	{
		public string name { get; set; }
		public string anomalyDescription { get; set; }
		public CelestialBody body { get; set; }
		public string contractDescription { get; set; }
		public string contractSynopsis { get; set; }
		public string contractCompletion { get; set; }
		public float payoutModifier;

		public void Save(ConfigNode node) {
			node.AddValue ("anomaly", body.name + ":" + name);
		}

		public static KourageousAnomaly Load(ConfigNode node, Dictionary<String, KourageousAnomaly> anomalies) {
			string anomalyName = node.GetValue ("anomaly");
			return (KourageousAnomaly)anomalies [anomalyName].MemberwiseClone ();
		}
	}

	public class KourageousAnomalyContract : KourageousContract
	{
		public const string anomalyCfgNode = "ANOMALY";
		public const string anomalyDistance = "anomalyDiscoveryDistance";
		internal static Dictionary<String, KourageousAnomaly> anomalies = null;
		protected KourageousAnomaly chosenAnomaly;
		protected static float anomalyDiscoveryDistance = 50.0f;

		public KourageousAnomalyContract () : base () {}

		internal static void readAnomalyConfig() {

			if (anomalies != null)
				return;
			
			anomalies = new Dictionary<String, KourageousAnomaly> ();
			ConfigNode config = GameDatabase.Instance.GetConfigNodes(KourageousTouristsAddOn.cfgRoot).FirstOrDefault();
			if (config == null)
				return;


			String distanceNode = config.GetValue (anomalyDistance);
			if (distanceNode != null) {
				try {
					anomalyDiscoveryDistance = (float)Convert.ToDouble(distanceNode);
				}
				catch(Exception e) {
					Log.error(e, "KourageousAnomalyContract readAnomalyConfig");
				}
			}

			ConfigNode[] nodes = config.GetNodes (anomalyCfgNode);
			foreach (ConfigNode node in nodes) {

				KourageousAnomaly anomaly = new KourageousAnomaly ();

				Log.dbg("cfg node: {0}", node);
				String name = node.GetValue("name");
				if (name == null)
					continue;
				anomaly.name = name;

				Log.dbg("anomaly name: {0}", name);
				String anomalyDescription = node.GetValue ("anomalyDescription");
				if (anomalyDescription == null)
					continue;
				anomaly.anomalyDescription = anomalyDescription;

				String contractDescription = node.GetValue ("contractDescription");
				if (contractDescription == null)
					continue;
				anomaly.contractDescription = contractDescription;

				String contractSynopsis = node.GetValue ("contractSynopsis");
				if (contractSynopsis == null)
					continue;
				anomaly.contractSynopsis = contractSynopsis;

				String bodyStr = node.GetValue ("body");
				Log.dbg("anomaly body: {0}", bodyStr);

				foreach (CelestialBody b in FlightGlobals.Bodies) {
					Log.dbg("list body name: {0}", b.name);
					if (b.name.Equals (bodyStr)) {
						anomaly.body = b;
						break;
					}
				}
				Log.dbg("anomaly body obj: {0}", anomaly.body == null);
				if (anomaly.body == null)
					continue;
				
				String payoutModifierStr = node.GetValue ("payoutModifier");
				Log.dbg("payout modifier str: {0}", payoutModifierStr);
				if (payoutModifierStr == null)
					continue;
				float payoutModifier = 1.0f;
				try {
					payoutModifier = (float)Convert.ToDouble(payoutModifierStr);
					Log.dbg("payout modifier: {0}", payoutModifier);
				}
				catch(Exception e) {
					Log.error(e, "readAnomalyConfig");
				}
				anomaly.payoutModifier = payoutModifier;

				anomalies.Add (bodyStr + ":" + name, anomaly);
				Log.dbg("added: {0}", bodyStr + ":" + name);
			}
			
		}

		protected KourageousAnomaly chooseAnomaly(CelestialBody body) {

			Log.dbg("entered KourageousAnomallyContract chooseAnomaly");
			readAnomalyConfig ();
			Log.dbg("anomalies: {0}, distance: {1}", anomalies.Count, anomalyDiscoveryDistance);

			List<KourageousAnomaly> chosen = new List<KourageousAnomaly> ();
			foreach (KeyValuePair<string, KourageousAnomaly> entry in anomalies)
				if (entry.Value.body.name.Equals (body.name))
					chosen.Add (entry.Value);

			Log.dbg("chosen: {0}, cnt: {1}", chosen, chosen.Count);
			if (chosen.Count == 0)
				return null;

			Random rnd = new Random ();
			return chosen [rnd.Next (chosen.Count)];
		}


		protected override bool Generate()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			Log.dbg("entered KourageousAnomallyContract Generate");

			targetBody = selectNextCelestialBody ();
			if (targetBody == null)
				return false;

			chosenAnomaly = chooseAnomaly (targetBody);
			if (chosenAnomaly == null)
				return false;
			
			this.numTourists = UnityEngine.Random.Range (2, 5);
			Log.dbg("num tourists: {0}", numTourists);
			for (int i = 0; i < this.numTourists; i++) {
				ProtoCrewMember tourist = CrewGenerator.RandomCrewMemberPrototype (ProtoCrewMember.KerbalType.Tourist);

				this.tourists.Add (tourist);
				Log.dbg("generated: {0}", tourist.name);

				// TODO: Add support for gender for 1.3 build
				KerbalTourParameter itinerary = new KerbalTourParameter (tourist.name, tourist.gender);
				// TODO: Add difficulty multiplier
				itinerary.FundsCompletion = 25000.0;
				itinerary.ReputationCompletion = 0.0f;
				itinerary.ReputationFailure = 0.0f;
				itinerary.ScienceCompletion = 0.0f;
				this.AddParameter (itinerary);

				KerbalDestinationParameter dstParameter = new KerbalDestinationParameter (
					targetBody, FlightLog.EntryType.Land, tourist.name
				);
				dstParameter.FundsCompletion = 1000.0f;
				dstParameter.FundsFailure = 0.0f;
				dstParameter.ReputationCompletion = 0.0f;
				dstParameter.ReputationFailure = 0.0f;
				dstParameter.ScienceCompletion = 0.0f;
				/*dstParameter.NestToParent (itinerary);
				dstParameter.CreateID ();
				dstParameter.AddParameter (new Contracts.Parameters.LandOnBody (targetBody));*/
				itinerary.AddParameter (dstParameter);


				KourageousAnomalyParameter anomalyParameter = new KourageousAnomalyParameter (
					targetBody, tourist.name, chosenAnomaly.name, chosenAnomaly.anomalyDescription
				);
				anomalyParameter.FundsCompletion = 1300.0;
				anomalyParameter.FundsFailure = 0.0;
				anomalyParameter.ReputationCompletion = 1.0f;
				anomalyParameter.ReputationFailure = 1.0f;
				anomalyParameter.ScienceCompletion = 0.0f;
				itinerary.AddParameter (anomalyParameter);
			}

			GenerateHashString ();

			base.SetExpiry ();
			base.SetScience (0.0f, targetBody);
			base.SetDeadlineYears (1, targetBody);
			base.SetReputation (2, 5, targetBody);
			base.SetFunds (
				3000 * chosenAnomaly.payoutModifier, 
				9000 * chosenAnomaly.payoutModifier, 
				21000 * chosenAnomaly.payoutModifier, 
				targetBody);

			return true;
		}

		protected override void OnAccepted() {
			Log.dbg("entered: KourageousAnomallyContract Generate body={0}", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.dbg("adding to roster: {0}", tourist.name);
			}
		}


		public override bool CanBeCancelled() {
			// TODO: Let's make that if any tourist is out of Kerbin, 
			// the contract can't be cancelled
			return true;
		}

		public override bool CanBeDeclined() {
			return true;
		}

		protected override void GenerateHashString() {
			string hash = "selfiecntrct-" + targetBody.bodyName;
			foreach (ProtoCrewMember tourist in this.tourists)
				hash += tourist.name;
			this.hashString = hash;
		}

		protected override string GetTitle () {
			Log.dbg("entered: KourageousAnomallyContract GetTitle anomaly={0}", chosenAnomaly);
			return String.Format("Visit {0} with {1}",
				chosenAnomaly.anomalyDescription,  getProperTouristWordLc());
		}

		protected override string GetDescription() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractDescription, getProperTouristWord(), anomalyDiscoveryDistance,
				trainingHint(chosenAnomaly.body.bodyName));
		}

		protected override string GetSynopsys() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractSynopsis, getProperTouristWordLc(), anomalyDiscoveryDistance);
		}

		protected override string MessageCompleted ()
		{
			return KourageousContract.tokenize (chosenAnomaly.contractCompletion,
				getProperTouristWordLc (), anomalyDiscoveryDistance);
		}

		public override bool MeetRequirements ()
		{
			// Later we should offer the contract only after some other tourist contract were completed
			return true;
		}

		protected override void OnSave (ConfigNode node) {
			base.OnSave (node);
			chosenAnomaly.Save(node);
		}

		protected override void OnLoad(ConfigNode node) {
			base.OnLoad (node);
			readAnomalyConfig ();
			chosenAnomaly = KourageousAnomaly.Load (node, anomalies);
		}
	}
}

