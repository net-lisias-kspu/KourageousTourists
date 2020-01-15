using UnityEngine;

namespace KourageousTourists
{
    internal class Startup : MonoBehaviour
	{
        private void Start()
        {
            Log.force("Version {0}", Version.Text);

            try
            {
                //KSPe.Util.Compatibility.Check<Startup>(typeof(Version), typeof(Configuration));
                KSPe.Util.Installation.Check<Startup>(typeof(Version));
            }
            catch (KSPe.Util.InstallmentException e)
            {
                Log.ex(this, e);
                KSPe.Common.Dialogs.ShowStopperAlertBox.Show(e);
            }
        }
	}
}
