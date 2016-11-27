using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;

using ULE;

public class SceneManager : MonoBehaviour {

    enum GameStatus {StatusOK, GameOver};
    GameStatus mGameStatus;

    float mReward = 0;

    public ReinforcementAgent mAgent;

    float mAccumulatedReward;
    private List<TickableObject> mActiveObjects;

    private Queue<string> mPendingMessages;

    ULEServer mServer;
    UleRPC mUleRPC;

    bool mMotorsUpdated;

    void Awake () {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mActiveObjects = new List<TickableObject>();

        mUleRPC = new UleRPC(OnGetEnvironment, OnUpdateMotor);

        mServer = new ULEServer("127.0.0.1", 3000);
        mServer.Connect(OnClientConnected);

        mPendingMessages = new Queue<string>();

        mMotorsUpdated = false;
    }

    //main loop, called each frame
    void Update()
    {
        if (mPendingMessages.Count > 0)
        {

            mUleRPC.ParseInput(mPendingMessages.Dequeue());
            if (mMotorsUpdated)
            {
                //Update all movable objects
                Tick();

                SendFeedback();

                mMotorsUpdated = false;
            }

        }
    }

    public void OnClientConnected()
    {
        Debug.Log("Client connected.");
        mServer.StartReceiving(OnClientRequest, OnClientDisconnected);
    }

    public void OnClientDisconnected()
    {
        Debug.Log("Client disconnected.");
        mServer.Connect(OnClientConnected);
    }

    public void OnClientRequest(string msg)
    {
        mPendingMessages.Enqueue(msg);
    }

    public void OnGetEnvironment()
    {
        JSONClass json = new JSONClass();

        int idx = 0;
        foreach (Sensor sens in mAgent.mSensors)
        {
            json["sensors"][idx++] = sens.JsonDescription();
        }

        idx = 0;
        foreach (Motor motor in mAgent.mMotors)
        {
            json["motors"][idx++] = motor.JsonDescription();
        }

        mServer.SendJson(json);
    }

    public void OnUpdateMotor(JSONNode motor)
    {
        mMotorsUpdated = true;
        mAgent.UpdateMotor(motor);
    }

    public void SendFeedback()
    {
        // Optimization. JSONNode.ToString() used 160 ms when parsing image. By writing out the json string
        // "by hand", I got it down to 2 ms.
        string jsonstring = "{{\"sensors\": [{0}], \"reward\": \"{1}\", \"done\": \"{2}\", \"info\": {3}}}";
        string sensors = "";
        int idx = 0;
        foreach (Sensor sens in mAgent.mSensors)
        {   
            if (idx++ != 0)
            {
                sensors += ", ";
            }
            sensors += sens.SampleJson();
        }

        string output = string.Format(jsonstring, sensors, mReward, (int)mGameStatus, "\"\"");
        mServer.SendResponse(output);
    }

    void Tick()
    {
        foreach (TickableObject obj in mActiveObjects)
        {
            obj.Tick();
        }
    }

    public void AddActiveObject(TickableObject obj)
    {
        mActiveObjects.Add(obj);
    }

    public void GameOver()
    {

    }

    public void OnDestroy()
    {
        mServer.Close();
    }

}
