using UnityEngine;
using System.Collections;
using SimpleJSON;

public class DiscreteActionMotor : Motor {

    public string mName;
    public int mNumActions;

    private DiscreteSpace mDiscreteSpace;

    protected void Init()
    {
        mDiscreteSpace = new DiscreteSpace(mNumActions);   
    }

    public override bool SetOutput(object value)
    {
        if(mDiscreteSpace.Contains(value))
        Act((int)value);

        return true;
    }

    public override bool PushJson(JSONNode json)
    {

        return false;
    }

    protected virtual void Act(int action)
    {

    }

    public override JSONNode JsonDescription()
    {
        JSONNode json = mDiscreteSpace.ToJSON();
        json["name"] = mName;
        return json;
    }
}
