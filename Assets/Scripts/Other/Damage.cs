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

    float backForceMultiplier = 1.3f;
    float upForce = 5;
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
                if (collision.gameObject.transform != transform.root)
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
                    if (collision.gameObject.GetComponent<Player>() != null) return;
                }
                if (thrown)
                {
                    if (thrown) rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rb.velocity = Vector3.zero;
                    Vector3 velocity;
                    if (collision.gameObject.TryGetComponent(out NavMeshAgent agent))
                    {
                        velocity = (Camera.main.transform.position - collision.transform.position).normalized * agent.velocity.magnitude * backForceMultiplier;
                    }
                    else
                    {
                        velocity = (Camera.main.transform.position - collision.transform.position).normalized * backForceMultiplier;
                    }
                    Vector3 direction = new Vector3(velocity.x, upForce, velocity.z);
                    rb.AddForce(direction, ForceMode.Impulse);
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
