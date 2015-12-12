using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace mapKnight.Android.Net
{
	public class Client
	{
		public event EventHandler<string> OnMessageReceived;

		public readonly int Port;
		public readonly IPAddress ServerIP;

		private readonly Thread ListenThread;
		private readonly TcpListener tcpListener;
		private readonly TcpClient tcpClient;
		private readonly StreamWriter clientStreamWriter;

		private bool running;

		public Client (IPAddress serverip, int port)
		{
			this.Port = port;
			this.ServerIP = serverip;

			this.tcpListener = new TcpListener (IPAddress.Any, port);
			this.tcpClient = new TcpClient (serverip.ToString (), port);
			this.clientStreamWriter = new StreamWriter (tcpClient.GetStream ());

			this.ListenThread = new Thread (new ThreadStart (handleListen));
			this.ListenThread.Start ();
		}

		public void Send (string message)
		{
			clientStreamWriter.WriteLine (message);
			clientStreamWriter.Flush ();
		}

		public void Disconnect ()
		{
			running = false;
			tcpListener.Stop ();
			tcpClient.Close ();
		}

		private void handleListen ()
		{
			tcpListener.Start ();
			running = true;
			while (running) {
				if (tcpListener.Pending ()) {
					using (TcpClient listenerTCPClient = tcpListener.AcceptTcpClient ()) {
						using (StreamReader listenerStreamReader = new StreamReader (listenerTCPClient.GetStream ())) {
							OnMessageReceived (this, listenerStreamReader.ReadToEnd ());
						}
					}
				}
				ListenThread.Join (30);
			}
		}
	}
}

