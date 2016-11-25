using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using SimpleJSON;

public class ReinforcementAgentTCPIface
{
	private bool mActionClientConnected, mImageClientConnected, mInfoClientConnected;

	Thread mReceiveThread;
	public string mLatestAction;

	string mHostIP;

	TcpListener mActionListener, mImageListener, mInfoListener;
	TcpClient mActionClient, mImageClient, mInfoClient;

	NetworkStream mImageStream, mInfoStream;

	int mActionInputPort;
	int mImagePort, mInfoPort;

	Action<string> mReceiveAction;

	public ReinforcementAgentTCPIface(string hostIp, int actionport, int imageport, int infoport, Action<string> ReceiveActionCallback)
	{
		mActionClientConnected = false;
		mImageClientConnected = false;
		mInfoClientConnected = false;

		mHostIP = hostIp;
		mActionInputPort = actionport;
		mImagePort = imageport;
		mInfoPort = infoport;

		mReceiveAction = ReceiveActionCallback;
	}

	public void Connect()
	{
		TcpInit();
	}

	public bool Connected()
	{
		return (mActionClientConnected && mImageClientConnected && mInfoClientConnected);
	}

	void TcpInit()
	{

		Thread imThread = new Thread(new ThreadStart(InitImageConnection));
		imThread.Start();

		Thread infoThread = new Thread(new ThreadStart(InitInfoConnection));
		infoThread.Start();

		mReceiveThread = new Thread(new ThreadStart(ReceiveData));
		mReceiveThread.IsBackground = true;
		mReceiveThread.Start();
	}

	private void InitImageConnection()
	{
		mImageListener = new TcpListener(IPAddress.Parse(mHostIP), mImagePort);
		mImageListener.Start();
		mImageClient = mImageListener.AcceptTcpClient();
		mImageStream = mImageClient.GetStream();
		mImageClientConnected = true;
	}

	private void InitInfoConnection()
	{
		mInfoListener = new TcpListener(IPAddress.Parse(mHostIP), mInfoPort);
		mInfoListener.Start();
		mInfoClient = mInfoListener.AcceptTcpClient();
		mInfoStream = mInfoClient.GetStream();
		mInfoClientConnected = true;
	}

	public void SendImage(byte[] bytes)
	{
		try
		{
			mImageStream.Write(bytes, 0, bytes.Length);
		}
		catch (Exception err)
		{
			UnityEngine.Debug.LogError(err.ToString());
		}
	}

	public void SendEnvironmentInfo(List<Sensor> sensors, List<Motor> motors)
	{
		JSONClass json = new JSONClass();

        int idx = 0;
		foreach (Sensor sens in sensors)
		{
			json["sensors"][idx] = sens.JsonDescription();
		}

        idx = 0;
		foreach (Motor motor in motors)
		{
            json["motors"][idx] = motor.JsonDescription();
		}

		SendJson(json);
	}

	public void SendInfo(float reward, int gamestatus, List<Sensor> sensors, List<string> info = null)
	{
		JSONClass json = new JSONClass();

		foreach (Sensor sens in sensors)
		{
			sens.Sample();
			json.Add("sensors", sens.SampleJson());
		}
		json["reward"].AsFloat = reward;
		json["done"].AsInt = gamestatus;

		if (info == null)
		{
			json["info"] = "";
		}
		else
		{
			foreach(string msg in info)
			{
				json["info"][0] = msg;
			}
		}

		SendJson(json);
	}

	public void SendJson(JSONClass json)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(json.ToString());
		mInfoStream.Write(bytes, 0, bytes.Length);
	}

	private void ReceiveData()
	{
		IPAddress address = IPAddress.Parse(mHostIP);
		mActionListener = new TcpListener(address, mActionInputPort);
		mActionListener.Start();

		mActionClient = mActionListener.AcceptTcpClient();

		NetworkStream nwStream = mActionClient.GetStream();
		mActionClientConnected = true;

		byte[] buffer = new byte[mActionClient.ReceiveBufferSize];

		while (true)
		{

			try
			{
				int bytesRead = nwStream.Read(buffer, 0, mActionClient.ReceiveBufferSize);
				if(bytesRead > 0)
				{
					mLatestAction = Encoding.UTF8.GetString(buffer);
					mReceiveAction(mLatestAction);
				}
			}
			catch (Exception err)
			{
				//UnityEngine.Debug.Log(err.ToString());
			}

			Thread.Sleep(10);
		}
	}

	public void Close()
	{
		mReceiveThread.Abort();
		mReceiveThread.Join();

		mActionClient.Close();
		mActionListener.Stop();

		mImageClient.Close();
		mImageListener.Stop();

		mInfoClient.Close();
		mInfoListener.Stop();
	}

}
