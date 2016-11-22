using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PongPlayer : DiscreteActionMotor {

    void Start()
    {
        mName = "PongPlayer";
        mNumActions = 3;

        base.Init();
    }
    
    protected override void Act(int action)
    {
        switch(action)
        {
            case 0: Debug.Log(0); break;
            case 1: Debug.Log(1); break;
            case 2: Debug.Log(2); break;
            default: break;
        }
    }
}
