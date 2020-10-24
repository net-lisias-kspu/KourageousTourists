using System.Collections;

using UnityEngine;

using Log = KourageousTourists.Log;

using RealChute;

namespace KourageousTourists.KSP.Chute.RealChute14
{
	public class Chutes : ChuteSupport.Interface
	{
		public Chutes(){}

		public bool hasChute(Vessel v)
		{
			return false; // TODO Implement this for Real Chute!!
		}

		public IEnumerator deployChute(Vessel v, float paraglidingDeployDelay, float paraglidingChutePitch) {
			Log.detail("Priming chute - KSP14.RealChute");
			if (!v.evaController.part.Modules.Contains ("RealChuteModule")) {
				Log.detail("No RealChuteModule!!! Oops...");
				yield  break;
			}
			Log.detail("checking chute module...");
			RealChuteModule chuteModule = (RealChuteModule)v.evaController.part.Modules["RealChuteModule"];
			Log.detail("deployment state: armed {0}; enabled: {1}", chuteModule.armed, chuteModule.enabled);
			//chuteModule.deploymentSafeState = ModuleParachute.deploymentSafeStates.UNSAFE; // FIXME: is it immediate???

			Log.detail("counting {0} sec...", paraglidingDeployDelay);
			yield return new WaitForSeconds (paraglidingDeployDelay); // 5 seconds to deploy chute. TODO: Make configurable
			Log.detail("Deploying chute");
			chuteModule.ActivateRC();

			// Set low forward pitch so uncontrolled kerbal doesn't gain lot of speed
			//chuteModule.chuteDefaultForwardPitch = paraglidingChutePitch;
		}
	}
}
