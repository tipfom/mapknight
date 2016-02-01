using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using mapKnight.Utils;

namespace mapKnight.ToolKit
{
	static class Updater
	{
		private static string configfileurl = "https://drive.google.com/uc?export=download&id=0B6yHQ6ybOBYjYTE4X1I5V2Zsa2c";

		public enum UpdateResult
		{
			UpdateRequired,
			UpToDate,
			NoConnection
		}

		public static UpdateResult Check (Values.Version currentVersion)
		{
           if (Connected ()) {
				WebClient webClient = new WebClient ();
				webClient.DownloadFile (configfileurl, "mapknighttoolkit_configfile.xml");

				XMLElemental config = XMLElemental.Load (File.OpenRead ("mapknighttoolkit_configfile.xml"));
				File.Delete ("mapknighttoolkit_configfile.xml");

				if (new Values.Version (config ["version"].Value) > currentVersion) {
					return UpdateResult.UpdateRequired;
				} else {
					return UpdateResult.UpToDate;
				}
			} else {
				return UpdateResult.NoConnection;
			}
		}

		public static void Update ()
		{
            WebClient webClient = new WebClient();
            webClient.DownloadFile(configfileurl, "mapknighttoolkit_configfile.xml");

            XMLElemental config = XMLElemental.Load(File.OpenRead("mapknighttoolkit_configfile.xml"));
            File.Delete("mapknighttoolkit_configfile.xml");

            webClient.DownloadFile("https://drive.google.com/uc?export=download&id=" + config["installer"].Attributes["link"], "mapknight_installer_cache.exe");
            Process.Start("mapknight_installer_cache.exe");
        }

		//Creating the extern function...
		[DllImport ("wininet.dll")]
		private extern static bool InternetGetConnectedState (out int Description, int ReservedValue);

		//Creating a function that uses the API function...
		private static bool Connected ()
		{
			int Desc;
			return InternetGetConnectedState (out Desc, 0);
		}
	}
}