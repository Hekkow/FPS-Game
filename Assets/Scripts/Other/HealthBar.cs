using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    TMP_Text healthText;
    Health health;
    bool barEnabled = true;
    void Awake()
    {
        health = GetComponent<Health>();
        healthText = GetComponentInChildren<TMP_Text>();
    }
    void Update()
    {
        if (barEnabled)
        {
            healthText.transform.LookAt(Camera.main.transform);
            healthText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
            healthText.text = Helper.HealthToHashtags(health);
        }
    }
    public void Disable()
    {
        healthText.enabled = false;
        barEnabled = false;
    }
    public void Enable()
    {
        healthText.enabled = true;
        barEnabled = true;
    }
}
