using UnityEngine;
using System.Collections;

public class ReinforcementAgent : MonoBehaviour {

    ReinforcementAgentInterface mIface;

    private static bool mActionAvailable;
    private string mAction;

	// Use this for initialization
	void Start () {
        mActionAvailable = false;
        mAction = "";

        mIface = new ReinforcementAgentInterface("127.0.0.1", 3000, 3001, 3002, 3003, ReceiveAction);
	}

    public void SendReinforcementFeedback(byte[] image, float reward, int gamestatus)
    {
        mIface.SendImage(image);
        mIface.SendReward(reward);
        mIface.SendGameStatus(gamestatus);
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
