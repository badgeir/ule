
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class ReinforcementAgentInterface
{

    Thread mReceiveThread;
    public string mLatestAction;

    UdpClient mImageSender, mRewardSender, mGameOverSender; 
    UdpClient mActionReceiver;
    IPEndPoint mCameraEndPoint, mRewardEndPoint, mGameStatusEndPoint;
    string mHostIP = "127.0.0.1";

    int mActionInputPort;
    int mCameraPort, mRewardPort, mGameStatusPort;

    Func<string, int> mReceiveAction;

    public ReinforcementAgentInterface(string hostIp, int actionport, int cameraport, int rewardport, int gamestatusport, Func<string, int> ReceiveAction)
    {
        mHostIP = hostIp;
        mActionInputPort = actionport;
        mCameraPort = cameraport;
        mGameStatusPort = gamestatusport;
        mRewardPort = rewardport;

        mReceiveAction = ReceiveAction;

        UdpInit();
    }

    private void UdpInit()
    {
        //sender
        mCameraEndPoint = new IPEndPoint(IPAddress.Parse(mHostIP), mCameraPort);
        mRewardEndPoint = new IPEndPoint(IPAddress.Parse(mHostIP), mRewardPort);
        mGameStatusEndPoint = new IPEndPoint(IPAddress.Parse(mHostIP), mGameStatusPort);

        mImageSender = new UdpClient();
        mRewardSender = new UdpClient();
        mGameOverSender = new UdpClient();

        //receiver
        mReceiveThread = new Thread(new ThreadStart(ReceiveData));
        mReceiveThread.IsBackground = true;
        mReceiveThread.Start();
    }

    public void SendImage(byte[] bytes)
    {
        try
        {
            mImageSender.Send(bytes, bytes.Length, mCameraEndPoint);
        }
        catch (Exception err)
        {
            UnityEngine.Debug.LogError(err.ToString());
        }
    }

    public void SendReward(float reward)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(reward.ToString());
            mRewardSender.Send(bytes, bytes.Length, mRewardEndPoint);
        }
        catch (Exception err)
        {
            UnityEngine.Debug.LogError(err.ToString());
        }
    }

    public void SendGameStatus(int status)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(status.ToString());
            mRewardSender.Send(bytes, bytes.Length, mGameStatusEndPoint);
        }
        catch (Exception err)
        {
            UnityEngine.Debug.LogError(err.ToString());
        }
    }

    private void ReceiveData()
    {
        mActionReceiver = new UdpClient(mActionInputPort);
        mActionReceiver.Client.ReceiveTimeout = 500;
        while (true)
        {

            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = null;
                data = mActionReceiver.Receive(ref anyIP);

                if (data != null)
                {
                    mLatestAction = Encoding.UTF8.GetString(data);
                    int a = mReceiveAction(mLatestAction);
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
