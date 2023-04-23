using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ThrowDamage : MonoBehaviour
{
    float damage = 34;
    Rigidbody rb;
    float backForceMultiplier = 1.3f;
    float upForce = 5;
    bool didDamage = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (didDamage) return;
        didDamage = true;
        if (collision.gameObject.TryGetComponentInParent(out IDamageable damageable))
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
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
            Vector3 direction = velocity.SetY(upForce);
            rb.AddForce(direction, ForceMode.Impulse);
            damageable.Damaged(damage, collision, this);
            Destroy(this);
        }

    }
}
