using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace ULE
{
    public class VectorMotor : Motor {

        public string mName;
        public int mLength;
        public float mMinVal;
        public float mMaxVal;

        private VectorSpace mVectorSpace;

        protected void Init()
        {
            mVectorSpace = new VectorSpace(mLength, mMinVal, mMaxVal);

            GameObject.Find("Agent").GetComponent<ReinforcementAgent>().AddMotor(this);
        }

        public override bool PushJson(JSONNode json)
        {
            bool status = true;

            JSONNode value = json["value"];

            if (value != null)
            {
                try
                {
                    if(value.Count != mLength)
                    {
                        return false;
                    }

                    float[] vec = new float[mLength];
                    for (int i = 0; i < mLength; i++ )
                    {
                        vec[i] = value[i].AsFloat;
                    }
                    Act(vec);
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

        public override JSONNode JsonDescription()
        {
            JSONNode json = mVectorSpace.ToJSON();
            json["name"] = mName;
            return json;
        }
    }
}
