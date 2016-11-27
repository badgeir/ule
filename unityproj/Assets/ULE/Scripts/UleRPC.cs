using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleJSON;

namespace ULE
{

    class UleRPC
    {

        Action mOnGetEnvironment, mReset;
        Action<JSONNode> mMotorUpdater;

        public UleRPC(Action onGetEnvironment, Action<JSONNode> motorUpdater, Action reset)
        {
            mOnGetEnvironment = onGetEnvironment;
            mMotorUpdater = motorUpdater;
            mReset = reset;
        }

        public void ParseInput(string jsonstring)
        {
            JSONNode Json = JSONNode.Parse(jsonstring);
            string methodname = Json["method"];

            switch (methodname)
            {
                case "getEnvironmentInfo":
                    {
                        mOnGetEnvironment();
                        break;
                    }

                case "updateEnvironment":
                    {
                        JSONNode parameters = Json["parameters"];
                        updateEnvironment(parameters);
                        break;
                    }
                case "reset":
                    {
                        mReset();
                        break;
                    }
                default: break;
            }
        }

        void updateEnvironment(JSONNode json)
        {
            for (int motor = 0; motor < json["motors"].Count; motor++)
            {
                mMotorUpdater(json["motors"][motor]);
            }
        }


    }

}