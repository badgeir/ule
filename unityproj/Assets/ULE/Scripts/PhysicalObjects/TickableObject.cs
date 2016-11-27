using UnityEngine;
using System.Collections;

public class TickableObject : MonoBehaviour {


    void Start()
    {
        Init();
    }

    protected void Init()
    {
        GameObject.Find("Agent").GetComponent<ReinforcementAgent>().AddTickableObject(this);
    }

    public virtual void Tick()
    {

    }

    void FixedUpdate()
    {
        Tick();
    }
}
