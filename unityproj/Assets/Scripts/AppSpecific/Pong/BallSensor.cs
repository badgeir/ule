using UnityEngine;
using System.Collections;

public class BallSensor : FloatSensor {

	void Start () {
        mName = "BallSensor";
        mValue = 0;
        base.Init();
	}

    public override void Sample()
    {
        mValue = transform.position.x;
    }
}
