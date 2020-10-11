using System;

namespace KourageousTourists.Contracts
{
	public class KourageousSkydiveJumpParameter : KourageousParameter
	{
		public KourageousSkydiveJumpParameter () : base() {}

		public KourageousSkydiveJumpParameter(CelestialBody target, String kerbal) : base(target, kerbal) {}

		protected override string GetHashString() {
			return "jump" + this.targetBody.bodyName + this.tourist;
		}

		protected override string GetTitle() {
			return String.Format ("Let {0} jump from the craft in the skies of {1}",
				tourist, targetBody.bodyName);
		}

		protected override void OnRegister() {
			Log.detail("setting event OnEva");
			GameEvents.onCrewOnEva.Add (OnEva);
			GameEvents.onVesselSituationChange.Add (OnSituationChange);
		}

		protected override void OnUnregister() {
			GameEvents.onCrewOnEva.Remove (OnEva);
			GameEvents.onVesselSituationChange.Remove (OnSituationChange);
		}

		private void OnSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> data) {
			if (!data.host.isEVA)
				return;
			
			checkCompletion (data.host);
		}

		private void OnEva(GameEvents.FromToAction<Part, Part> action) {
			Vessel v = action.to.vessel;
			Log.detail(
				"triggered; vessel: {0}, {1}; param tourist: {2}; body: {3}; vessel situation: {4}; vessel body: {5}",
				action.to.vessel, action.from.vessel, this.tourist, this.targetBody.bodyName, v.situation, v.mainBody.bodyName
				);
			
			checkCompletion (v);
		}

		private void checkCompletion(Vessel v) {

			Log.detail("checking param: vessel crew: {0}; alt: {1}", v.GetVesselCrew()[0].name, v.radarAltitude);
			if (v.isEVA &&
			    v.mainBody == targetBody &&
			    v.GetVesselCrew().Count == 1 &&
			    v.GetVesselCrew()[0].name.Equals(tourist) &&
			    v.situation == Vessel.Situations.FLYING &&
			    v.radarAltitude > 1300)
			{
				Log.detail("Setting 'complete'");
				base.SetComplete();
			}
		}
	}
}

