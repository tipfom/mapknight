using System;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace mapKnight_Editor
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (File.Exists("mapKnightTK_Updater.exe")) File.Delete("mapKnightTK_Updater.exe");

            if (Updater.Check(new XML.Version(Assembly.GetExecutingAssembly().GetName().Version.ToString())) == Updater.UpdateResult.UpdateRequired)
            {
                if (MessageBox.Show("Do you want to update the ToolKit?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    Updater.Update();
                }
                else
                {
                    Application.Run(new Main(null));
                }
            }
            else
            {
                if (args.Length > 0)
                {
                    if (Directory.Exists(args[0]))
                    {
                        Application.Run(new Main(args[0]));
                    }
                    else if (args[0] == "updatesuccessful")
                    {
                        MessageBox.Show("mapKnigth ToolKit has been updated successfully", "Update successful");
                        Application.Run(new Main(null));
                    }
                }
                else
                {
                    Application.Run(new Main(null));
                }
            }
        }
    }
}
