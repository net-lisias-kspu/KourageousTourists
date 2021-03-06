﻿using System;
using System.Linq;

using UnityEngine;

namespace KourageousTourists.Contracts
{
	public class KourageousAnomalyParameter: KourageousParameter 
	{
		public const string discoveryDistance = "anomalyDiscoveryDistance";
		public const float defaultMinAnomalyDistance = 50;

		protected string anomalyName;
		protected string anomalyDisplayName;
		protected float minAnomalyDistance;


		public KourageousAnomalyParameter() : base() {}

		public KourageousAnomalyParameter(
			CelestialBody target, string kerbal, string anomaly, string displayName) : base(target, kerbal) {

			this.anomalyName = anomaly;
			this.anomalyDisplayName = displayName;

			setDistance ();
		}

		protected void setDistance() {
			ConfigNode config = GameDatabase.Instance.GetConfigNodes(KourageousTouristsAddOn.cfgRoot).FirstOrDefault();
			this.minAnomalyDistance = defaultMinAnomalyDistance;
			if (config != null) {

				String dscvr = config.GetValue (discoveryDistance);
				if (dscvr != null) {
					try {
						this.minAnomalyDistance = (float)Convert.ToDouble (dscvr);
					} catch (Exception e) {
						Log.error(e, this);
					}
				}
			}
			else
				Log.warn("no config found in game database");
		}

		protected override string GetHashString() {
			return "walk" + this.targetBody.bodyName + this.tourist;
		}

		protected override string GetTitle() {
			return String.Format ("Take photo of {0} from the surface of {1} near the {2}",
				tourist, targetBody.bodyName, anomalyDisplayName);
		}

		protected override string GetMessageComplete() {
			return String.Format ("{0} was pictured on the surface of {1} in the vicinity of {2}",
				tourist, targetBody.bodyName, anomalyDisplayName);
		}

		protected override void OnRegister() {
			KourageousTouristsAddOn.selfieListeners.Add (onSelfieTaken);
		}

		protected override void OnUnregister() {
			KourageousTouristsAddOn.selfieListeners.Remove (onSelfieTaken);
		}

		private void onSelfieTaken() {
			Log.dbg("onSelfieTaken");
			foreach (Vessel v in FlightGlobals.VesselsLoaded) {
				if (
					v.mainBody == targetBody &&
					v.GetVesselCrew().Count == 1 &&
					v.GetVesselCrew () [0].name.Equals (tourist) &&
					v.situation == Vessel.Situations.LANDED &&
					v.srfSpeed < 0.1f
				) {
					Log.dbg("checking for {0} at {1}", tourist, anomalyName);
					if (this.isNearbyAnomaly (v, anomalyName)) {
						base.SetComplete ();
					}
					break;
				}
			}
		}

		private bool isNearbyAnomaly(Vessel v, string anomalyName) {
			// FIXME: Can we have objects with same names, but on different bodies?
			// FIXME: So far I think we can.
			GameObject[] obj = UnityEngine.Object.FindObjectsOfType<GameObject>();
			foreach (GameObject anomalyObj in obj) {
				
				Component[] c = anomalyObj.GetComponents<PQSCity> ();
				if (c == null || c.Length == 0)
					continue;
				Log.dbg("has pqscity: {0}", anomalyObj.name);
				PQSCity pqscity = (PQSCity)c [0];
				if (pqscity == null)
					continue;

				if (!pqscity.sphere.isAlive)
					continue;

				Transform tr = anomalyObj.GetComponent<Transform> ();

				if (!anomalyObj.name.Equals (anomalyName))
					continue;
				if (tr == null)
					return false;
				float dist1 = Vector3.Distance (v.transform.position, tr.position);
				Log.dbg("distance: {0}; min dist: {1}", dist1, minAnomalyDistance);
				if (dist1 < this.minAnomalyDistance)
					return true;
			}
			return false;
		}
	
		protected override void OnLoad (ConfigNode node)
		{
			base.OnLoad (node);
			this.anomalyName = String.Copy(node.GetValue ("anomalyName"));
			KourageousAnomalyContract.readAnomalyConfig ();
			this.anomalyDisplayName = 
				KourageousAnomalyContract.anomalies [targetBody.name + ":" + anomalyName].anomalyDescription;
			Log.dbg("display name: {0}", anomalyDisplayName);
			setDistance ();
		}

		protected override void OnSave (ConfigNode node)
		{
			base.OnSave (node);
			node.AddValue ("anomalyName", anomalyName);
		}
	}
}

