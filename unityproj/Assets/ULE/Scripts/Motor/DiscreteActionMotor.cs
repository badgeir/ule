using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace ULE
{
    public class DiscreteActionMotor : Motor {

        public string mName;
        protected int mNumActions;

        private DiscreteSpace mDiscreteSpace;

        protected void Init()
        {
            mDiscreteSpace = new DiscreteSpace(mNumActions);

            GameObject.Find("Agent").GetComponent<ReinforcementAgent>().AddMotor(this);
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
                    if(mDiscreteSpace.Contains(action))
                    {
                        Act(action);
                    }
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
            JSONNode json = mDiscreteSpace.ToJSON();
            json["name"] = mName;
            return json;
        }
    }
}
