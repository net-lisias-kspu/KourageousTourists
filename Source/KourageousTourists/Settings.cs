
using Asset = KSPe.IO.Asset<KourageousTourists.Startup>;
using Data = KSPe.IO.Data<KourageousTourists.Startup>;

namespace KourageousTourists
{
	internal static class Settings
	{
		private static readonly Data.ConfigNode SETTINGS = Data.ConfigNode.For(KourageousTouristsAddOn.cfgRoot, "Kourage.cfg");

		internal static ConfigNode Read()
		{
			if (!SETTINGS.IsLoadable)
			{
				Asset.ConfigNode defaults = Asset.ConfigNode.For(KourageousTouristsAddOn.cfgRoot, "Kourage.cfg");
				if (!defaults.IsLoadable)
				{
					Log.error("Where is the default Kourage.cfg? Kourageous Tourists will not work properly without it!");
					return null;
				}
				SETTINGS.Clear();
				SETTINGS.Save(defaults.Load().Node);
			}

			return SETTINGS.Load().Node;
		}
	}
}
