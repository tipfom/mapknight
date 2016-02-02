using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

using Microsoft.Win32;

using mapKnight.Utils;

namespace mapKnight.ToolKit.Installer
{
    class Program
    {
        private static string configfileurl = "https://drive.google.com/uc?export=download&id=0B6yHQ6ybOBYjYTE4X1I5V2Zsa2c";

        static void Main(string[] args)
        {
            // Header
            Console.WriteLine("===============================================================");
            Console.WriteLine("================== mapKnight ToolKit Updater ==================");
            Console.WriteLine("===============================================================");
            Console.WriteLine("");
            Console.WriteLine("log :");
            Console.WriteLine("");

            XMLElemental config = LoadConfig();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "mapKnight ToolKit");

            Console.WriteLine("> updating to version " + config["version"].Value);
            Console.WriteLine("> creating registry entries");
            Registry.ClassesRoot.CreateSubKey(".workfile").SetValue("", "mapknight_toolkit");
            Registry.ClassesRoot.CreateSubKey(@"mapknight_toolkit\shell\open\command").SetValue("", "\"" + path + @"\mapKnightToolKit.exe" + "\" \"%L\"");
            Registry.ClassesRoot.CreateSubKey(@"mapknight_toolkit\DefaultIcon").SetValue("", path + @"\files.ico");

            UpdateTKData("https://drive.google.com/uc?export=download&id=" + config["file"].Attributes["link"], path);

            // cleanup old stuff
            if (File.Exists("icon.ico")) File.Delete("icon.ico");
            if (File.Exists("files.ico")) File.SetAttributes("files.ico", FileAttributes.Hidden);

            Console.WriteLine("");
            Console.WriteLine("update sucessfull");
            Console.WriteLine("");

            Console.Write("press enter to exit ...");
            Console.ReadLine();

            Process.Start(Path.Combine(path, "mapKnightToolKit.exe"), "updatesuccessful");
        }

        private static XMLElemental LoadConfig()
        {
            Console.WriteLine("> downloading config from " + configfileurl);

            WebClient webClient = new WebClient();
            webClient.DownloadFile(configfileurl, "mapknighttoolkit_configfile.xml");

            XMLElemental config = XMLElemental.Load(File.OpenRead("mapknighttoolkit_configfile.xml"));

            Console.WriteLine("> deleting file mapknighttoolkit_configfile.xml");
            File.Delete("mapknighttoolkit_configfile.xml");

            return config;
        }

        private static void UpdateTKData(string downloadurl, string destinationdirectory)
        {
            Console.WriteLine("> downloading mapKnightToolKit from " + downloadurl);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(downloadurl, "mapknighttoolkit_cache.zip");
            
            Console.WriteLine("> extracting mapKnightToolKit from mapknighttoolkit_cache.zip");
            using (ZipArchive archive = ZipFile.OpenRead("mapknighttoolkit_cache.zip"))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    Console.WriteLine("> extracting " + entry.FullName);
                    try {
                        if (!Path.HasExtension (entry.FullName)) {
                            if (!Directory.Exists (Path.Combine (destinationdirectory, Path.GetDirectoryName (entry.FullName))))
                                Directory.CreateDirectory (Path.Combine (destinationdirectory, Path.GetDirectoryName (entry.FullName)));
                        } else {
                            entry.ExtractToFile (Path.Combine (destinationdirectory, entry.FullName), true);
                        }
                    } catch(Exception ex) {
                        Console.WriteLine ("> error while extracting " + entry.FullName);
                    }
                }
            }

            Console.WriteLine("> deleting file mapknighttoolkit_cache.zip");
            File.Delete("mapknighttoolkit_cache.zip");
        }
    }
}
