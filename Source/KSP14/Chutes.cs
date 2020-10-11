using System.Collections;

using UnityEngine;

using Log = KourageousTourists.Log;

namespace KourageousTourists.KSP14
{
	public class Chutes : ChuteSupport.Interface
	{
		public Chutes(){}

		public IEnumerator deployChute(Vessel v, float paraglidingDeployDelay, float paraglidingChutePitch) {
			Log.detail("Priming chute - KSP14");
			if (!v.evaController.part.Modules.Contains ("ModuleEvaChute")) {
				Log.detail("No ModuleEvaChute!!! Oops...");
				yield  break;
			}
			Log.detail("checking chute module...");
			ModuleEvaChute chuteModule = (ModuleEvaChute)v.evaController.part.Modules ["ModuleEvaChute"];
			Log.detail("deployment state: {0}; enabled: {1}", chuteModule.deploymentSafeState, chuteModule.enabled);
			chuteModule.deploymentSafeState = ModuleParachute.deploymentSafeStates.UNSAFE; // FIXME: is it immediate???

			Log.detail("counting {0} sec...", paraglidingDeployDelay);
			yield return new WaitForSeconds (paraglidingDeployDelay); // 5 seconds to deploy chute. TODO: Make configurable
			Log.detail("Deploying chute");
			chuteModule.Deploy ();

			// Set low forward pitch so uncontrolled kerbal doesn't gain lot of speed
			chuteModule.chuteDefaultForwardPitch = paraglidingChutePitch;
		}
	}
}
