using UnityEngine;

namespace KourageousTourists
{
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("KSP13.RealChute Support Version {0}", Version.Text);
		}
	}
}
