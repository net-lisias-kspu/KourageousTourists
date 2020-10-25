using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Log = KourageousTourists.Log;

namespace KourageousTourists.KSP.EVA.Stock13
{
	public class ExtraVehicularActivity : EVASupport.Interface
	{
		public ExtraVehicularActivity(){}

		private static readonly HashSet<string> EVENT_BLACKLIST = new HashSet<string>() {
			"MakeReference"
		};

		public void disableEvaEvents(Vessel v, bool isEvaEnabled)
		{
			if (null == v.evaController) return;

			KerbalEVA evaCtl = v.evaController;

			foreach (BaseEvent e in evaCtl.Events) {
				Log.dbg("disabling event {0} -- {1}", e.name, e.guiName);
				e.guiActive = false;
				e.guiActiveUnfocused = false;
				e.guiActiveUncommand = false;
			}
		}

		public void disableEvaEvents(Part p, bool isEvaEnabled)
		{
			this.disablePartEvents(p, isEvaEnabled);
		}

		private void disablePartEvents(Part p, bool isEvaEnabled)
		{
			foreach (BaseEvent e in p.Events) {
				// Everything not in the Black List will stay
				if (!EVENT_BLACKLIST.Contains(e.name)) continue;

				Log.dbg("disabling event {0} -- {1}", e.name, e.guiName);
				e.guiActive = false;
				e.guiActiveUnfocused = false;
				e.guiActiveUncommand = false;
			}
		}


		public bool isHelmetOn(Vessel v)
		{
			return true; // Helmet is always on on KSP 1.4
		}

		public void equipHelmet(Vessel v) { }	// No changes allowed on KSP 1.3
		public void removeHelmet(Vessel v) { }	// No changes allowed on KSP 1.3
	}
}
