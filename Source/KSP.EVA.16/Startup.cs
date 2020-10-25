using UnityEngine;

namespace KourageousTourists.KSP.EVA.Stock16
{
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("KSP.EVA.16 Support Version {0}", Version.Text);
		}
	}
}
