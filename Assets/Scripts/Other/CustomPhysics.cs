using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public static class CustomPhysics
{
    public static void ThrowItem(GameObject item, float throwStartDistance, float throwForce)
    {
        item.transform.position += Camera.main.transform.forward * throwStartDistance;
        item.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
    }
}
