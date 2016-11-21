using UnityEngine;

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

    TcpListener mActionListener, mImageListener;
    TcpClient mActionClient, mImageClient;

    NetworkStream mImageStream;

    UdpClient mInfoSender, mImageSender;
    IPEndPoint mInfoEndPoint, mImageEndPoint;

    int mActionInputPort;
    int mImagePort, mInfoPort;

    Func<string, int> mReceiveAction;

    public ReinforcementAgentTCPIface(string hostIp, int actionport, int imageport, int infoport, Func<string, int> ReceiveActionCallback)
    {
        mActionClientConnected = false;
        mImageClientConnected = false;
        mInfoClientConnected = true;

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
        imThread.IsBackground = true;
        imThread.Start();

        //mImageEndPoint = new IPEndPoint(IPAddress.Parse(mHostIP), mImagePort);
        mInfoEndPoint = new IPEndPoint(IPAddress.Parse(mHostIP), mInfoPort);

        //mImageSender = new UdpClient();
        mInfoSender = new UdpClient();

        //receiver
        mReceiveThread = new Thread(new ThreadStart(ReceiveData));
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

    public void SendImage(byte[] bytes)
    {
        try
        {
            //mImageSender.Send(bytes, bytes.Length, mImageEndPoint);
            mImageStream.Write(bytes, 0, bytes.Length);
        }
        catch (Exception err)
        {
            UnityEngine.Debug.LogError(err.ToString());
        }
    }

    public void SendInfo(float reward, int gamestatus)
    {
        JSONClass json = new JSONClass();
        json["reward"]["dtype"] = "float";
        json["reward"]["value"].AsFloat = reward;

        json["status"]["dtype"] = "int";
        json["status"]["value"].AsInt = gamestatus;

        SendJson(json);
    }

    public void SendInfo(float reward, int gamestatus, List<Observation> observations)
    {
        JSONClass json = new JSONClass();
        json["reward"]["dtype"] = "float";
        json["reward"]["value"].AsFloat = reward;

        json["status"]["dtype"] = "int";
        json["status"]["value"].AsInt = gamestatus;

        foreach(Observation o in observations)
        {
            json["observation"][o.name()]["dtype"] = o.datatype();
            json["observation"][o.name()]["value"].AsFloat = o.value();
        }

        SendJson(json);
    }

    public void SendJson(JSONClass json)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(json.ToString());
        mInfoSender.Send(bytes, bytes.Length, mInfoEndPoint);
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
    }

}
