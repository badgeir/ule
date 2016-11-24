using UnityEngine;
using System.Collections;
using SimpleJSON;

public class VectorSpace : Space {

	private int mLength;
	private float mMinVal, mMaxVal;

	public VectorSpace(int length, float minval = float.MinValue, float maxval = float.MaxValue)
	{
		mLength = length;
		mMinVal = minval;
		mMaxVal = maxval;
	}

	public float[] Zeros()
	{
		return new float[mLength];
	}

	public override bool Contains(object o)
	{
		return false;
	}

	public override JSONNode ToJSON()
	{
		JSONClass json = new JSONClass();
		json["type"] = "vector";
		json["min"].AsFloat = mMinVal;
		json["max"].AsFloat = mMaxVal;
		json["length"].AsInt = mLength;
		return json;
	}

}
