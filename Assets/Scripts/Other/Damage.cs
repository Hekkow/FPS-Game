using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class Damage : MonoBehaviour
{
    public float knockback = 0;
    public float damage = 0;
    public bool thrown = false;
    public bool oneTime = false;
    public bool environment = true;
    public bool punch = false;
    public float velocityMagnitude;

    bool didDamage = false;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (environment && rb != null) StartCoroutine(UpdateVelocity());
    }
    IEnumerator UpdateVelocity()
    {
        while (true)
        {
            velocityMagnitude = rb.velocity.magnitude;
            yield return new WaitForFixedUpdate();
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (!didDamage)
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                if (collision.gameObject.transform.root != transform.root)
                {
                    damageable.Damaged(damage, collision, this);
                }
                if (oneTime)
                {
                    Destroy(this);
                }
                didDamage = true;
            }
            
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!didDamage)
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                if (environment)
                {
                    if (rb != null)
                    {
                        damage = velocityMagnitude * rb.mass / 10;
                    }
                    if (collision.gameObject.GetComponent<Player>()) return;
                }
                damageable.Damaged(damage, collision, this);
                didDamage = true;
                if (oneTime)
                {
                    Destroy(this);
                }
            }
        }
    }
}
