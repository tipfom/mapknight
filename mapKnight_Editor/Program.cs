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
            
			if (File.Exists ("mapKnightTK_Updater.exe"))
				File.Delete ("mapKnightTK_Updater.exe");

			if (Updater.Check (new mapKnight.Values.Version (Assembly.GetExecutingAssembly ().GetName ().Version.ToString ())) == Updater.UpdateResult.UpdateRequired) {
				if (MessageBox.Show ("Do you want to update the ToolKit?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
					Updater.Update ();
				} else {
					loadForm.Close ();
					Application.Run (new Main (null));
				}
			} else {
				if (args.Length > 0) {
					if (File.Exists (args [0]) && Path.GetExtension (args [0]) == ".workfile") {
						loadForm.Close ();
						Application.Run (new Main (args [0]));
					} else if (args [0] == "updatesuccessful") {
						loadForm.Close ();
						MessageBox.Show ("mapKnigth ToolKit has been updated successfully", "Update successful");
						Application.Run (new Main (null));
					}
				} else {
					loadForm.Close ();
					Application.Run (new Main (null));
				}
			}
		}
	}
}
