using UnityEngine;
using System.Collections;
using ULE;

public class DiscreteMotorTemplate : DiscreteActionMotor {

	void Start () {
        mName = "Name";
        mNumActions = 3;

        // remember to init base class
        base.Init();
	}

    // Called when new action is received
    protected override void Act(int action)
    {
        switch (action)
        {
            case 0: Debug.Log("action 0"); break;
            case 1: Debug.Log("action 1"); break;
            case 2: Debug.Log("action 2"); break;
            default: break;
        }
    }
}
