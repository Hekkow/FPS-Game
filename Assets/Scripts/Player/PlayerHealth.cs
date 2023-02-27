using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    public GameEvent onPlayerHurt;
    Health health;
    bool canHurt = true;
    bool lastChance = true;
    void Awake()
    {
        health = GetComponent<Health>();
    }
    public void Damaged(float amount, object collision)
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
            }
            onPlayerHurt.Raise(null, null);

        }
    }
    IEnumerator LastChance()
    {
        canHurt = false;
        yield return new WaitForSeconds(1);
        canHurt = true;
        lastChance = false;
    }
}
