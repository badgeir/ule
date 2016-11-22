using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FloatSensor : Sensor {

    protected string mName;
    protected float mValue;

    public override string name()
    {
        return mName;
    }

    public virtual void Init()
    {

    }

    public override JSONNode ToJson()
    {
        JSONClass json = new JSONClass();
        json[mName]["type"] = "float";
        json[mName]["value"] = mValue.ToString();
        Debug.Log(mValue.ToString());
        return json;
    }
}
