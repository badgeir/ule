using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ReinforcementAgent : MonoBehaviour {

    ReinforcementAgentTCPIface mIface;
    SceneManager mManager;

    public VirtualCamera mCamera;
    public List<Sensor> mSensors;
    public List<Motor> mMotors;

    private float mReward;

	// Use this for initialization
    void Start()
    {
        mManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();

        mIface = new ReinforcementAgentTCPIface("127.0.0.1", 3000, 3001, 3002, mManager.QueueAction);
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
        mIface.SendInfo(0, 0, mSensors);
    }

    public void PerformActions(string actions)
    {
        Debug.Log(actions);
    }

    public void SendReinforcementFeedback(int gamestatus)
    {
        byte[] image = mCamera.GetImageBytes();
        mIface.SendImage(image);
        mIface.SendInfo(mReward, gamestatus, mSensors);
        mReward = 0;
    }

    public void AddReward(float amount)
    {
        mReward += amount;
    }

    void OnDestroy()
    {
        mIface.Close();
    }
}
