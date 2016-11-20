using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    private float mBallSpeed;

    void Start()
    {
        mBallSpeed = 0.3f;
    }

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.left * mBallSpeed * Time.deltaTime);
	}
}
