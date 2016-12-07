using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleJSON;

namespace ULE
{

    class UleRPC
    {

        Action mRPCGetEnvironment, mRPCReset;
        Action<JSONNode> mRPCStep, mRPCConfig;

        public UleRPC(Action getEnvironment, Action<JSONNode> step, Action reset, Action<JSONNode> config)
        {
            mRPCGetEnvironment = getEnvironment;
            mRPCStep = step;
            mRPCReset = reset;
            mRPCConfig = config;

        }

        public void ParseInput(string jsonstring)
        {
            JSONNode Json = JSONNode.Parse(jsonstring);
            string methodname = Json["method"];

            switch (methodname)
            {
                case "env":
                    {
                        mRPCGetEnvironment();
                        break;
                    }

                case "step":
                    {
                        mRPCStep(Json["parameters"]);
                        break;
                    }
                case "reset":
                    {
                        mRPCReset();
                        break;
                    }
                case "config":
                    {
                        mRPCConfig(Json["parameters"]);
                        break;
                    }
                default: break;
            }
        }

    }

}