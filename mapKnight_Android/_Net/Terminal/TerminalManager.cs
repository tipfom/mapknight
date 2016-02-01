using System;
using System.Linq;
using System.Collections.Generic;

using mapKnight.Utils;

namespace mapKnight.Android.Net
{
	public class TerminalManager
	{
		private List<IMessageProcessor> messageProcessor = new List<IMessageProcessor> () { 
			new MessageProcessor.ContentMessageProcessor (),
			new MessageProcessor.AnimationMessageProcessor ()
		};

		public bool Connected{ get; private set; }

		private Server server;

		private int currentProcessor = -1;
		private Queue<int> currentMessageLengths = new Queue<int> ();

		public TerminalManager ()
		{
			server = new Server ();
			server.OnConnectionStateChanged += server_OnConnectionStateChanged;
			server.OnMessageReceived += server_OnMessageReceived;

			server.Begin ();
		}

		private void server_OnMessageReceived (object sender, byte[] e)
		{
			if (currentProcessor == -1) {
				foreach (int length in messageProcessor [(int)e [0]].BeginCompute (e.Cut (1, e.Length - 1))) {
					currentMessageLengths.Enqueue (length);
				}
				currentProcessor = e [0];
			} else {
				messageProcessor [currentProcessor].FillComputeData (e);

				if (currentMessageLengths.Count == 0) {
					currentProcessor = -1;
				}
			}

			if (currentMessageLengths.Count > 0) {
				server.Receive (currentMessageLengths.Dequeue ());
			} else {
				server.Receive ();
			}
		}

		private void server_OnConnectionStateChanged (object sender, bool e)
		{
			Connected = e;
			if (e == true) {
				Log.All (this, "connection acquired", MessageType.Info);
				server.Receive ();
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

