using UnityEngine;
using System.Collections;
using ULE;

public class VectorSensorTemplate : VectorSensor
{

    void Start()
    {
        mName = "Name";
        mLength = 3;
        mMinVal = -100;
        mMaxVal = 100;

        //remember to initialize base class
        base.Init();
    }

    protected override void Sample()
    {
        mVector[0] = transform.position.x;
        mVector[1] = transform.position.y;
        mVector[2] = transform.position.z;
    }
}
