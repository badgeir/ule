using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Space {
    
    public virtual bool Contains(object o)
    {
        return false;
    }

    public virtual JSONNode ToJSON()
    {
        return null;
    }

    public virtual Space FromJSON(JSONNode json)
    {
        return null;
    }
    
}
