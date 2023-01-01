using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public void Destruct()
    {
        Helper.MakePhysical(gameObject, false);
        Rigidbody[] rbs = gameObject.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rbs.Length; i++)
        {
            Helper.MakePhysical(rbs[i].gameObject, true);
        }
    }
}
