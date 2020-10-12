using System;
using System.Collections.Generic;
using System.Linq;

using Contracts;

namespace KourageousTourists
{
	public class KourageousContract : Contract
	{

		protected CelestialBody targetBody = null;
		protected int numTourists;
		protected List<ProtoCrewMember> tourists;
		protected string hashString;

		public KourageousContract() : base() {
			this.tourists = new List<ProtoCrewMember> ();
			this.hashString = "";
			this.numTourists = 0;
		}

		public bool hasTourist(string tourist) {
			foreach (ProtoCrewMember crew in tourists) {
				if (crew.name == tourist) {
					return true;
				}
			}
			return false;
		}

		protected CelestialBody selectNextCelestialBody()
		{
			List<CelestialBody> allBodies = getCelestialBodyList();
			if (allBodies.Count < 1)
				return null;
			return allBodies[UnityEngine.Random.Range(0, allBodies.Count - 1)];
		}

		protected List<CelestialBody> getCelestialBodyList() {

			List<CelestialBody> allBodies = 
				GetBodies_Reached (false, false).Where(
					b => b.hasSolidSurface).ToList();
			Log.detail("celestials: {0}", String.Join(",", allBodies.Select(b => b.ToString()).ToArray()));
			return allBodies;
		}

		// Perhaps this is implemented somewhere in Contract.TextGen
		protected string getProperTouristWord() {

			string [] numbers = new[] {
				"No","One","Two","Three","Four","Five","Six","Seven","Eight","Nine","Ten","Eleven","Twelve"
			};

			string t;
			if (this.numTourists > 13)
				t = this.numTourists.ToString();
			else
				t = numbers[this.numTourists];
			return t + " tourist" + (this.numTourists > 1 ? "s" : "");
		}

		protected string getProperTouristWordLc() {
			string t = getProperTouristWord ();
			return char.ToLower (t [0]) + t.Substring (1);
		}

		protected override bool Generate() {
			return false;
		}

		protected override string GetHashString() {
			return this.hashString;
		}

		protected virtual void GenerateHashString () {}

		protected override void OnCompleted()
		{
			Log.detail("OnCompleted");

			foreach (ProtoCrewMember tourist in tourists)
			{
				Log.detail("Setting hasToured for {0}", tourist.name);
				KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;
				if (roster.Exists(tourist.name))
				{
					ProtoCrewMember t = roster[tourist.name];
					t.type = ProtoCrewMember.KerbalType.Tourist;
					t.hasToured = true;
				}
			}
			base.OnCompleted();
		}

		protected override void OnLoad (ConfigNode node)
		{
			int bodyID = int.Parse(node.GetValue ("targetBody"));
			foreach(CelestialBody body in FlightGlobals.Bodies)
			{
				if (body.flightGlobalsIndex == bodyID)
					targetBody = body;
			}
			ConfigNode touristNode = node.GetNode ("TOURISTS");
			Log.dbg("tourist node: {0}", touristNode);
			if (touristNode == null) {
				Log.warn("Can't load tourists from save file");
				return;
			}
			foreach (ConfigNode crewNode in touristNode.GetNodes()) {
				Log.dbg("tourist: {0}", crewNode);
				this.tourists.Add (
					new ProtoCrewMember (
						HighLogic.CurrentGame.Mode, crewNode, ProtoCrewMember.KerbalType.Tourist));
			}
			this.numTourists = tourists.Count;
			Log.detail("numtourists: {0}; {1}", this.numTourists, tourists.Count);
		}

		protected override void OnSave (ConfigNode node)
		{
			Log.dbg("saving {0} tourists", this.numTourists);
			Log.dbg("node: {0}", node);
			int bodyID = targetBody.flightGlobalsIndex;
			node.AddValue ("targetBody", bodyID);
			ConfigNode touristNode = node.AddNode ("TOURISTS");
			foreach (ProtoCrewMember tourist in this.tourists) {
				ConfigNode crewNode = touristNode.AddNode ("TOURIST");
				tourist.Save (crewNode);
				Log.dbg("adding tourist: {0}", tourist.name);
			}
			Log.dbg("node: {0}", node);
			Log.dbg("tourist node: {0}", touristNode);
			
		}

		protected static string tokenize(params Object[] args) {
			string result = "";
			int token = 0;
			foreach (Object p in args) {
				if (result.Length == 0) {
					result = p.ToString();
					continue;
				}
				result = result.Replace ("Token" + token, p.ToString());
				token++;
			}
			return result;
		}

		protected static string trainingHint(string body) {

			string hint = "Please note, tourists should be trained at least to level {0} to be able to disembark the" +
			              " vessel landed on {1}. Training usually could be performed by {2}";
			string[] trainings = {
				"the orbital flight and successful recovery.",
				"visiting Mun or Minmus and following safe recovery."
			};
			int requiredLevel = 0;
			if (body.Equals ("Mun") || body.Equals ("Minmus"))
				requiredLevel = 1;
			else
				requiredLevel = 2;

			return String.Format (hint, requiredLevel, body, trainings [requiredLevel - 1]);
		}
	}
}

