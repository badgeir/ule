using UnityEngine;
using System.Collections;

public class Sensor : TickableObject {

    public virtual Observation observation()
    {
        return null;
    }
}
