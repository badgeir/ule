using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

    enum GameStatus {StatusOK, GameOver};

    public ReinforcementAgent mAgent;
    public VirtualCamera mCamera;

    float mAccumulatedReward;
    GameStatus mGameStatus;

    void Start () {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;
    }

    void Update()
    {
        if(mAgent.action_available())
        {
            mAgent.set_action_available(false);

            string actionstr = mAgent.GetActionString();

            byte[] imagebytes = mCamera.GetImageBytes();
            Debug.Log("send");
            mAgent.SendReinforcementFeedback(imagebytes, mAccumulatedReward, (int)mGameStatus);
        }
    }

    public void GameOver()
    {

    }

}
