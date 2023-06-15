using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ThrowDamage : Damage
{
    Rigidbody rb;
    float throwForce = 40;
    float throwStartDistance = 1.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.position += Camera.main.transform.forward * throwStartDistance;
        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponentInParent(out IDamageable damageable))
        {
            damageable.Damaged(100, collision.GetContact(0).point, this);
        }
    }
}
