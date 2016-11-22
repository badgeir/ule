using UnityEngine;
using System;
using System.Collections;
using SimpleJSON;

public abstract class Observation
{
    public abstract string name();
    public abstract string datatype();

    public abstract object value();
    public abstract void set_value(object value);

    public abstract JSONNode ToJson();
}

public class Observation<T> : Observation
{
    private string mName;
    private T mValue;

    public Observation(string name, T startingobservation)
    {
        mName = name;
        mValue = startingobservation;
    }

    public override string name()
    {
        return mName;
    }

    public override string datatype()
    {
        return typeof(T).ToString();
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
        JSONClass json = new JSONClass();
        json[mName]["datatype"] = mValue.GetType().ToString();
        json[mName]["value"] = mValue.ToString();
        return json;
    }
}
