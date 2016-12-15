using UnityEngine;
using System.Collections;

public class PongOpponent : MonoBehaviour {

    public Transform mBall;
	
	void FixedUpdate () {
        float ballY = mBall.position.y;
        if(ballY > 1.43)
        {
            ballY = 1.43f; 
        }
        else if(ballY < -1.43)
        {
            ballY = -1.43f;
        }
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, ballY, 0.2f), transform.position.z);
	}
}
