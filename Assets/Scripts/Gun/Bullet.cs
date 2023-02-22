using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    public bool gravityFlip;
    void Start()
    {
        StartCoroutine(BulletDecay());
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() == null)
        {
            if (collision.gameObject.GetComponent<BulletEffects>() == null)
            {
                if (gravityFlip)
                {
                    collision.gameObject.AddComponent<BulletEffects>().FlipGravity();
                }
            }
            else
            {
                collision.gameObject.GetComponent<BulletEffects>().FlipGravity();
            }
            Destroy(gameObject);

        }

    }
    IEnumerator BulletDecay()
    {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }
}