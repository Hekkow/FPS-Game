using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletEffects : MonoBehaviour
{
    Coroutine enemyAgentCoroutine;
    float gravityFlipVelocity = 9.81f * 1.5f;
    float gravityFlipTime = 1;
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
        if (TryGetComponent(out Enemy enemy))
        {
            if (enemyAgentCoroutine != null) StopCoroutine(enemyAgentCoroutine);
            enemyAgentCoroutine = StartCoroutine(enemy.DisableAgentCoroutine());
        }
        float startTime = Time.time;
        while (Time.time - startTime < gravityFlipTime)
        {
            gravityFlip = true;
            rb.velocity += new Vector3(0, gravityFlipVelocity * Time.deltaTime, 0);
            yield return new WaitForFixedUpdate();
        }
        gravityFlip = false;
    }
    IEnumerator RenableGravityFlip()
    {
        allowGravityFlip = false;
        yield return new WaitForSeconds(0.3f);
        allowGravityFlip = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Rigidbody rb))
        {
            if (gravityFlip && !collision.gameObject.GetComponent<Player>())
            {
                Helper.GetOrAdd<BulletEffects>(collision.gameObject).FlipGravity();
            }
        }
    }
}
