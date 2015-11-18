using System;

namespace mapKnight_Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			SaveManager testmanager = new SaveManager (System.IO.File.Open ("hallo.sv", System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite));
			SaveManager.SaveStream p1stream = testmanager.Open ("hallo");
			SaveManager.SaveStream p2stream = testmanager.Open ("hallo2");

			p1stream.SetValue ("wiegehts", "gut");
			p1stream.SetValue ("teilnehmer", 10);
			p1stream.SetValue ("floattest", 0.1337f);
			p2stream.SetValue ("meinnameist", "tim");

			Console.WriteLine (p1stream.GetString ("wiegehts"));
			Console.WriteLine (p1stream.GetFloat ("floattest"));

			p1stream.Dispose ();
			p2stream.Dispose ();
			Console.WriteLine (testmanager.Flush ());

			p1stream = testmanager.Open ("hallo2");
			p1stream.SetValue ("test2", "kallo");

			Console.WriteLine (testmanager.Flush ());
			Console.WriteLine (p1stream.GetString ("meinnameist"));
			Console.WriteLine ("");
			Console.ReadLine ();
		}
	}
}
