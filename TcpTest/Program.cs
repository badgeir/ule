using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TcpTest
{
	class Program
	{
		const int PORT_NO = 5000;
		const string SERVER_IP = "127.0.0.1";

		static void Main(string[] args)
		{
			//---listen at the specified IP and port no.---
			IPAddress localAdd = IPAddress.Parse(SERVER_IP);
			TcpListener listener = new TcpListener(localAdd, PORT_NO);
			Console.WriteLine("Listening...");
			listener.Start();

			//---incoming client connected---
			TcpClient client = listener.AcceptTcpClient();

			NetworkStream nwStream = client.GetStream();
			byte[] buffer = new byte[client.ReceiveBufferSize];

			//---read incoming stream---
			for (int i = 0; i < 4; i++)
			{
				int bytesRead;
				do
				{
					bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
				} while (bytesRead <= 0);

				//---convert the data received into a string---
				string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
				Console.WriteLine("Received : " + dataReceived);

				//---write back the text to the client---
				Console.WriteLine("Sending back : " + dataReceived);
				nwStream.Write(buffer, 0, bytesRead);
			}
			client.Close();
			listener.Stop();
			while (true) ;
		}
	}
}
