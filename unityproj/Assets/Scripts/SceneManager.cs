using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

    enum GameStatus {StatusOK, GameOver};

    public ReinforcementAgent mAgent;
    public VirtualCamera mCamera;

    float mAccumulatedReward;
    GameStatus mGameStatus;

    private List<ActiveObject> mActiveObjects;

    void Awake () {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;
        mActiveObjects = new List<ActiveObject>();
    }

    void Update()
    {
        if(mAgent.action_available())
        {

            mAgent.set_action_available(false);
            string actionstr = mAgent.GetActionString();

            TickAll();

            byte[] imagebytes = mCamera.GetImageBytes();
            mAgent.SendReinforcementFeedback(imagebytes, mAccumulatedReward, (int)mGameStatus);
        }
    }

    void TickAll()
    {
        foreach(ActiveObject obj in mActiveObjects)
        {
            obj.Tick();
        }
    }

    public void AddActiveObject(ActiveObject obj)
    {
        mActiveObjects.Add(obj);
    }

    public void GameOver()
    {

    }

}
