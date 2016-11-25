using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class VectorSensor : Sensor {

	protected string mName;

	public int mLength;
	public float mMinVal, mMaxVal;

	protected VectorSpace mVectorSpace;
	protected float[] mVector;

	void Start()
	{
		Init();
	}
	public void Init()
	{
		mVectorSpace = new VectorSpace(mLength, mMinVal, mMaxVal);
		mVector = mVectorSpace.Zeros();
	}

	public override string name()
	{
		return mName;
	}

    public override JSONNode JsonDescription()
    {
        JSONNode json = mVectorSpace.ToJSON();
        json["name"] = mName;
        return json;
    }

	public override JSONNode SampleJson()
	{
		JSONClass json = new JSONClass();
		for (int i = 0; i < mLength; i++)
		{
			json[mName][i].AsFloat = mVector[i];
		}
		return json;
	}
}
