using System;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace mapKnight.ToolKit
{
	static class Program
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main (string[] args)
		{
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
            
			LoadForm loadForm = new LoadForm ();
			loadForm.Show ();
			Application.DoEvents ();

            // cleanup
			if (File.Exists("mapKnightTK_Updater.exe"))
            {
                // older updater has been launched
                if (MessageBox.Show("You have updated the mapKnight ToolKit from an older version using an outdated integrated updater.\n Do you want to refresh the installation to accomplish the current state of the ToolKit?", "Update",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    File.Delete("mapKnightTK_Updater.exe");
                    Updater.Update();
                }
            } else
            {
                if (File.Exists("mapknight_installer_cache.exe"))
                    File.Delete("mapknight_installer_cache.exe");

                if (Updater.Check(new mapKnight.Values.Version(Assembly.GetExecutingAssembly().GetName().Version.ToString())) == Updater.UpdateResult.UpdateRequired)
                {
                    if (MessageBox.Show("Do you want to update the ToolKit?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        Updater.Update();
                    }
                    else
                    {
                        loadForm.Close();
                        Application.Run(new Main(null));
                    }
                }
                else
                {
                    if (args.Length > 0)
                    {
                        if (File.Exists(args[0]) && Path.GetExtension(args[0]) == ".workfile")
                        {
                            loadForm.Close();
                            Application.Run(new Main(args[0]));
                        }
                        else if (args[0] == "updatesuccessful")
                        {
                            loadForm.Close();
                            Application.Run(new WhatsNewForm());
                            Application.Run(new Main(null));
                        }
                    }
                    else
                    {
                        loadForm.Close();
                        Application.Run(new Main(null));
                    }
                }
            }
		}
    }
}
