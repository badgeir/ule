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


	public override JSONNode ToJson()
	{
		JSONClass json = new JSONClass();
		json[mName] = mVectorSpace.ToJSON();

		for (int i = 0; i < mLength; i++)
		{
			json[mName]["value"][i].AsFloat = mVector[i];
		}
		return json;
	}
}
