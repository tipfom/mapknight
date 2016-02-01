using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

using mapKnight.Utils;

namespace mapKnight.Android.Net
{
	public class Server
	{
		public event EventHandler<byte[]> OnMessageReceived;
		public event EventHandler<Boolean> OnConnectionStateChanged;

		private const int port = 1337;

		private static byte[] buffer;

		private IPEndPoint ipEndPoint = new IPEndPoint (IPAddress.Any, port);
		private Socket serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private Socket clientSocket;
		private bool connected = false;

		public Server ()
		{
		}

		public void Begin ()
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

				if (buffer [0] == 255) { // close connection send
					clientSocket.Close ();
					if (OnConnectionStateChanged != null)
						OnConnectionStateChanged (this, false);
					connected = false;

					serverSocket.BeginAccept (new AsyncCallback (AcceptCallback), serverSocket);
					return;
				} else if (bytesReceived != 0 && OnMessageReceived != null) {
					OnMessageReceived (this, buffer.Cut (0, bytesReceived));
				}
			} catch (Exception ex) {
				Log.All (this, "", MessageType.Error, ex);
			}
		}

		public void Send (byte[] bytes)
		{
			try {
				clientSocket.BeginSend (bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback (SendingCallback), clientSocket);
			} catch (Exception ex) {
				Log.All (this, "", MessageType.Error, ex);
			}
		}

		public void Receive (int bufferLength = 1024)
		{
			buffer = new byte[bufferLength];
			clientSocket.BeginReceive (buffer, 0, bufferLength, SocketFlags.None, new AsyncCallback (ReceiveCallback), clientSocket);
		}

		private bool disconnected;

		public void Disconnect ()
		{
			if (!disconnected) {
				disconnected = true;

				if (connected) {
					Send (new byte[]{ 255 });
					clientSocket.Close ();
					if (OnConnectionStateChanged != null) {
						OnConnectionStateChanged (this, false);
					}
				}

				serverSocket.Close ();
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

