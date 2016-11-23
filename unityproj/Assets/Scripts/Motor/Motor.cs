using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Motor : TickableObject {

    new public virtual string name()
    {
        return "";
    }

    public virtual bool SetOutput(object output)
    {
        return false;
    }

    public virtual JSONNode ToJson()
    {
        return null;
    }

}
