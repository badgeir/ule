using UnityEngine;
using System;
using System.Collections;

public class Ball : MonoBehaviour
{
    private float mHorizontalSpeed;
    private float mVerticalSpeed;

    ReinforcementAgent mAgent;

    void Start()
    {
        mAgent = GameObject.Find("Agent").GetComponent<ReinforcementAgent>();

        int p = UnityEngine.Random.Range(0, 2);
        if(p == 0)
        {
            mHorizontalSpeed = 0.09f;
        }
        else
        {
            mHorizontalSpeed = -0.09f;
        }

        mVerticalSpeed = UnityEngine.Random.Range(-0.02f, 0.02f);
    }

	// Ticked on physics update, regardsless of discrete or continuous mode
	void FixedUpdate()
    {
        transform.Translate((Vector3.right * mHorizontalSpeed + Vector3.up*mVerticalSpeed));
        if (transform.position.y < -2 || transform.position.y > 2)
        {
            mAgent.GameOver();
        }
	}

    void OnTriggerEnter(Collider col)
    {
        switch(col.tag)
        {
            case "Player":
            case "Opponent":
                {
                    mHorizontalSpeed = -mHorizontalSpeed;

                    //calculate vertical speed:
                    float diff = transform.position.y - col.transform.position.y;
					mVerticalSpeed += diff / 2;
					if (mVerticalSpeed > 0.12)
					{
						mVerticalSpeed = 0.12f;
					}
					else if (mVerticalSpeed < -0.12)
					{
						mVerticalSpeed = -0.12f;
					}
					break;
                }
            case "Edge":
                mVerticalSpeed = -mVerticalSpeed;
                break;
            case "Back":
                {
                    mAgent.AddReward(-1);
                    mAgent.GameOver();
                    break;
                }
            case "Front":
                {
                    mAgent.AddReward(1);
                    mAgent.GameOver();
                    break;
                }
            default: break;
        }

    }
}
