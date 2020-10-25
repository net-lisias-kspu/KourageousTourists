using UnityEngine;

namespace KourageousTourists.KSP.EVA.KIS13
{
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("KSP.EVA.13.KIS Support Version {0}", Version.Text);
		}
	}
}
