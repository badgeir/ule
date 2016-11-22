using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Motor : TickableObject {

    public virtual string name()
    {
        return "";
    }

    public virtual bool SetOutput(string output)
    {
        return false;
    }

    public virtual JSONNode ToJson()
    {
        return null;
    }
}
