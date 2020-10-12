using System.Collections;

namespace KourageousTourists
{
	public static class ChuteSupport
	{
		public interface Interface
		{
			bool hasChute(Vessel v);
			IEnumerator deployChute(Vessel v, float paraglidingDeployDelay, float paraglidingChutePitch);
		}

		internal static readonly Interface INSTANCE;
		private static Interface GetInstance()
		{
			Log.dbg("Looking for {0}", typeof(Interface).Name);
			foreach(System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
				foreach(System.Type type in assembly.GetTypes())
					foreach(System.Type ifc in type.GetInterfaces() )
					{
						#if DEBUG
							UnityEngine.Debug.LogFormat("[KSP.UI] Checking {0} {1} {2}", assembly, type, ifc);
						#endif
						Log.dbg("Checking {0} {1} {2}", assembly, type, ifc);
						if ("KourageousTourists.ChuteSupport+Interface" == ifc.ToString())
						{
							Log.dbg("Found it! {0}", ifc);
							object r = System.Activator.CreateInstance(type);
							Log.dbg("Type of result {0}", r.GetType());
							return (Interface)r;
						}
					}
			UnityEngine.Debug.LogWarning("No realisation for the abstract Interface found! We are doomed!");
			return (Interface) null;
		}
		static ChuteSupport()
		{
			INSTANCE = GetInstance();
		}

	}
}
