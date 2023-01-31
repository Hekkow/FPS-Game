using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public static class CustomPhysics
{
    static float backForce = 5;
    static float upForce = 5;
    public static void BounceUpAndBack(GameObject item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(-Camera.main.transform.forward.x * backForce, upForce, -Camera.main.transform.forward.z * backForce), ForceMode.Impulse);
    }
    public static void ThrowItem(GameObject item, float throwStartDistance, float throwForce)
    {
        item.transform.position += Camera.main.transform.forward * throwStartDistance;
        item.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
    }
    public static void KnockBack(GameObject enemy, float knockbackAmount)
    {
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce((rb.position - GameObject.Find("Player").transform.position).normalized * knockbackAmount, ForceMode.Impulse);
    }
}
