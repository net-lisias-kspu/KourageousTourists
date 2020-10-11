using UnityEngine;

namespace KourageousTourists.GUI
{
	internal static class ShowStopperAlertBox
	{
		private static readonly string MSG = @"Unfortunately Kourageous Tourists /L will not run on your current installment!

The problem detected is that {0}.";

		private static readonly string AMSG = @"Install all the dependencies for your current installment.";

		internal static void Show(string reason)
		{
			KSPe.Common.Dialogs.ShowStopperAlertBox.Show(
				string.Format(MSG, reason),
				AMSG,
				() => { Application.Quit(); }
			);
			Log.detail("\"Houston, we have a Problem!\" was displayed about {0}", reason);
		}
	}
}