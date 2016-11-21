using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ReinforcementAgent : MonoBehaviour {

    ReinforcementAgentTCPIface mIface;
    SceneManager mManager;

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

    void OnDestroy()
    {
        mIface.Close();
    }
}
