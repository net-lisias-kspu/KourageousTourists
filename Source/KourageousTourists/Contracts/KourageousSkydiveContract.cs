using System;
using System.Collections.Generic;
using System.Linq;

namespace KourageousTourists.Contracts
{
	public class KourageousSkydiveContract : KourageousContract
	{
		public KourageousSkydiveContract() : base()
		{
			IgnoresWeight = true;
		}

		protected override bool Generate()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			Log.detail("skydive generate");

			targetBody = selectNextCelestialBody();
			if (targetBody == null)
			{
				Log.detail("target body is null");
				return false;
			}

			numTourists = UnityEngine.Random.Range (1, 6);
			Log.detail("num tourists: {0}", numTourists);
			for (int i = 0; i < this.numTourists; i++) {
				ProtoCrewMember tourist = CrewGenerator.RandomCrewMemberPrototype (ProtoCrewMember.KerbalType.Tourist);

				tourists.Add (tourist);
				Log.detail("generated: {0}", tourist.name);

				KourageousSkydiveJumpParameter jumpParameter = new KourageousSkydiveJumpParameter(targetBody, tourist.name);
				jumpParameter.FundsCompletion = 0.0;
				jumpParameter.FundsFailure = 0.0;
				jumpParameter.ReputationCompletion = 0.0f;
				jumpParameter.ReputationFailure = 0.0f;
				jumpParameter.ScienceCompletion = 0.0f;
				AddParameter (jumpParameter);
				
				KourageousSkydiveLandParameter landParameter = new KourageousSkydiveLandParameter(targetBody, tourist.name);
				landParameter.FundsCompletion = 1000.0;
				landParameter.FundsFailure = 0.0;
				landParameter.ReputationCompletion = 0.0f;
				landParameter.ReputationFailure = 0.0f;
				landParameter.ScienceCompletion = 0.0f;
				AddParameter (landParameter);
			}

			GenerateHashString ();

			SetExpiry ();
			SetScience (0.0f, targetBody);
			SetDeadlineYears (1, targetBody);
			SetReputation (2, 5, targetBody);
			SetFunds (500, 2000, 15000, targetBody);

			return true;
		}
		
		protected new CelestialBody selectNextCelestialBody()
		{
			List<CelestialBody> allBodies = getCelestialBodyList().Where(
					b => b.atmosphere)
					.ToList();
			allBodies.Add(Planetarium.fetch.Home);

			Log.detail("skydive bodies: {0}", String.Join(", ", allBodies.Select(b => b.ToString()).ToArray()));

			if (allBodies.Count < 1)
				return null;
			return allBodies [UnityEngine.Random.Range (0, allBodies.Count - 1)];
		}

		protected override void OnAccepted() {
			Log.detail("entered: body= {0]", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.detail("adding to roster: {0}", tourist.name);
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
			string hash = "skydivecntrct-" + targetBody.bodyName;
			foreach (ProtoCrewMember tourist in this.tourists)
				hash += tourist.name;
			this.hashString = hash;
		}

		protected override string GetTitle () {
			return String.Format("Let {0} experience skydiving on {1}",
				getProperTouristWordLc(), targetBody.bodyName);
		}

		protected override string GetDescription() {
			return String.Format (
				"{0} just want to experience skydiving anywhere on {1}. Please note, that " +
				"for safety reasons air speed for jumping out should not exceed {2} m/s, " +
				"and altitude must be at least {3} m above the ground level. " +
				"The skydiver should land on a solid ground.",
				getProperTouristWord (), targetBody.bodyName, 
				KourageousTouristsAddOn.paraglidingMaxAirspeed,
				KourageousTouristsAddOn.paraglidingMinAltAGL);
		}

		protected override string GetSynopsys() {
			return String.Format (
				"Let {0} to jump out of the aircraft somewhere on {1}.",
				getProperTouristWordLc(), targetBody.bodyName
			);
		}

		protected override string MessageCompleted ()
		{
			return String.Format ("Skydiving charter for {0} was a success! This time your parachute riggers " +
				"made everything right.",
				getProperTouristWordLc()
			);
		}

		public override bool MeetRequirements ()
		{
			if (KourageousTouristsAddOn.noSkyDiving)
			{
				return false;
			}
			ProgressNode buzz = ProgressTracking.Instance.FindNode("TowerBuzz");

			if (buzz == null || !buzz.IsComplete)
			{
				Log.detail("buzz node node complete");
				return false;
			}	
			return true;
		}
	}


}

