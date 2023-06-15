using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class GunThrowDamage : Damage
{
    Rigidbody rb;
    float returnSpeed = 40;
    float throwForce = 40;
    float throwStartDistance = 1.5f;
    List<IDamageable> hurt = new List<IDamageable>();
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        transform.position += Camera.main.transform.forward * throwStartDistance;
        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        Vector3 direction = (Camera.main.transform.position - transform.position).normalized;
        rb.AddForce(direction * returnSpeed, ForceMode.Force);
    }
    void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;
        if (collision.gameObject.TryGetComponentInParent(out IDamageable damageable))
        {
            if (!hurt.Contains(damageable))
            {
                hurt.Add(damageable);
                damageable.Damaged(20, collision.GetContact(0).point, null);
            }
            if (collision.gameObject.GetComponentInParent<Player>())
            {
                rb.useGravity = true;
                Destroy(this);
            }
        }

    }
}
