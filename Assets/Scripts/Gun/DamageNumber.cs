using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    TMP_Text damageNumbersText;
    Vector3 collisionPoint;
    Vector3 randomizedLocation;
    float duration = 0.3f;
    void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(collisionPoint);
        if (screenPoint.z >= 0)
        {
            transform.position = screenPoint + randomizedLocation;
        }
    }
    public void Init(float amount, Vector3 hitPoint)
    {
        damageNumbersText = GetComponent<TMP_Text>();
        damageNumbersText.text = Mathf.RoundToInt(amount).ToString();
        randomizedLocation = new Vector3(Random.Range(-20, 20), Random.Range(10, 20), 0);
        collisionPoint = hitPoint;
        Destroy(damageNumbersText.gameObject, duration);
    }
}