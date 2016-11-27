using UnityEngine;
using System.Collections;

public class PongOpponent : MonoBehaviour {

    public Transform mBall;
	
	void FixedUpdate () {
        transform.position = new Vector3(transform.position.x, Vector3.Lerp(transform.position, mBall.position,0.1f).y, transform.position.z);
	}
}
