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

    public override bool PushJson(JSONNode json)
    {
        bool status = true;

        JSONNode value = json["value"];

        if(value != null)
        {
            try
            {
                int action = value.AsInt;
                Act(action);
            }
            catch
            {
                status = false;
            }
        }
        else
        {
            status = false;
        }
        return status;
    }

    public override string name()
    {
        return mName;
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
