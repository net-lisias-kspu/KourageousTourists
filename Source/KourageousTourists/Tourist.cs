using System;
using System.Collections.Generic;


namespace KourageousTourists
{
	public class Tourist : ProtoTourist
	{
		public ProtoCrewMember crew;

		public bool smile;
		public bool taken;
		public float whee;
		public float fear;
		public System.Random rnd;


		internal Tourist() 
		{
			level = 0;
			abilities = new List<String> ();
			situations = new List<String> ();
			celestialBodies = new List<String> ();
			srfspeed = Double.NaN;
			crew = new ProtoCrewMember(ProtoCrewMember.KerbalType.Crew);
			whee = 0.0f;
			fear = 0.0f;
			rnd = new System.Random ();
			isSkydiver = false;
		}

		public bool hasAbility(String ability) {

			foreach (String a in this.abilities) {
				if (a.Equals (ability))
					return true;
			}
			return false;
		}

		public static bool isTourist(ProtoCrewMember crew) {
		
			return crew.trait.Equals (KerbalRoster.touristTrait);
		}

		public void generateEmotion() {

			// less courageous kerbals tend to express fear more often
			// more stupid kerbals tend to smile often: c*0.2 + s*(c - 0.5) 

			float type = (float)rnd.NextDouble () - 0.5f + crew.courage * 0.2f + 
				crew.stupidity * (crew.courage - 0.5f) ;
			whee = type;
			fear = -type;
		}

		public EVAAttempt canEVA(Vessel v)
		{
			if (!hasAbility("EVA")) 
				return new EVAAttempt(String.Format("Level {0} tourists can not go EVA at all", 
					level), false);

			string noSkyDiveMessage = "";
			EVAAttempt attempt;
			// Check for skydiving. 
			if (looksLikeSkydiving(v)) {
				attempt = checkSkydivingAttempt (v);
				if (attempt.status)
				{
					// if looks like skydiving and requirements are met - let them out
					return attempt;
				}

				noSkyDiveMessage = attempt.message + "; ";
			}
			
			// If we can go outside right away, so be it
			attempt = checkSituation (v);
			if (attempt.status) {
				return attempt;
			}

			attempt.message = noSkyDiveMessage + attempt.message;
			return(attempt);
		}

		internal bool looksLikeSkydiving(Vessel v) {
			return isSkydiver && v.mainBody.atmosphere && v.situation == Vessel.Situations.FLYING;
		}

		private EVAAttempt checkSituation(Vessel v)
		{
			String message = "";

			// Check if our situation is among allowed
			bool situationOk = situations.Count == 0;
			foreach (String situation in situations)
				if (v.situation.ToString().Equals(situation)) {
					situationOk = true;
					break;
				}

			bool celestialBodyOk = celestialBodies.Count == 0;
			foreach (String body in celestialBodies)
				if (v.mainBody.GetName().Equals(body)) {
					celestialBodyOk = true;
					break;
				}

			bool srfSpeedOk = Double.IsNaN(srfspeed) || Math.Abs (v.srfSpeed) < srfspeed;

			String preposition = " ";
			switch (v.situation) {
			case Vessel.Situations.LANDED:
			case Vessel.Situations.SPLASHED:
			case Vessel.Situations.PRELAUNCH:
				preposition = " at ";
				break;
			case Vessel.Situations.FLYING:
			case Vessel.Situations.SUB_ORBITAL:
			case Vessel.Situations.DOCKED:
				preposition = " above ";
				break;
			}
			message = String.Format ("Level {0} tourists can not go EVA when craft is {1}{2}{3}",
				level, v.situation.ToString ().ToLower ().Replace ("_", ""),
				preposition, v.mainBody.GetName ());
			if (!srfSpeedOk)
				message += String.Format (" and moving at speed {0:F2} m/s", v.srfSpeed);

			// message makes sense when they can not go EVA
			return new EVAAttempt(message, situationOk && celestialBodyOk && srfSpeedOk);
		}

		private EVAAttempt checkSkydivingAttempt(Vessel v) {
			// Check if the situation is safe enough for sky diving
			// Altitude above 1500m AGL
			// Speed below 100 m/s


			// if (this.level < 1) {
			// 	return new EVAAttempt ("This tourist is not trained for skydiving!", false);
			// }
			bool srfSpeedOk = Math.Abs (v.srfSpeed) < KourageousTouristsAddOn.paraglidingMaxAirspeed;
			bool altOk = v.radarAltitude > KourageousTouristsAddOn.paraglidingMinAltAGL;

			String skyDiveMessage = "";
			if (!srfSpeedOk) {
				skyDiveMessage = String.Format ("Air speed {0:F2} m/s is too fast", v.srfSpeed);
			}
			if (!altOk) {
				skyDiveMessage += String.Format ((srfSpeedOk ? "A" : " and a") + "ltitude {0} is not safe",
					(int)v.radarAltitude);
			}

			if (!srfSpeedOk || !altOk) {
				skyDiveMessage += " for skydiving";
			}
			return new EVAAttempt (skyDiveMessage, srfSpeedOk && altOk);
		}
	}
}

