using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ReinforcementAgent : MonoBehaviour {

    public VirtualCamera mCamera;
    public List<Sensor> mSensors;
    public List<Motor> mMotors;

    public void UpdateMotor(JSONNode motor)
    {
        foreach (Motor m in mMotors)
        {
            string name = motor["name"];
            if (m.name() == name)
            {
                m.PushJson(motor);
            }
        }
    }

}
