using System;
using Contracts;

namespace KourageousTourists.Contracts
{
	public class KourageousSkydiveLandParameter : KourageousParameter
	{
		public KourageousSkydiveLandParameter () : base() {}

		public KourageousSkydiveLandParameter(CelestialBody target, String kerbal) : base(target, kerbal) {}

		protected override string GetHashString() {
			return "jump" + this.targetBody.bodyName + this.tourist;
		}

		protected override string GetTitle() {
			return String.Format ("Let {0} safely land on {1} with the parachute",
				tourist, targetBody.bodyName);
		}

		protected override void OnRegister() {
			Log.detail("setting event onVesselSituationChange");
			GameEvents.onVesselSituationChange.Add (OnSituationChange);
		}

		protected override void OnUnregister() {
			GameEvents.onVesselSituationChange.Remove (OnSituationChange);
		}

		private void OnSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> data) {
			if (!data.host.isEVA)
				return;
			
			checkCompletion (data.host);
		}
		
		private void checkCompletion(Vessel v) {

			foreach(ProtoCrewMember c in v.GetVesselCrew())
				Log.detail("param vessel crew: {0}",c.name);
			
			// Check that this tourist has already jumped out
			Log.detail("Checking jump parameter: {0}", Root.GetParameter(0).State);

			if (Root.GetParameter(0).State != ParameterState.Complete)
			{
				return;
			}
			
			if (v.isEVA &&
			    v.mainBody == targetBody &&
			    v.GetVesselCrew().Count == 1 &&
			    v.GetVesselCrew () [0].name.Equals(tourist) &&
			    v.situation == Vessel.Situations.LANDED)
				base.SetComplete ();
		}
	}
}

