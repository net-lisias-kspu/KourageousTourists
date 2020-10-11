using UnityEngine;

namespace KourageousTourists
{
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("KSP14 Support Version {0}", Version.Text);
		}
	}
}
