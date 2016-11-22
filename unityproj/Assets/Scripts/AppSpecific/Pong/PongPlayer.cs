using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PongPlayer : Motor {

    AgentAction<float> mAgentAction;

    private string mName;

	// Use this for initialization
	void Start () {
        mName = "PongPaddle";
        mAgentAction = new AgentAction<float>(transform.position.y, -1, 1);
	}

    public override void set_output(AgentAction output)
    {
        transform.position = Vector3.up * (float)output.value();
    }

    public override JSONNode ToJson()
    {
        JSONClass json = new JSONClass();
        json[mName] = mAgentAction.ToJson();
        return json;
    }

}
