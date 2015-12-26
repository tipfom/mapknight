using System;
using System.Collections.Generic;
using System.Diagnostics;

using mapKnight.Utils;

namespace mapKnight.Android
{
	public static class Log
	{

		public static void All (object sender, string message, MessageType type, Exception ex = null)
		{
			All (sender.GetType (), message, type, ex);
		}

		public static void All (Type sender, string message, MessageType type, Exception ex = null)
		{
			switch (type) {
			case MessageType.Debug:
				Debug (tagRegister [sender], message);
				break;
			case MessageType.Error:
				if (ex != null) {
					Error (tagRegister [sender], ex);
				} else {
					WTF ("Log", "Invalid Exception", new ArgumentException ("no error given"));
				}
				break;
			case MessageType.Info:
				Info (tagRegister [sender], message);
				break;
			case MessageType.Warn:
				Warn (tagRegister [sender], message);
				break;
			case MessageType.WTF:
				if (ex != null) {
					WTF (tagRegister [sender], message, ex);
				} else {
					WTF ("Log", "Invalid Exception", new ArgumentException ("no error given"));
				}
				break;
			}
		}

		public static void Debug (string tag, string message)
		{
			global::Android.Util.Log.Debug (tag, message);
		}

		public static void Error (string tag, Exception ex)
		{
			global::Android.Util.Log.Error (tag, ex.Message);
			global::Android.Util.Log.Info (tag, "ErrorSource = " + ex.Source);
			global::Android.Util.Log.Info (tag, "ErrorStack = " + ex.StackTrace);
		}

		public static void Info (string tag, string message)
		{
			global::Android.Util.Log.Info (tag, message);
		}

		public static void Warn (string tag, string message)
		{
			global::Android.Util.Log.Warn (tag, message);
		}

		public static void WTF (string tag, string message, Exception ex)
		{
			global::Android.Util.Log.Wtf (tag, message);
			global::Android.Util.Log.Error (tag, "ErrorMessage " + ex.Message);
			global::Android.Util.Log.Info (tag, "ErrorSource = " + ex.Source);
			global::Android.Util.Log.Info (tag, "ErrorStack = " + ex.StackTrace);
		}

		private static Dictionary<Type, string> tagRegister = new Dictionary<Type, string> () {
			{ typeof(SQLManager), "SQLDataManager" },
			{ typeof(XMLElemental), "XMLElemental" },
			{ typeof(Content),"GlobalContent" },
			{ typeof(CGL.CGLMap),"CGLMap" },
			{ typeof(CGL.CGLTools),"CGLTools" },
			{ typeof(CGL.CGLRenderer),"CGLRenderer" },
			{ typeof(Touch),"Touch" },
			{ typeof(TouchManager),"TouchManager" },
			{ typeof(CGL.CGLInterface),"CGLInterface" },
			{ typeof(Net.Client),"SocketClient" },
			{ typeof(Net.Server), "SocketServer" },
			{ typeof(MainActivity), "Activity" },
			{ typeof(Net.TerminalManager),"TerminalManager" }
		};
	}
}