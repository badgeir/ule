using UnityEngine;
using System.Collections;
using SimpleJSON;

public class DiscreteActionMotor : Motor {

    public string mName;
    public int mNumActions;

    private int mMaxValue;
    private int mMinValue;

    protected void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        Debug.Log("Init " + mNumActions);

        mMinValue = 0;
        mMaxValue = mNumActions - 1;
        
    }

    public override bool SetOutput(string output)
    {
        int val = int.Parse(output);
        if(val > mMaxValue || val < mMinValue)
        {
            return false;
        }
        Act(val);
        return true;
    }

    protected virtual void Act(int action)
    {

    }

    public override string name()
    {
        return mName;
    }

    public override JSONNode ToJson()
    {
        JSONClass json = new JSONClass();
        json[mName]["type"] = "Discrete Action";
        json[mName]["range"].AsInt = mNumActions;
        return json;
    }
}
