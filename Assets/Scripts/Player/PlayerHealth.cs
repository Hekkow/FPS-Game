using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    public static event Action onPlayerHurt;
    Health health;
    bool canHurt = true;
    bool lastChance = true;
    void Awake()
    {
        health = GetComponent<Health>();
    }
    public void Damaged(float amount, object collision, object origin)
    {
        if (canHurt)
        {
            if (lastChance && health.currentHealth - amount <= 0)
            {
                health.Damage(health.currentHealth - 1);
                StartCoroutine(LastChance());
            }
            else
            {
                health.Damage(amount);
                if (health.currentHealth <= 0)
                {
                    Killed();
                }
            }
            onPlayerHurt?.Invoke();

        }
    }
    public void Killed()
    {
        Debug.Log("Died");
    }
    IEnumerator LastChance()
    {
        canHurt = false;
        yield return new WaitForSeconds(1);
        canHurt = true;
        lastChance = false;
    }
}
