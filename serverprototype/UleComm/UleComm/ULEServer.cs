using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using SimpleJSON;


namespace ULE
{
	class ULEServer
	{
		string mHostIP;
		int mPort;

		TcpListener mListener;
		TcpClient mClient;
		NetworkStream mStream;

		Action<string> mReceiveCallback;

		public ULEServer(string hostIp, int port)
		{
			mHostIP = hostIp;
			mPort = port;

			mListener = new TcpListener(IPAddress.Parse(mHostIP), mPort);
		}

		public void Connect(Action onConnected)
		{
			Thread conn = new Thread(new ParameterizedThreadStart(InitConnection));
			conn.Start(onConnected);
		}

		void InitConnection(object onConnected)
		{
			mListener.Start();
			mClient = mListener.AcceptTcpClient();
			mStream = mClient.GetStream();

			Action callback = (Action)onConnected ;
			callback();
		}

		public void StartReceiving(Action<string> ReceiveCallback, Action onDisconnect)
		{
			mReceiveCallback = ReceiveCallback;

			Thread receive = new Thread(new ParameterizedThreadStart(Receiver));
			receive.IsBackground = true;
			receive.Start(onDisconnect);
		}


		void Receiver(object onDisconnect)
		{
			byte[] buffer = new byte[mClient.ReceiveBufferSize];

			while (mClient.Connected)
			{
				try
				{
					int bytesRead = mStream.Read(buffer, 0, mClient.ReceiveBufferSize);
					if (bytesRead > 0)
					{
						string message = Encoding.UTF8.GetString(buffer);
						mReceiveCallback(message);
					}
				}
				catch (Exception err)
				{
					Console.WriteLine("ULEServer.Receiver(): " + err.ToString());
				}

				Thread.Sleep(10);
			}
			Action disconnected = (Action)onDisconnect;
			disconnected();
		}

		public void SendResponse(string msg)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(msg);
			Console.WriteLine(bytes.Length);
			mStream.Write(bytes, 0, bytes.Length);
		}

	}
}
