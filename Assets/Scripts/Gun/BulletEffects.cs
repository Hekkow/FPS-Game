using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletEffects : MonoBehaviour
{
    Coroutine enemyAgentCoroutine;
    float gravityFlipVelocity = 9.81f * 3f;
    float gravityFlipTime = 0.3f;
    bool gravityFlip = false;
    bool allowGravityFlip = true;
    public void FlipGravity()
    {
        if (allowGravityFlip) StartCoroutine(GravityFlipCoroutine());
        
    }
    IEnumerator GravityFlipCoroutine()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!allowGravityFlip) yield break;
        if (rb == null) yield break;
        StartCoroutine(RenableGravityFlip());
        float startTime = Time.time;
        if (rb.TryGetComponentInParent(out Enemy enemy))
        {
            StartCoroutine(enemy.DisableAgent());
            rb = enemy.GetComponent<Rigidbody>();
        }
        if (rb.velocity.y < 0) rb.velocity = Vector3.zero;
        while (Time.time - startTime < gravityFlipTime)
        {
            gravityFlip = true;
            rb.velocity = rb.velocity.AddY(gravityFlipVelocity * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        gravityFlip = false;
    }
    IEnumerator RenableGravityFlip()
    {
        allowGravityFlip = false;
        yield return new WaitForSeconds(1f);
        allowGravityFlip = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            if (gravityFlip && !collision.gameObject.GetComponent<Player>())
            {
                collision.gameObject.GetOrAdd<BulletEffects>().FlipGravity();
            }
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            if (gravityFlip && !collision.gameObject.GetComponent<Player>())
            {
                collision.gameObject.GetOrAdd<BulletEffects>().FlipGravity();
            }
        }
    }
}
