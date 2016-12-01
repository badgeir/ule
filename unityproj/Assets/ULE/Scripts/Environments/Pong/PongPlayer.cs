using UnityEngine;
using System.Collections;
using ULE;

public class PongPlayer : DiscreteActionMotor {

    public float mSpeed;

    void Start()
    {
        mNumActions = 3;
        base.Init();
    }
    
    protected override void Act(int action)
    {
        switch(action)
        {
            case 0: break;
            case 1:
            {
                if (transform.position.y < 0.47)
                { 
                    transform.Translate(Vector3.up * mSpeed); 
                }
                break;
            }
            case 2:
            { 
                if(transform.position.y > -0.47)
                {
                    transform.Translate(-Vector3.up * mSpeed); 
                }
                break;
            } 
            default: break;
        }
    }
}
