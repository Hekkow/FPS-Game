using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public bool alive = true;
    public void Damage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            alive = false;
        }
    }
    public void Heal(float health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    public void MultiplyMaxHealth(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth *= multiplier;
    }
    public bool IsAlive()
    {
        return alive;
    }
    public float GetHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
