using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace mapKnight.Android.Net
{
	public class Server
	{
		public event EventHandler<string> OnMessageReceived;
		public event EventHandler<Boolean> OnConnectionStateChanged;

		private const int bufferSize = 1024;
		private const int port = 1337;

		private static byte[] buffer = new byte[bufferSize];

		private IPEndPoint ipEndPoint = new IPEndPoint (IPAddress.Any, port);
		private Socket serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private Socket clientSocket;
		private bool connected = false;

		public Server ()
		{
			Begin ();
		}

		void Begin ()
		{
			try {
				serverSocket.Bind (ipEndPoint);
				serverSocket.Listen (0);
				serverSocket.BeginAccept (new AsyncCallback (AcceptCallback), serverSocket);
			} catch (Exception ex) {
				Log.All (this, "", MessageType.Error, ex);
			}
		}

		void AcceptCallback (IAsyncResult ar)
		{
			try {
				clientSocket = ((Socket)ar.AsyncState).EndAccept (ar);
				clientSocket.BeginReceive (buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback (ReceiveCallback), clientSocket);
				if (OnConnectionStateChanged != null)
					OnConnectionStateChanged (this, true);
				connected = true;
			} catch (Exception ex) {
				Log.All (this, "", MessageType.Error, ex);
			}
		}

		void ReceiveCallback (IAsyncResult ar)
		{
			try {
				int bytesReceived = ((Socket)ar.AsyncState).EndReceive (ar);
				string message = Encoding.ASCII.GetString (buffer, 0, bytesReceived);

				if (message == "__:disconnect:__") {
					clientSocket.Close ();
					if (OnConnectionStateChanged != null)
						OnConnectionStateChanged (this, false);
					connected = false;

					serverSocket.BeginAccept (new AsyncCallback (AcceptCallback), serverSocket);
					return;
				} else if (bytesReceived != 0 && OnMessageReceived != null) {
					OnMessageReceived (this, Encoding.ASCII.GetString (buffer, 0, bytesReceived));
				}
				
				clientSocket.BeginReceive (buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback (ReceiveCallback), clientSocket);
			} catch (Exception ex) {
				Log.All (this, "", MessageType.Error, ex);
			}
		}

		public void Send (string msg)
		{
			try {
				byte[] rawData = Encoding.ASCII.GetBytes (msg);
				if (rawData.Length < bufferSize) {
					clientSocket.BeginSend (rawData, 0, rawData.Length, SocketFlags.None, new AsyncCallback (SendingCallback), clientSocket);
				} else {
					Log.All (this, "the message you are trying to send is too long", MessageType.Debug);
				}
			} catch (Exception ex) {
				Log.All (this, "", MessageType.Error, ex);
			}
		}

		public void Disconnect ()
		{
			serverSocket.Close ();
			if (connected) {
				Send ("__:disconnect:__");
				clientSocket.Close ();
				if (OnConnectionStateChanged != null) {
					OnConnectionStateChanged (this, false);
				}
			}
		}

		void SendingCallback (IAsyncResult ar)
		{
			try {
				// returns the amount of bytes send
				int bytesSend = ((Socket)ar.AsyncState).EndSend (ar);
			} catch (Exception ex) {
				Log.All (this, "", MessageType.Error, ex);
			}
		}
	}
}

