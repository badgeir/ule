﻿using UnityEngine;
using System.Collections;

public class Ball : ActiveObject {

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

	// Update is called once per frame
	public override void Tick()
    {
        transform.Translate((mBallDirection * mHorizontalSpeed + Vector3.up*mVerticalSpeed));
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collision!");
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
