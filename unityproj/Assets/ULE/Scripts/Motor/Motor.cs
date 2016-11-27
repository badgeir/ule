using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Motor : TickableObject {

    new public virtual string name()
    {
        return "";
    }

    public virtual bool PushJson(JSONNode json)
    {
        return false;
    }

    public virtual JSONNode JsonDescription()
    {
        return null;
    }

    protected virtual void Act(int action)
    {

    }
    
    protected virtual void Act(float[] action)
    {

    }

}
