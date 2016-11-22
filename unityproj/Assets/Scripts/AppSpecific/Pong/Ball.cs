using UnityEngine;
using System;
using System.Collections;

public class Ball : TickableObject
{
    private float mHorizontalSpeed;
    private float mVerticalSpeed;
    private Vector3 mBallDirection;

    void Start()
    {
        mHorizontalSpeed = 0.01f;
        mVerticalSpeed = 0;
        mBallDirection = Vector3.left;

        GameObject.Find("SceneManager").GetComponent<SceneManager>().AddActiveObject(this);

    }

	// Ticked by SceneManager on each action received
	public override void Tick()
    {
        transform.Translate((mBallDirection * mHorizontalSpeed + Vector3.up*mVerticalSpeed));
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Paddle")
        {
            if(mBallDirection == Vector3.left)
            {
                mBallDirection = Vector3.right;
            }
            else 
            {
                mBallDirection = Vector3.left;
            }
        }
    }
}
