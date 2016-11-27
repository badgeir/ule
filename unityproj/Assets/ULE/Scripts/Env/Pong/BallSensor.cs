using UnityEngine;
using System.Collections;

public class BallSensor : VectorSensor {

	void Start () {
		base.Init();
	}

	protected override void Sample()
	{
		mVector[0] = transform.position.x;
		mVector[1] = transform.position.y;
		mVector[2] = transform.position.z;
	}
}
