﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ReinforcementAgent : MonoBehaviour {

    ReinforcementAgentTCPIface mIface;

    private static bool mActionAvailable;
    private string mAction;

	// Use this for initialization
    void Start()
    {
        mActionAvailable = false;
        mAction = "";

        mIface = new ReinforcementAgentTCPIface("127.0.0.1", 3000, 3001, 3002, ReceiveAction);
        StartCoroutine(ConnectToAgent());
    }

    IEnumerator ConnectToAgent()
    {
        mIface.Connect();
        while(!mIface.Connected())
        {
            yield return new WaitForSeconds(0.1f);
        }
        // oneway handshake, tell client it's ok to start sending actions
        mIface.SendInfo(0, 0);
    }

    public void SendReinforcementFeedback(byte[] image, float reward, int gamestatus)
    {
        mIface.SendImage(image);
        mIface.SendInfo(reward, gamestatus);
    }

    public void SendReinforcementFeedback(byte[] image, float reward, int gamestatus, List<Observation> observations)
    {
        mIface.SendImage(image);
        mIface.SendInfo(reward, gamestatus, observations);
    }

    public bool action_available()
    {
        return mActionAvailable;
    }

    public void set_action_available(bool val)
    {
        mActionAvailable = val;
    }

    public string GetActionString()
    {
        return mAction;
    }

    // called when AgentIface receives an action
    public int ReceiveAction(string msg)
    {
        mActionAvailable = true;
        mAction = msg;
        return 0;
    }

    void OnDestroy()
    {
        mIface.Close();
    }
}
