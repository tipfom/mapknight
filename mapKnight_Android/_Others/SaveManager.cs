using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace mapKnight.Utils
{
	public class SaveManager
	{
		private SQLManager iDataManager;

		public SaveManager (string filename)
		{
			iDataManager = new SQLManager (filename);
		}

		public SaveStream Open (string packagename)
		{ 
			iDataManager.Create (packagename);
			return new SaveStream (this, packagename);
		}

		private string GetValue (string packagename, string key)
		{
			return iDataManager.Get (packagename, key);
		}

		private void SetValue (string packagename, string key, string value)
		{
			iDataManager.SetOrCreate (packagename, key, value);
		}

		public string Flush ()
		{
			return "";
		}

		public class SaveStream : IDisposable
		{
			SaveManager iManager;

			string iPackageName;

			public SaveStream (SaveManager manager, string packagename)
			{
				iManager = manager;
				iPackageName = packagename;
			}

			public void SetValue (string key, string value)
			{
				iManager.SetValue (this.iPackageName, key, value);
			}

			public void SetValue (string key, int value)
			{
				SetValue (key, value.ToString ());
			}

			public void SetValue (string key, float value)
			{
				SetValue (key, value.ToString ());
			}

			public void SetValue (string key, object value)
			{
				SetValue (key, value.ToString ());
			}

			public string GetString (string key)
			{
				return iManager.GetValue (this.iPackageName, key);
			}

			public int GetInt (string key)
			{
				int value; 
				if (int.TryParse (GetString (key), out value))
					return value;

				return 0;
			}

			public float GetFloat (string key)
			{
				float value; 
				if (float.TryParse (GetString (key), out value))
					return value;

				return 0f;
			}

			public void Dispose ()
			{
				iManager = null;
				GC.SuppressFinalize (this);
			}

			~SaveStream ()
			{
				Dispose ();
			}
		}
	}
}

