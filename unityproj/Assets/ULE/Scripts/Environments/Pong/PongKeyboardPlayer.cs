using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongKeyboardPlayer : MonoBehaviour {

    public float mSpeed;

    void Start()
    {
        mSpeed = 0.05f;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            if (transform.position.y < 1.43)
            {
                transform.Translate(Vector3.up * mSpeed);
            }
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            if (transform.position.y > -1.43)
            {
                transform.Translate(-Vector3.up * mSpeed);
            }
        }
    }
}
