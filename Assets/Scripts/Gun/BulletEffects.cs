using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletEffects : MonoBehaviour
{
    Coroutine enemyAgentCoroutine;
    float gravityFlipVelocity = 9.81f * 1.5f;
    float gravityFlipTime = 1;
    public void FlipGravity()
    {
        StartCoroutine(GravityFlipCoroutine());
    }
    IEnumerator GravityFlipCoroutine()
    {
        float startTime = Time.time;
        if (TryGetComponent(out Rigidbody rb))
        {
            //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (TryGetComponent(out Enemy enemy))
            {
                if (enemyAgentCoroutine != null) StopCoroutine(enemyAgentCoroutine);
                enemyAgentCoroutine = StartCoroutine(enemy.DisableAgentCoroutine());
            }
            while (Time.time - startTime < gravityFlipTime)
            {
                rb.velocity += new Vector3(0, gravityFlipVelocity * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
