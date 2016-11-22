using UnityEngine;
using System;
using System.Collections;
using SimpleJSON;

public abstract class AgentAction
{
    public abstract string name();
    public abstract string datatype();

    public abstract object value();
    public abstract void set_value(object value);

    public abstract JSONNode ToJson();
}

public class AgentAction<T> : AgentAction {

    private string mName;
    private T mValue;

    public AgentAction(string name, T startingaction)
    {
        mName = name;
        mValue = startingaction;
    }

    public override string name()
    {
        return mName;
    }

    public override string datatype()
    {
        return mValue.GetType().ToString();
    }

    public override object value()
    {
        return mValue;
    }

    public override void set_value(object value)
    {
        mValue = (T)value;
    }

    public override JSONNode ToJson()
    {
        JSONArray json = new JSONArray();
        json["datatype"] = mValue.GetType().ToString();
        json["value"] = mValue.ToString();
        return json;
    }
}
