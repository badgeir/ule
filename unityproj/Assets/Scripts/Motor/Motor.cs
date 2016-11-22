using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Motor : TickableObject {

    public virtual string name()
    {
        return "";
    }

    public virtual object max()
    {
        return 0;
    }

    public virtual object min()
    {
        return 0;
    }

    public virtual void set_output(AgentAction output)
    {
    }

    public virtual JSONNode ToJson()
    {
        return null;
    }
}
