using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool gravityFlip;
    void Start()
    {
        StartCoroutine(BulletDecay());
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            return;
        }
        if (collision.gameObject.GetComponent<BulletEffects>() == null)
        {
            if (gravityFlip)
            {
                collision.gameObject.AddComponent<BulletEffects>().FlipGravity();
            }
        }
        else
        {
            if (gravityFlip)
            {
                collision.gameObject.GetComponent<BulletEffects>().FlipGravity();
            }
        }
        Destroy(gameObject);
    }
    IEnumerator BulletDecay()
    {
        yield return new WaitForSeconds(120);
        Destroy(gameObject);
    }
}