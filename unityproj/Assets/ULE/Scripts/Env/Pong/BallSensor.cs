using UnityEngine;
using System.Collections;

public class BallSensor : VectorSensor {

	void Start () {
		mName = "BallSensor";
		mLength = 3;
		base.Init();
	}

	public override void Sample()
	{
		mVector[0] = transform.position.x;
		mVector[1] = transform.position.y;
		mVector[2] = transform.position.z;
	}
}
