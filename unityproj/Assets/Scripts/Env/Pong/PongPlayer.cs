using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PongPlayer : DiscreteActionMotor {

    public float mSpeed;

    void Start()
    {
        mName = "PongPlayer";
        mNumActions = 3;
        mSpeed = 0.05f;
        base.Init();
    }
    
    protected override void Act(int action)
    {
        switch(action)
        {
            case 0: break;
            case 1: transform.Translate(Vector3.up*mSpeed); break;
            case 2: transform.Translate(-Vector3.up * mSpeed); break;
            default: break;
        }
    }
}
