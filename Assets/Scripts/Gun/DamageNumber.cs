using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    TMP_Text damageNumbersText;
    Vector3 collisionPoint;
    Vector3 randomizedLocation;
    void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(collisionPoint);
        if (screenPoint.z >= 0)
        {
            transform.position = screenPoint + randomizedLocation;
        }
    }
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(damageNumbersText.gameObject);
    }
    public void Init(float amount, Collider collision)
    {
        damageNumbersText = GetComponent<TMP_Text>();
        damageNumbersText.text = Mathf.RoundToInt(amount).ToString();
        randomizedLocation = new Vector3(Random.Range(-20, 20), Random.Range(10, 20), 0);
        collisionPoint = collision.transform.position;
        StartCoroutine(DestroyThis());
    }
    public void Init(float amount, Collision collision)
    {
        damageNumbersText = GetComponent<TMP_Text>();
        damageNumbersText.text = Mathf.RoundToInt(amount).ToString();
        randomizedLocation = new Vector3(Random.Range(-20, 20), Random.Range(10, 20), 0);
        collisionPoint = collision.contacts[collision.contacts.Length - 1].point;
        StartCoroutine(DestroyThis());
    }
}
