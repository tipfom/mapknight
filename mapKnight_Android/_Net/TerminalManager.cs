using System;
using System.Linq;
using System.Collections.Generic;

namespace mapKnight.Android.Net
{
	public class TerminalManager
	{
		public bool Connected{ get; private set; }

		private Server server;

		public TerminalManager ()
		{
			server = new Server ();
			server.OnConnectionStateChanged += server_OnConnectionStateChanged;
			server.OnMessageReceived += server_OnMessageReceived;
		}

		private void server_OnMessageReceived (object sender, string e)
		{
			List<string> arguments = e.Split (new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList ();
			string command = arguments [0];
			arguments.RemoveAt (0);
			Log.All (this, "computing : " + e, MessageType.Info);

			switch (command) {
			// implement all commands here
			case "":
				break;
			}
		}

		private void server_OnConnectionStateChanged (object sender, bool e)
		{
			Connected = e;
			if (e == true) {
				Log.All (this, "connection acquired", MessageType.Info);
			} else {
				Log.All (this, "connection lost", MessageType.Info);
			}
		}

		public void Disconnect ()
		{
			server.Disconnect ();
		}
	}
}

