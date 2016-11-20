using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

    enum GameStatus {StatusOK, GameOver};

    public ReinforcementAgent mAgent;
    DepthCamera mCamera;

    float mAccumulatedReward;
    GameStatus mGameStatus;

    void Start () {
        mCamera = GameObject.Find("VirtualKinect").GetComponent<DepthCamera>();

        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;
    }

    void Update()
    {
        if(mAgent.action_available())
        {
            mAgent.set_action_available(false);

            string actionstr = mAgent.GetActionString();
            float action = float.Parse(actionstr);

            byte[] imagebytes = mCamera.GetImageBytes();
            mAgent.SendReinforcementFeedback(imagebytes, mAccumulatedReward, (int)mGameStatus);
        }
    }

    public void GameOver()
    {

    }

}
