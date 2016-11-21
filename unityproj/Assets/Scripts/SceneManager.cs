using UnityEngine;
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

    void Awake () {
        mGameStatus = GameStatus.StatusOK;
        mAccumulatedReward = 0;
        mActiveObjects = new List<TickableObject>();
        mObservations = new List<Observation>();
    }

    void Update()
    {
        if(mAgent.action_available())
        {

            mAgent.set_action_available(false);
            string actionstr = mAgent.GetActionString();

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
