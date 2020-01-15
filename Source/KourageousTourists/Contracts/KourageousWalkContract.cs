using System;
using System.Collections.Generic;
using Contracts;
using FinePrint.Contracts.Parameters;

namespace KourageousTourists
{
	public class KourageousWalkContract : KourageousContract
	{
		public KourageousWalkContract () : base () {}

		protected override bool Generate()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			Log.dbg("entered KourageousWalkContract Generate");

			targetBody = selectNextCelestialBody ();
			if (targetBody == null)
				return false;

			this.numTourists = UnityEngine.Random.Range (1, 5);
			Log.dbg("num tourists: {0}", numTourists);
			for (int i = 0; i < this.numTourists; i++) {
				ProtoCrewMember tourist = CrewGenerator.RandomCrewMemberPrototype (ProtoCrewMember.KerbalType.Tourist);

				tourists.Add (tourist);
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

				KourageousWalkParameter walkParameter = new KourageousWalkParameter (targetBody, tourist.name);
				walkParameter.FundsCompletion = 3000.0;
				walkParameter.FundsFailure = 0.0;
				walkParameter.ReputationCompletion = 0.0f;
				walkParameter.ReputationFailure = 0.0f;
				walkParameter.ScienceCompletion = 0.0f;
				itinerary.AddParameter (walkParameter);
			}

			GenerateHashString ();

			base.SetExpiry ();
			base.SetScience (0.0f, targetBody);
			base.SetDeadlineYears (1, targetBody);
			base.SetReputation (2, 5, targetBody);
			base.SetFunds (2000, 7000, 18000, targetBody);


			return true;
		}

		protected override void OnAccepted() {
			Log.dbg("entered: KourageousWalkContract OnAccepted body={0}", targetBody.bodyName);
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
			string hash = "walkcntrct-" + targetBody.bodyName;
			foreach (ProtoCrewMember tourist in this.tourists)
				hash += tourist.name;
			this.hashString = hash;
		}

		protected override string GetTitle () {
			return String.Format("Let {0} walk on the surface of {1}",
				getProperTouristWordLc(), targetBody.bodyName);
		}

		protected override string GetDescription() {
			return String.Format (
				"{0} want to practice their moon-walk by performing a real {1}-walk. Ferry them " +
				"there, let them out and return safely. {2}", getProperTouristWord(), targetBody.bodyName,
				trainingHint(targetBody.bodyName));
		}

		protected override string GetSynopsys() {
			return String.Format (
				"Ferry {0} to {1} and let them walk on the surface.",
				getProperTouristWordLc(), targetBody.bodyName
			);
		}

		protected override string MessageCompleted ()
		{
			return String.Format ("You have successfully returned {0} from the surface of {1}. They are pretty " +
			"impressed and had nothing but good time and brought back lot of memories",
				getProperTouristWordLc(), targetBody.bodyName
			);
		}

		public override bool MeetRequirements ()
		{
			// Later we should offer the contract only after some other tourist contract were completed
			return true;
		}
	}


}

