using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public float radius;
    public float force;
    public float explosionTime;
    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                StartCoroutine(RemoveEffect());
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }
    }
    IEnumerator RemoveEffect()
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Explosion"), this.transform);
        Destroy(this.gameObject.GetComponent<Renderer>());
        yield return new WaitForSeconds(explosionTime);
        Destroy(effect);
        Destroy(this);

    }

}
