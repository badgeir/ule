using UnityEngine;
using System.Collections;

public class PongOpponent : MonoBehaviour {

    public Transform mBall;
    
    void FixedUpdate () {
        float targetY = mBall.position.y + UnityEngine.Random.Range(-0.3f, 0.3f);
        if (targetY > 1.43)
        {
            targetY = 1.43f; 
        }
        else if(targetY < -1.43)
        {
            targetY = -1.43f;
        }
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetY, 0.2f), transform.position.z);
    }
}
