using System;
using System.Collections.Generic;
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

		protected CelestialBody selectNextCelestialBody() {

			List<CelestialBody> allBodies = Contract.GetBodies_Reached (false, false);
			foreach (CelestialBody body in allBodies)
				if (!body.hasSolidSurface)
					allBodies.Remove (body);
			if (allBodies.Count < 1)
				return null;
			return allBodies [UnityEngine.Random.Range (0, allBodies.Count - 1)];
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

		protected override void OnLoad (ConfigNode node)
		{
			int bodyID = int.Parse(node.GetValue ("targetBody"));
			foreach(var body in FlightGlobals.Bodies)
			{
				if (body.flightGlobalsIndex == bodyID)
					targetBody = body;
			}
			ConfigNode touristNode = node.GetNode ("TOURISTS");
			KourageousTouristsAddOn.printDebug ("tourist node: " + touristNode);
			if (touristNode == null) {
				KourageousTouristsAddOn.printDebug ("Can't load tourists from save file");
				return;
			}
			foreach (ConfigNode crewNode in touristNode.GetNodes()) {
				KourageousTouristsAddOn.printDebug ("tourist: " + crewNode);
				this.tourists.Add (
					new ProtoCrewMember (
						HighLogic.CurrentGame.Mode, crewNode, ProtoCrewMember.KerbalType.Tourist));
			}
			this.numTourists = tourists.Count;
			KourageousTouristsAddOn.printDebug ("numtourists: " + this.numTourists + "; " + tourists.Count);
		}

		protected override void OnSave (ConfigNode node)
		{
			//KourageousTouristsAddOn.printDebug ("saving " + this.numTourists + "tourists");
			//KourageousTouristsAddOn.printDebug ("node: " + node.ToString());
			int bodyID = targetBody.flightGlobalsIndex;
			node.AddValue ("targetBody", bodyID);
			ConfigNode touristNode = node.AddNode ("TOURISTS");
			foreach (ProtoCrewMember tourist in this.tourists) {
				ConfigNode crewNode = touristNode.AddNode ("TOURIST");
				tourist.Save (crewNode);
				//KourageousTouristsAddOn.printDebug ("adding tourist: " + tourist.name);
			}
			//KourageousTouristsAddOn.printDebug ("node: " + node.ToString());
			//KourageousTouristsAddOn.printDebug ("tourist node: " + touristNode.ToString());
			
		}

		protected static string tokenize(params Object[] args) {
			string result = "";
			int token = 0;
			foreach (var p in args) {
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

