﻿using UnityEngine;
using System;
using System.Collections;

public class Ball : MonoBehaviour
{
    private float mHorizontalSpeed;
    private float mVerticalSpeed;

    void Start()
    {
        mHorizontalSpeed = 0.01f;
        mVerticalSpeed = 0.01f;
    }

	// Ticked on physics update, regardsless of discrete or continuous mode
	void FixedUpdate()
    {
        transform.Translate((Vector3.right * mHorizontalSpeed + Vector3.up*mVerticalSpeed));
	}

    void OnTriggerEnter(Collider col)
    {
        switch(col.tag)
        {
            case "Player":
                {
                    ReinforcementAgent agent = GameObject.Find("Agent").GetComponent<ReinforcementAgent>();
                    agent.AddReward(1);
                    mHorizontalSpeed = -mHorizontalSpeed;

                    //calculate vertical speed:
                    float diff = transform.position.y - col.transform.position.y;
                    mVerticalSpeed += diff/2;
                    if(mVerticalSpeed > 0.025)
                    {
                        mVerticalSpeed = 0.025f;
                    }
                    else if(mVerticalSpeed < -0.025)
                    {
                        mVerticalSpeed = -0.025f;
                    }

                    break;
                }
            case "Opponent":
                {
                    mHorizontalSpeed = -mHorizontalSpeed;

                    //calculate vertical speed:
                    float diff = transform.position.y - col.transform.position.y;
                    mVerticalSpeed += diff / 2;
                    if (mVerticalSpeed > 0.025)
                    {
                        mVerticalSpeed = 0.025f;
                    }
                    else if (mVerticalSpeed < -0.025)
                    {
                        mVerticalSpeed = -0.025f;
                    }
                    break;
                }
            case "Edge":
                mVerticalSpeed = -mVerticalSpeed;
                break;
            case "Back":
                {
                    ReinforcementAgent agent = GameObject.Find("Agent").GetComponent<ReinforcementAgent>();
                    agent.AddReward(-1);
                    agent.GameOver();
                    break;
                }
            case "Front":
                {
                    ReinforcementAgent agent = GameObject.Find("Agent").GetComponent<ReinforcementAgent>();
                    agent.AddReward(1);
                    agent.GameOver();
                    break;
                }
            default: break;
        }

    }
}