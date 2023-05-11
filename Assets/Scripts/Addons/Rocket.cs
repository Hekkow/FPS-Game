using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour, IDamageable
{
    Rigidbody rb;
    float speed = 100;
    float explosionRadius = 5;
    float force = 30;
    float upForce = 1f;
    float damage = 50;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        KinematicCharacterMotor player = GameObject.Find("Player").GetComponent<KinematicCharacterMotor>();
        float dotProduct = Vector3.Dot(transform.forward, player.Velocity);
        float finalSpeed = speed + Mathf.Abs(dotProduct);
        rb.velocity = transform.forward * finalSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, ~LayerMask.GetMask("Bullet"));
        List<ILaunchable> launchables = new List<ILaunchable>();
        List<IDamageable> damageables = new List<IDamageable>();

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponentInParent(out ILaunchable launchable) && !launchables.Contains(launchable))
            {
                launchables.Add(launchable);
                launchable.Launch(transform.position, force, upForce);

            }
            if (collider.TryGetComponentInParent(out IDamageable damageable) && !damageables.Contains(damageable))
            {
                damageables.Add(damageable);
                damageable.Damaged(damage, collider, this);
            }
        }
        Destroy(gameObject);
    }

    public void Damaged(float amount, object collision, object origin)
    {
        Killed();
    }

    public void Killed()
    {
        Explode();
    }
}
