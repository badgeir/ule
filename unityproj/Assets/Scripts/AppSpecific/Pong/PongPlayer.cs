using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PongPlayer : Motor {

    AgentAction<float> mAgentAction;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void set_output(AgentAction output)
    {
        transform.Translate(Vector3.up * (float)output.value());
    }

    JSONNode JsonNode()
    {
        JSONArray json = new JSONArray();
        json["PongPlayer"].Add(mAgentAction.ToJson());
        return json;
    }

}
