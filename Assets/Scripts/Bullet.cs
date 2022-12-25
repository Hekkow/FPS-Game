using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    public int knockback;

    void Start()
    {
        StartCoroutine(BulletDecay());
    }
    void OnCollisionEnter(Collision collision)
    {
            Destroy(gameObject);
    }
    IEnumerator BulletDecay()
    {
        yield return new WaitForSeconds(60);
        Destroy(gameObject);
    }
}