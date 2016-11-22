using UnityEngine;
using System;
using System.Collections;
using SimpleJSON;

public abstract class AgentAction
{
    public abstract string datatype();

    public abstract object value();
    public abstract void set_value(object value);

    public abstract object max();
    public abstract object min();

    public abstract JSONNode ToJson();
}

public class AgentAction<T> : AgentAction {

    private T mValue;

    private T mMaxVal;
    private T mMinVal;

    bool mLimited;

    public AgentAction(T startingaction)
    {
        mValue = startingaction;
        mLimited = false;
    }

    public AgentAction(T startingaction, T minval, T maxval)
    {
        mValue = startingaction;
        mMinVal = minval;
        mMaxVal = maxval;
        mLimited = true;
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

    public override object max()
    {
        return mMaxVal;
    }

    public override object min()
    {
        return mMinVal;
    }

    public override JSONNode ToJson()
    {
        JSONClass json = new JSONClass();
        json["datatype"] = mValue.GetType().ToString();
        json["value"] = mValue.ToString();
        if(mLimited)
        {
            json["maxval"] = mMaxVal.ToString();
            json["minval"] = mMinVal.ToString();
        }
        return json;
    }
}
