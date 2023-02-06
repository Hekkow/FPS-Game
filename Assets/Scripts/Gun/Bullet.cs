using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    float timeSpawned;
    void Start()
    {
        timeSpawned = Time.time;
        StartCoroutine(BulletDecay());
    }
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(Time.time - timeSpawned + " " + collision.gameObject.name);
        if (UpgradeManager.gravityFlip)
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