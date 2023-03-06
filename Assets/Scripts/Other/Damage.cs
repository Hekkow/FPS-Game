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
    public float velocityMagnitude;

    float backForceMultiplier = 1.3f;
    float upForce = 5;
    
    bool didDamage = false;
    // for non solid things like animations
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb != null)
        {
            velocityMagnitude = rb.velocity.magnitude;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        Health collisionObjectHealth = collision.gameObject.GetComponent<Health>();
        if (collisionObjectHealth == null) // if health doesn't exist in current object, check parent object
        {
            Transform collisionObjectHealthParent = collision.gameObject.transform.parent;
            if (collisionObjectHealthParent != null)
            {
                collisionObjectHealth = collision.gameObject.transform.parent.GetComponent<Health>();
            }
        }
        if (collisionObjectHealth != null)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (collision.gameObject.transform != transform.root && damageable != null) {
                damageable.Damaged(damage, collision);
            }
            if (oneTime)
            {
                Destroy(this);
            }
        }
    }

    // for solid things like bullets

    void OnCollisionEnter(Collision collision)
    {
        if (!didDamage)
        {
            Health collisionObjectHealth = collision.gameObject.GetComponent<Health>();
            if (collisionObjectHealth == null)
            {
                Transform collisionObjectHealthParent = collision.gameObject.transform.parent;
                if (collisionObjectHealthParent != null)
                {
                    collisionObjectHealth = collision.gameObject.transform.parent.GetComponent<Health>();
                }
            }
            if (collisionObjectHealth != null)
            {
                if (environment)
                {
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        damage = velocityMagnitude * rb.mass / 10;
                    }
                }
                if (thrown)
                {
                    Rigidbody rb = GetComponent<Rigidbody>();
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
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>(); 
                if (damageable != null) damageable.Damaged(damage, collision);
                
                if (oneTime)
                {
                    Destroy(this);
                }
            }
            
        }
        if (thrown) rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
}
