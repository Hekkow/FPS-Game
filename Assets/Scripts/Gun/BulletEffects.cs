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
        if (TryGetComponent(out Rigidbody rb))
        {
            float startTime = Time.time;
            while (Time.time - startTime < 2)
            {
                rb.velocity += new Vector3(0, 1.5f * 9.81f * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
