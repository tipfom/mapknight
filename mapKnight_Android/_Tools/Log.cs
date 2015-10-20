using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mapKnight_Android
{
	namespace Utils 
	{
		public static class Log{

			public static void All(object sender, string message, MessageType type, Exception ex = null)
			{
				All (sender.GetType (), message, type, ex);
			}

			public static void All(Type sender, string message, MessageType type, Exception ex = null)
			{
				switch (type) {
				case MessageType.Debug:
					Debug (tagRegister[sender] , message);
					break;
				case MessageType.Error:
					if (ex != null) {
						Error (tagRegister[sender], ex);
					} else {
						WTF("Log","Invalid Exception", new ArgumentException ("no error given"));
					}
					break;
				case MessageType.Info:
					Info (tagRegister[sender], message);
					break;
				case MessageType.Warn:
					Warn (tagRegister[sender], message);
					break;
				case MessageType.WTF:
					if (ex != null) {
						WTF (tagRegister[sender], message, ex);
					} else {
						WTF("Log","Invalid Exception", new ArgumentException ("no error given"));
					}
					break;
				}
			}

			public static void Debug (string tag, string message)
			{
				Android.Util.Log.Debug (tag, message);
			}

			public static void Error (string tag, Exception ex)
			{
				Android.Util.Log.Error (tag , ex.Message);
				Android.Util.Log.Info ( tag ,"ErrorSource = " + ex.Source);
				Android.Util.Log.Info ( tag ,"ErrorStack = " + ex.StackTrace);
			}

			public static void Info (string tag, string message)
			{
				Android.Util.Log.Info (tag, message);
			}

			public static void Warn (string tag, string message)
			{
				Android.Util.Log.Warn (tag , message);
			}

			public static void WTF (string tag, string message, Exception ex)
			{
				Android.Util.Log.Wtf ( tag , message);
				Android.Util.Log.Error ( tag ,"ErrorMessage " + ex.Message);
				Android.Util.Log.Info ( tag ,"ErrorSource = " + ex.Source);
				Android.Util.Log.Info ( tag,"ErrorStack = " + ex.StackTrace);
			}

			private static Dictionary<Type, string> tagRegister = new Dictionary<Type, string> () {
				{ typeof(SQLDataManager), "SQLDataManager" },
				{ typeof(XMLElemental), "XMLElemental" },
				{ typeof(GlobalContent),"GlobalContent" },
				{ typeof(CGL.CGLMap),"CGLMap" }
			};
		}
	}
}