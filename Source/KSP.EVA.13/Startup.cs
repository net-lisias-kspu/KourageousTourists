using UnityEngine;

namespace KourageousTourists.KSP.EVA.Stock13
{
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("KSP.EVA.13 Support Version {0}", Version.Text);
		}
	}
}
