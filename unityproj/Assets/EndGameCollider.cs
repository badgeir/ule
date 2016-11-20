using UnityEngine;
using System.Collections;

public class EndGameCollider : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 8)
        {

        }

    }
}
