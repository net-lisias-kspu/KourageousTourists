using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KourageousTourists
{
	public class TouristFactory
	{

		public Dictionary<int,ProtoTourist> touristConfig;
		public bool initialized = false;

		public TouristFactory ()
		{
			touristConfig = new Dictionary<int, ProtoTourist> ();
			initialized = readConfig ();
		}

		public Tourist createForLevel(int level, ProtoCrewMember crew) {

			Tourist t = new Tourist ();
			if (!initialized) {
				KourageousTouristsAddOn.printDebug ("TouristFactory not initialized, can't make tourists!");
				return t;
			}

			ProtoTourist pt;
			if (!touristConfig.TryGetValue (level, out pt)) {
				KourageousTouristsAddOn.printDebug ("Can't find config for level " + level);
				return t;
			}

			t.level = pt.level;
			t.abilities = pt.abilities;
			t.situations = pt.situations;
			t.celestialBodies = pt.celestialBodies;
			t.srfspeed = pt.srfspeed;
			t.crew = crew;
			t.rnd = new System.Random ();
			return t;
		}

		private bool readConfig() 
		{
			KourageousTouristsAddOn.printDebug ("reading config");
			ConfigNode config = GameDatabase.Instance.GetConfigNodes(KourageousTouristsAddOn.cfgRoot).FirstOrDefault();
			if (config == null) {
				KourageousTouristsAddOn.printDebug ("no config found in game database");
				return false;
			}

			// TODO: Remove this coupling
			String dbg = config.GetValue (KourageousTouristsAddOn.debugLog);
			if (dbg != null)
				KourageousTouristsAddOn.debug = dbg.ToLower ().Equals ("true");

			ConfigNode[] nodes = config.GetNodes (KourageousTouristsAddOn.cfgLevel);
			foreach (ConfigNode cfg in nodes) {

				String tLvl = cfg.GetValue("touristlevel");
				if (tLvl == null) {
					KourageousTouristsAddOn.printDebug ("tourist config entry has no attribute 'level'");
					return false;
				}

				KourageousTouristsAddOn.printDebug ("lvl=" + tLvl);
				ProtoTourist t = new ProtoTourist ();
				int lvl;
				if (!Int32.TryParse (tLvl, out lvl)) {
					KourageousTouristsAddOn.printDebug ("Can't parse tourist level as int: " + tLvl);
					return false;
				}
				t.level = lvl;

				if (cfg.HasValue("situations"))
					t.situations.AddRange(
						cfg.GetValue ("situations").Replace (" ", "").Split(','));
				t.situations.RemoveAll(str => String.IsNullOrEmpty(str));
				if (cfg.HasValue("bodies"))
					t.celestialBodies.AddRange(
						cfg.GetValue ("bodies").Replace (" ", "").Split (','));
				t.celestialBodies.RemoveAll(str => String.IsNullOrEmpty(str));
				if (cfg.HasValue("abilities"))
					t.abilities.AddRange(
						cfg.GetValue ("abilities").Replace (" ", "").Split (','));
				t.abilities.RemoveAll(str => String.IsNullOrEmpty(str));
				if (cfg.HasValue("srfspeed")) {
					String srfSpeed = cfg.GetValue ("srfspeed");
					KourageousTouristsAddOn.printDebug ("srfspeed = " + srfSpeed);
					double spd = 0.0;
					if (Double.TryParse (srfSpeed, out spd))
						t.srfspeed = spd;
					else
						t.srfspeed = Double.NaN;
				}

				KourageousTouristsAddOn.printDebug ("Adding cfg: " + t.ToString());
				this.touristConfig.Add (lvl, t);
			}
			return true;
		}
	}
}

