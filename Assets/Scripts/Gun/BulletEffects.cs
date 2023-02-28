using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletEffects : MonoBehaviour
{
    bool gravityFlip = false;
    public void FlipGravity()
    {
        gravityFlip = true;
        StartCoroutine(FlipGravityCoroutine());
    }
    public IEnumerator FlipGravityCoroutine()
    {
        float startTime = Time.time;
        if (TryGetComponent(out Rigidbody rb))
        {
            if (TryGetComponent(out Enemy enemy)) StartCoroutine(enemy.DisableAgentCoroutine());
            while (Time.time - startTime < 3)
            {
                rb.velocity += new Vector3(0, 1.5f * 9.81f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }
        Destroy(this);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (gravityFlip)
        {
            if (collision.gameObject.GetComponent<Player>() == null)
            {
                if (collision.gameObject.GetComponent<BulletEffects>() == null)
                {
                    collision.gameObject.AddComponent<BulletEffects>().FlipGravity();
                }
                else
                {
                    collision.gameObject.GetComponent<BulletEffects>().FlipGravity();
                }
            }
        }
    }
}
