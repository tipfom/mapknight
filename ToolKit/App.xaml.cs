using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        public static string StartupFile = null;

        public App ( ) {
            EmbeddedAssemblies.Serve( );
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; 
        }

        private void CurrentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e) {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\critical_error_log_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss_ffff");
            Exception exception = e.ExceptionObject as Exception;
            Directory.CreateDirectory(folder);
            using (FileStream stream = File.Create(folder + @"\error_stacktrace.txt"))
            using (StreamWriter writer = new StreamWriter(stream)) {
                writer.WriteLine("An critical Error occured.");
                writer.Write("Timestamp: ");
                writer.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:ffff"));
                if (exception != null) {
                    writer.Write("Stacktrace: ");
                    writer.WriteLine(exception.StackTrace);
                    writer.Write("Message: ");
                    writer.WriteLine(exception.Message);
                } else {
                    writer.WriteLine("the ExceptionObject was not of type Exception :(");
                }
                writer.Write("event: ");
                writer.WriteLine(e.ToString( ));
                writer.Write("exception-object: ");
                writer.WriteLine(e.ExceptionObject.ToString( ));
            }

            string projectfile = folder + @"\project.mkproj";
            File.Create(projectfile).Close( );
            ((Windows.EditorWindow)App.Current.MainWindow).CRASH_SAVE(projectfile);

            string text =
                "Hmm... It seems like we made an mistake!\n" +
                "Some application-thread threw an error we didnt think about. :(\n" +
                "Since we dont want your progress to be lost, we create saved your current progress to \n\"" + folder + "\"\nand additionally added an error stacktrace to make it easier to track down the error and remove it.\n" +
                (e.IsTerminating ? "\nIf you are reading this, then we have to appologize, but the error was so critical, the application wanted to terminate itself. :(" : "") + "\n\n" +
                "More information on the error:\n" +
                (exception != null ?
                "Message: " + exception.Message + "\n" +
                "Stacktrace: \n" + exception.StackTrace :
                "It seems the error is a really shitty one and there is no further information. ");
            MessageBox.Show(text, "Ooops!     (╯°□°）╯︵ ┻━┻", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Application_Startup (object sender, StartupEventArgs e) {
            if (e.Args.Length == 1) {
                StartupFile = e.Args[0];
            }
        }
    }
}
