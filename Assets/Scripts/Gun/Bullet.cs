using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(BulletDecay());
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() == null)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator BulletDecay()
    {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }
}