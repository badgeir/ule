using UnityEngine;
using System.Collections;
using SimpleJSON;

public class DiscreteSpace : Space {

    public int mRange;

    public DiscreteSpace(int range)
    {
        mRange = range;
    }

    public override bool Contains(object o)
    {
        bool success = true;
        //check that o is of type int
        if(o.GetType() != typeof(int))
        {
            success =  false;
        }
        // check that value is within range
        else if((int)o >= mRange || (int)o < 0)
        {
            success = false;
        }
        return success;
    }

    public override JSONNode ToJSON()
    {
        JSONClass json = new JSONClass();
        json["type"] = "Discrete Action";
        json["range"].AsInt = mRange;
        return json;
    }
}
