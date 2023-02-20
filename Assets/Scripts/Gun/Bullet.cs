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
        //Debug.Log(Time.time - timeSpawned + " " + collision.gameObject.name);
        if (gravityFlip)
        {
            collision.gameObject.AddComponent<BulletEffects>().FlipGravity();
        }
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