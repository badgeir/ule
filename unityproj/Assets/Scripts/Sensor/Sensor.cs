using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Sensor : MonoBehaviour {

    new public virtual string name()
    {
        return "";
    }

    public virtual void Sample()
    {
    }

    public virtual JSONNode ToJson()
    {
        return null;
    }
}
