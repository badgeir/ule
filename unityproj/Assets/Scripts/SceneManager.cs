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
    private List<Observation> mObservations;

    private Queue<string> mActionQueue;


    void Awake () {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;

        mActiveObjects = new List<TickableObject>();
        mObservations = new List<Observation>();

        mActionQueue = new Queue<string>();
    }

    void Update()
    {
        // check if new action has been added to queue
        if(mActionQueue.Count > 0)
        {
            Debug.Log(mActionQueue.Dequeue());

            TickAllTickableObjects();

            byte[] imagebytes = mCamera.GetImageBytes();
            mAgent.SendReinforcementFeedback(imagebytes, mAccumulatedReward, (int)mGameStatus, mObservations);
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

    public void AddObservation(Observation obs)
    {
        mObservations.Add(obs);
    }

    public void GameOver()
    {

    }

}
