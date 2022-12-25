using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static string HealthToHashtags(Health health)
    {
        string hashtags = "";
        int currentHealth = Mathf.CeilToInt(health.GetHealth() / 10);
        int maxHealth = Mathf.CeilToInt(health.GetMaxHealth() / 10) - currentHealth;
        for (int i = 0; i < currentHealth; i++)
        {
            hashtags += "#";
        }
        for (int i = 0; i < maxHealth; i++)
        {
            hashtags += "-";
        }
        return hashtags;
    }
}
