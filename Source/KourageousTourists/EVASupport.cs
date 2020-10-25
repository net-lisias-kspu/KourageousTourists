using System.Collections;

namespace KourageousTourists
{
	public class EVASupport
	{
		public interface Interface
		{
			bool isHelmetOn(Vessel v);
			void equipHelmet(Vessel v);
			void removeHelmet(Vessel v);
			void disableEvaEvents(Vessel v, bool isEvaEnabled);
			void disableEvaEvents(Part v, bool isEvaEnabled);
		}

		internal static readonly Interface INSTANCE;
		private static Interface GetInstance()
		{
			Log.dbg("Looking for {0}", typeof(Interface).Name);
			foreach(System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
				foreach(System.Type type in assembly.GetTypes())
					foreach(System.Type ifc in type.GetInterfaces() )
					{
						Log.dbg("Checking {0} {1} {2}", assembly, type, ifc);
						if ("KourageousTourists.EVASupport+Interface" == ifc.ToString())
						{
							Log.dbg("Found it! {0}", ifc);
							object r = System.Activator.CreateInstance(type);
							Log.dbg("Type of result {0}", r.GetType());
							return (Interface)r;
						}
					}
			Log.error("No realisation for the abstract Interface found! We are doomed!");
			return (Interface) null;
		}
		static EVASupport()
		{
			INSTANCE = GetInstance();
		}

	}
}
