﻿using System;
using System.Diagnostics;

#if DEBUG
using System.Collections.Generic;
#endif

using KSPe.Util.Log;

namespace KourageousTourists
{
	public static class Log
	{
		private static readonly Logger log = Logger.CreateForType<Startup>();

		public static void force (string msg, params object [] @params)
		{
			log.force (msg, @params);
		}

		public static void info(string msg, params object[] @params)
		{
			log.info(msg, @params);
		}

		public static void warn(string msg, params object[] @params)
		{
			log.warn(msg, @params);
		}

		public static void detail(string msg, params object[] @params)
		{
			StackTrace trace = new StackTrace ();
			String caller = trace.GetFrame(1).GetMethod ().Name;
			int line = trace.GetFrame (1).GetFileLineNumber ();
			string submsg = $"{caller}:line {line} :: ";
			log.detail(submsg + msg, @params);
		}

		public static void error(Exception e, object offended)
		{
			log.error(offended, e);
		}

		public static void error(Exception e, string msg, params object[] @params)
		{
			log.error(e, msg, @params);
		}

		public static void error(string msg, params object[] @params)
		{
			log.error(msg, @params);
		}

		[ConditionalAttribute("DEBUG")]
		public static void dbg(string msg, params object[] @params)
		{
			log.trace(msg, @params);
		}

		#if DEBUG
		private static readonly HashSet<string> DBG_SET = new HashSet<string>();
		#endif

		[ConditionalAttribute("DEBUG")]
		public static void dbgOnce(string msg, params object[] @params)
		{
			string new_msg = string.Format(msg, @params);
			#if DEBUG
			if (DBG_SET.Contains(new_msg)) return;
			DBG_SET.Add(new_msg);
			#endif
			log.trace(new_msg);
		}
	}
}
