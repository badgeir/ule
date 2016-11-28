using UnityEngine;
using System.Collections;
using ULE;

public class VectorMotorTemplate : VectorMotor {

	void Start () {
        mName = "Name";

        mLength = 3;
        mMinVal = -1;
        mMaxVal = 1;
	}
	
    // Called when a new action is received
    protected override void Act(float[] action)
    {
        Debug.Log(action[0]);
        Debug.Log(action[1]);
        Debug.Log(action[2]);
    }
}
