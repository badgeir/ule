using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimpleJSON;


class UleRPC
{

    Action mOnGetEnvironment;
    Action<JSONNode> mMotorUpdater;

    public UleRPC(Action onGetEnvironment, Action<JSONNode> motorUpdater)
    {
        mOnGetEnvironment = onGetEnvironment;
        mMotorUpdater = motorUpdater;
    }

    public void ParseInput(string jsonstring)
    {
        JSONNode Json = JSONNode.Parse(jsonstring);
        string methodname = Json["method"];

        switch(methodname)
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
            case "restart":
            {
                restart();
                break;
            }
            default: break;
        }
    }

    void updateEnvironment(JSONNode json)
    {
        for (int motor = 0; motor < json["motors"].Count; motor++ )
        {
            mMotorUpdater(json["motors"][motor]);
        }
    }

    void restart()
    {

    }


}
