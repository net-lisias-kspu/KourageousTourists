using UnityEngine;

using Logger = KSPe.Util.Log.Logger;
using System.Diagnostics;

namespace KourageousTourists
{
    public static class Log
    {
        private static readonly Logger LOG = Logger.CreateForType<KourageousTouristsAddOn>();

		public static int debuglevel {
            get => (int)LOG.level;
            set => LOG.level = (KSPe.Util.Log.Level)(value % 6);
        }

        public static void force(string format, params object[] @parms)
        {
            LOG.force(format, parms);
        }

        public static void detail(string format, params object[] @parms)
        {
            LOG.detail(format, parms);
        }

        public static void info(string format, params object[] @parms)
        {
            LOG.info(format, parms);
        }

        public static void warn(string format, params object[] @parms)
        {
            LOG.warn(format, parms);
        }

        public static void err(string format, params object[] parms)
        {
            LOG.error(format, parms);
        }

		public static void err(System.Exception e, string format, params object[] parms)
		{
			LOG.error(e, format, parms);
		}

		public static void ex(MonoBehaviour offended, System.Exception e)
        {
            LOG.error(offended, e);
        }

        public static void ex(object offended, System.Exception e)
        {
            LOG.error(offended, e);
        }

        [Conditional("DEBUG")]
        public static void dbg(string format, params object[] @parms)
        {
            LOG.dbg(format, parms);
        }
    }
}
