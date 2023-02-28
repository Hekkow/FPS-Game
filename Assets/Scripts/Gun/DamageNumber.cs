using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public Collision collision;
    TMP_Text damageNumbersText;
    Vector3 collisionPoint;
    Vector3 randomizedLocation;
    void Start()
    {
        randomizedLocation = new Vector3(Random.Range(-20, 20), Random.Range(10, 20), 0);
        collisionPoint = collision.contacts[collision.contacts.Length - 1].point;
        damageNumbersText = GetComponent<TMP_Text>();
        StartCoroutine(DestroyThis());
    }
    void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(collisionPoint);
        if (screenPoint.z >= 0)
        {
            damageNumbersText.transform.position = screenPoint + randomizedLocation;
        }
    }
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(damageNumbersText.gameObject);
    }
}
