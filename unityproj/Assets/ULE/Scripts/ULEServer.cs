using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        Thread mReceiveThread;

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

            mReceiveThread = new Thread(new ParameterizedThreadStart(Receiver));
            mReceiveThread.IsBackground = true;
            mReceiveThread.Start(onDisconnect);
		}

        //Receiver thread function
		void Receiver(object onDisconnect)
		{
			byte[] buffer = new byte[mClient.ReceiveBufferSize];

			while (IsConnected)
			{
				try
				{
					int bytesRead = mStream.Read(buffer, 0, mClient.ReceiveBufferSize);
					if (bytesRead > 0)
					{
						string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
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

        public void SendJson(JSONClass json)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string s = json.Value;

            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            UnityEngine.Debug.Log(elapsed);
 
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            mStream.Write(bytes, 0, bytes.Length);
        }

        public void SendResponse(string msg)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(msg);
			mStream.Write(bytes, 0, bytes.Length);
		}

        bool IsConnected
        {
            get
            {
                try
                {
                    if (mClient != null && mClient.Client != null && mClient.Client.Connected)
                    {
                        // Detect if client disconnected
                        if (mClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (mClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                            {
                                // Client disconnected
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Close()
        {
            mReceiveThread.Abort();
            mReceiveThread.Join();
        }
	}
}
