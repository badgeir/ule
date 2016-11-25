using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULE;

namespace UleComm
{
	class Program
	{
		static void Main(string[] args)
		{
			Test test = new Test();
			test.Start();
			while (true) ;
		}

		class Test
		{
			ULEServer mServer;

			public void Start()
			{
				mServer = new ULEServer("127.0.0.1", 3000);
				mServer.Connect(OnClientConnected);
			}

			public void OnClientConnected()
			{
				Console.WriteLine("Client connected.");
				mServer.StartReceiving(OnClientRequest, OnClientDisconnected);
			}

			public void OnClientDisconnected()
			{
				Console.WriteLine("Client disconnected.");
				mServer.Connect(OnClientConnected);
			}

			public void OnClientRequest(string msg)
			{
				Console.WriteLine(msg);
				mServer.SendResponse(msg);
			}
		}
	}
}
