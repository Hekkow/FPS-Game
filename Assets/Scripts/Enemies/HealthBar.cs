using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    TMP_Text healthText;
    Health health;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform target;
    void Awake()
    {
        health = GetComponent<Health>();
        healthText = GetComponentInChildren<TMP_Text>();
    }
    private void Start()
    {
        healthText.enabled = false;
    }
    void LateUpdate()
    {
        
        Vector3 direction = (target.position - Camera.main.transform.position).normalized;
        bool isBehind = Vector3.Dot(direction, Camera.main.transform.forward) <= 0;
        healthText.enabled = !isBehind;
        healthText.transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
        healthText.text = health.ToString();

    }
    public void Disable()
    {
        healthText.enabled = false;
    }
    public void Enable()
    {
        healthText.enabled = true;
    }
}
