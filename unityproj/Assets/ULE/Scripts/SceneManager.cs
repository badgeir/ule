﻿using UnityEngine;
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
    UleParser mParser;

    bool mMotorsUpdated;

    void Awake () {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mActiveObjects = new List<TickableObject>();

        mParser = new UleParser(OnGetEnvironment, OnUpdateMotor);

        mServer = new ULEServer("127.0.0.1", 3000);
        mServer.Connect(OnClientConnected);

        mPendingMessages = new Queue<string>();

        mMotorsUpdated = false;
    }

    void Update()
    {
        if (mPendingMessages.Count > 0)
        {

            mParser.ParseInput(mPendingMessages.Dequeue());
            if (mMotorsUpdated)
            {
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
        JSONClass json = new JSONClass();

        int idx = 0;
        foreach (Sensor sens in mAgent.mSensors)
        {
            json["sensors"][idx++] = sens.SampleJson();
        }

        json["reward"].AsFloat = mReward;
        json["done"].AsInt = (int)mGameStatus;

        json["info"] = "";

        mServer.SendJson(json);
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