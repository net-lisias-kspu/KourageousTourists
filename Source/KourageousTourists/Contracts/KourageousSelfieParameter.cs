using System;

namespace KourageousTourists.Contracts
{
	public class KourageousSelfieParameter: KourageousParameter 
	{

		public KourageousSelfieParameter() : base() {}

		public KourageousSelfieParameter(CelestialBody target, string kerbal) : base(target, kerbal) {}

		protected override string GetHashString() {
			return "walk" + this.targetBody.bodyName + this.tourist;
		}

		protected override string GetTitle() {
			return String.Format ("Take photo of {0} from the surface of {1}",
				tourist, targetBody.bodyName);
		}

		protected override string GetMessageComplete() {
			return String.Format ("{0} was pictured on the surface of {1}",
				tourist, targetBody.bodyName);
		}

		protected override void OnRegister() {
			KourageousTouristsAddOn.selfieListeners.Add (onSelfieTaken);
		}

		protected override void OnUnregister() {
			KourageousTouristsAddOn.selfieListeners.Remove (onSelfieTaken);
		}

		private void onSelfieTaken() {
			
			foreach (Vessel v in FlightGlobals.VesselsLoaded) {
				if (v.isEVA &&
					v.mainBody == targetBody &&
					v.GetVesselCrew().Count == 1 &&
					v.GetVesselCrew () [0].name.Equals (tourist) &&
					v.situation == Vessel.Situations.LANDED) {

					base.SetComplete ();
					break;
				}
			}
		}
	}

}

