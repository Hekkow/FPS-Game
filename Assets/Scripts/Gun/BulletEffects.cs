using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffects : MonoBehaviour
{
    public void FlipGravity()
    {
        StartCoroutine(FlipGravityCoroutine());
    }
    public IEnumerator FlipGravityCoroutine()
    {
        float startTime = Time.time;
        if (TryGetComponent(out Rigidbody rb))
        {
            while (Time.time - startTime < 2)
            {
                rb.velocity += new Vector3(0, 1.5f * 9.81f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }
        Destroy(this);
    }
    void OnCollisionEnter(Collision collision)
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
