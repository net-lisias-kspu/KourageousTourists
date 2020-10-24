using UnityEngine;

namespace KourageousTourists.KSP.Chute.Stock14
{
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("KSP.Chute.14 Support Version {0}", Version.Text);
		}
	}
}
