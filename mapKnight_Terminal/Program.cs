using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace server
{
    class Program
    {
        private const int bufferSize = 1024;
        private const int port = 1337;

        private static byte[] buffer = new byte[bufferSize];

        private static IPEndPoint ipEndPoint;
        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static bool connected = false;

        private static string input;

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            Console.WriteLine("mapKnight Development Terminal");
            Console.WriteLine("Version = " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("");
            Console.WriteLine("");

            Begin ();

            while ((input = Console.ReadLine().ToLower()) != "exit")
            {
                Send(input);
                Console.Write("> ");
            }

            Send ("__:disconnect:__");
            Console.Write("exiting .");
            Thread.Sleep(450);
            Console.Write(".");
            Thread.Sleep(450);
            Console.Write(".");
            Thread.Sleep(450);
        }

        static void Begin()
        {
            try
            {
                Console.Write("mobilephone-ip = ");
                ipEndPoint = new IPEndPoint(IPAddress.Parse(Console.ReadLine()), port);

                Console.WriteLine("");
                Console.WriteLine("init-log :");
                Console.Write("! connecting to " + ipEndPoint.ToString() + " ");
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), clientSocket);
                while (!connected)
                {
                    Spin();
                }
                Console.WriteLine("");
                Console.WriteLine("! connected");
                Console.WriteLine("");
                Console.Write("> ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static long counter;

        static void Spin()
        {
            counter++;
            if (counter % 800 < 200)
            {
                Console.Write("/");
            }
            else if (counter % 800 < 400)
            {
                Console.Write("-");
            }
            else if (counter % 800 < 600)
            {
                Console.Write("\\");
            }
            else
            {
                Console.Write("|");
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }

        static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // casts the object parsed to the .BeginConnect method and calls the method to complete the connection-phase
                if (((Socket)ar.AsyncState).Connected)
                {
                    ((Socket)ar.AsyncState).EndConnect(ar);
                    ((Socket)ar.AsyncState).BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), ((Socket)ar.AsyncState));
                    connected = true;
                }
                else
                {
                    clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), clientSocket);
                }
            }
            catch
            {
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), clientSocket);
            }
        }

        static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesReceived = ((Socket)ar.AsyncState).EndReceive(ar);
                if (bytesReceived == 0)
                    return; //client diconnected
                string messageReceived = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                if (messageReceived == "__:disconnect:__")
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine("! server disconnected");
                    Console.Write("! trying to reconnect ");

                    connected = false;
                    clientSocket.Close();
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), clientSocket);
                    while (!connected)
                    {
                        Spin();
                    }
                    Console.WriteLine("");
                    Console.WriteLine("! connected");
                    Console.WriteLine("");
                    Console.Write("> ");
                }
                else
                {
                    Console.WriteLine("< " + messageReceived);
                    clientSocket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Send(string msg)
        {
            if (connected)
            {
                try
                {
                    byte[] rawData = Encoding.ASCII.GetBytes(msg);
                    if (rawData.Length < bufferSize)
                    {
                        clientSocket.BeginSend(rawData, 0, rawData.Length, SocketFlags.None, new AsyncCallback(SendingCallback), clientSocket);
                    }
                    else
                    {
                        Console.WriteLine("! message is to long");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static void SendingCallback(IAsyncResult ar)
        {
            try
            {
                // returns the amount of bytes send
                int bytesSend = ((Socket)ar.AsyncState).EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            Send("__:disconnect:__");
            // Put your own handler here
            return true;
        }
    }
}
