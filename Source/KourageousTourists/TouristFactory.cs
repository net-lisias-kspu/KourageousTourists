using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contracts;
using KourageousTourists.Contracts;

using Asset = KSPe.IO.Asset<KourageousTourists.Startup>;
using Data = KSPe.IO.Data<KourageousTourists.Startup>;

namespace KourageousTourists
{
	public class TouristFactory
	{

		public Dictionary<int,ProtoTourist> touristConfig;
		public bool initialized = false;

        private static readonly Data.ConfigNode SETTINGS = Data.ConfigNode.For(KourageousTouristsAddOn.cfgRoot, "Kourage.cfg");

		public TouristFactory ()
		{
			touristConfig = new Dictionary<int, ProtoTourist> ();
			initialized = readConfig ();
		}

		public Tourist createForLevel(int level, ProtoCrewMember crew) {

			Tourist t = new Tourist ();
			if (!initialized) {
				Log.warn("TouristFactory not initialized, can't make tourists!");
				return t;
			}

			ProtoTourist pt;
			if (!touristConfig.TryGetValue (level, out pt)) {
				Log.warn("Can't find config for level " + level);
				return t;
			}

			t.level = pt.level;
			t.abilities = pt.abilities;
			t.situations = pt.situations;
			t.celestialBodies = pt.celestialBodies;
			t.srfspeed = pt.srfspeed;
			t.crew = crew;
			t.rnd = new System.Random ();
			t.isSkydiver = isSkyDiver (crew);
			return t;
		}

		public static bool isSkyDiver(ProtoCrewMember crew) {
			// Check if this kerbal is participating in any skydiving contract
			if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
			{
				return false;
			}
			foreach (Contract c in ContractSystem.Instance.Contracts)
			{
				var contract = c as KourageousSkydiveContract;
				if (contract != null) {
					if (contract.hasTourist (crew.name)) {
						return true;
					}
				}
			}
			return false;
		}

		private bool readConfig()
		{
			Log.dbg("reading config");

			if (!SETTINGS.IsLoadable)
			{
				Asset.ConfigNode defaults = Asset.ConfigNode.For(KourageousTouristsAddOn.cfgRoot, "Kourage.cfg");
				if (!defaults.IsLoadable)
				{
					Log.error("Where is the default Kourage.cfg? Kourageous Tourists will not work properly without it!");
					return false;
				}
				SETTINGS.Clear();
				SETTINGS.Save(defaults.Load().Node);
			}

			ConfigNode config = SETTINGS.Load().Node;

			if (config == null) {
				Log.dbg("no config found in game database");
				return false;
			}

			ConfigNode[] nodes = config.GetNodes (KourageousTouristsAddOn.cfgLevel);
			foreach (ConfigNode cfg in nodes) {

				String tLvl = cfg.GetValue("touristlevel");
				if (tLvl == null) {
					Log.dbg("tourist config entry has no attribute 'level'");
					return false;
				}

				Log.dbg("lvl={0}", tLvl);
				ProtoTourist t = new ProtoTourist ();
				int lvl;
				if (!Int32.TryParse (tLvl, out lvl)) {
					Log.dbg("Can't parse tourist level as int: {0}", tLvl);
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
					Log.dbg("srfspeed = {0}", srfSpeed);
					double spd = 0.0;
					if (Double.TryParse (srfSpeed, out spd))
						t.srfspeed = spd;
					else
						t.srfspeed = Double.NaN;
				}

				Log.dbg("Adding cfg: {0}", t);
				this.touristConfig.Add (lvl, t);
			}
			return true;
		}
	}
}

