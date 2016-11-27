using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Sensor : MonoBehaviour {

    new public virtual string name()
    {
        return "";
    }

    protected virtual void Sample()
    {
    }

    public virtual string SampleJson()
    {
        return null;
    }

    public virtual JSONNode JsonDescription()
    {
        return null;
    }
}
