
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
    Thread mReceiveThread;
    public string mLatestAction;

    string mHostIP = "127.0.0.1";

    TcpListener mListener;
    TcpClient mActionReceiver;

    UdpClient mCameraSender, mInfoSender;
    IPEndPoint mCameraEndPoint, mInfoEndPoint;

    int mActionInputPort;
    int mCameraPort, mInfoPort;

    Func<string, int> mReceiveAction;

    public ReinforcementAgentTCPIface(string hostIp, int actionport, int cameraport, int infoport, Func<string, int> ReceiveAction)
    {
        mHostIP = hostIp;
        mActionInputPort = actionport;
        mCameraPort = cameraport;
        mInfoPort = infoport;

        mReceiveAction = ReceiveAction;

        UdpInit();
    }

    private void UdpInit()
    {
        //sender
        mCameraEndPoint = new IPEndPoint(IPAddress.Parse(mHostIP), mCameraPort);
        mInfoEndPoint = new IPEndPoint(IPAddress.Parse(mHostIP), mInfoPort);

        mCameraSender = new UdpClient();
        mInfoSender = new UdpClient();

        //receiver
        mReceiveThread = new Thread(new ThreadStart(ReceiveData));
        mReceiveThread.IsBackground = true;
        mReceiveThread.Start();
    }

    public void SendImage(byte[] bytes)
    {
        try
        {
            mCameraSender.Send(bytes, bytes.Length, mCameraEndPoint);
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
        mListener = new TcpListener(address, mActionInputPort);
        mListener.Start();

        mActionReceiver = mListener.AcceptTcpClient();

        NetworkStream nwStream = mActionReceiver.GetStream();
        byte[] buffer = new byte[mActionReceiver.ReceiveBufferSize];

        while (true)
        {

            try
            {
                int bytesRead = nwStream.Read(buffer, 0, mActionReceiver.ReceiveBufferSize);
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
    }

}
