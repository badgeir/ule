using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

    enum GameStatus {StatusOK, GameOver};

    public ReinforcementAgent mAgent;
    public VirtualCamera mCamera;

    float mAccumulatedReward;
    GameStatus mGameStatus;

    private List<TickableObject> mActiveObjects;

    private Queue<string> mActionQueue;


    void Awake () {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mActiveObjects = new List<TickableObject>();

        mActionQueue = new Queue<string>();
    }

    void Update()
    {
        // check if new action has been added to queue
        if(mActionQueue.Count > 0)
        {
            mAgent.PerformActions(mActionQueue.Dequeue());

            TickAllTickableObjects();

            mAgent.SendReinforcementFeedback((int)mGameStatus);
        }
    }

    void TickAllTickableObjects()
    {
        foreach (TickableObject obj in mActiveObjects)
        {
            obj.Tick();
        }
    }

    public void QueueAction(string action)
    {
        mActionQueue.Enqueue(action);
    }

    string DequeueAction()
    {
        return mActionQueue.Dequeue();
    }

    public void AddActiveObject(TickableObject obj)
    {
        mActiveObjects.Add(obj);
    }

    public void GameOver()
    {

    }

}
