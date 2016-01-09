using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using mapKnight.Android;

namespace mapKnight.Android.Net
{
	public class Client
	{
		public event EventHandler<string> OnMessageReceived;
		public event EventHandler<Boolean> OnConnectionStateChanged;

		private const int bufferSize = 1024;
		private const int port = 1337;

		private byte[] buffer = new byte[bufferSize];

		private IPEndPoint ipEndPoint;
		private Socket clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		public Client (IPAddress serverIP)
		{
			ipEndPoint = new IPEndPoint (serverIP, port);
			Begin ();
		}

		private void Begin ()
		{
			try {
				clientSocket.BeginConnect (ipEndPoint, new AsyncCallback (ConnectCallback), clientSocket);
			} catch (Exception ex) {
				Log.All (this, "no connection could be acquired", MessageType.Error, ex);
			}
		}

		void ConnectCallback (IAsyncResult ar)
		{
			try {
				// casts the object parsed to the .BeginConnect method and calls the method to complete the connection-phase
				((Socket)ar.AsyncState).EndConnect (ar);
				((Socket)ar.AsyncState).BeginReceive (buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback (ReceiveCallback), ((Socket)ar.AsyncState));
				if (OnConnectionStateChanged != null)
					OnConnectionStateChanged (this, true);
			} catch {
				clientSocket.BeginConnect (ipEndPoint, new AsyncCallback (ConnectCallback), clientSocket);
			}
		}

		private void ReceiveCallback (IAsyncResult ar)
		{
			try {
				int bytesReceived = ((Socket)ar.AsyncState).EndReceive (ar);
				if (bytesReceived != 0 && OnMessageReceived != null)
					OnMessageReceived (this, Encoding.ASCII.GetString (buffer, 0, bytesReceived));
				clientSocket.BeginReceive (buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback (ReceiveCallback), clientSocket);
			} catch (Exception ex) {
				Log.All (this, "message receive failed", MessageType.Error, ex);
			}
		}

		public void Send (string msg)
		{
			try {
				byte[] rawData = Encoding.ASCII.GetBytes (msg);
				if (rawData.Length < bufferSize) {
					clientSocket.BeginSend (rawData, 0, rawData.Length, SocketFlags.None, new AsyncCallback (SendingCallback), clientSocket);
				} else {
					Log.All (this, "the message you are trying to send is to long", MessageType.Error, new  ArgumentException ("the message you are trying to send is to long"));
				}
			} catch (Exception ex) {
				Log.All (this, "something went wrong while sending", MessageType.Error, ex);
			}
		}

		private void SendingCallback (IAsyncResult ar)
		{
			try {
				// returns the amount of bytes send
				int bytesSend = ((Socket)ar.AsyncState).EndSend (ar);
			} catch (Exception ex) {
				Log.All (this, "something went wrong while sending", MessageType.Error, ex);
			}
		}
	}
}

