using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] float lastChanceTime;
    public static event Action onPlayerHurt;
    Health health;
    bool canHurt = true;
    bool lastChance = true;
    void Start()
    {
        health = GetComponent<Health>();
    }
    public void Damaged(float amount, Vector3 hitPoint, Component origin)
    {
        float realAmount = amount;
        if (canHurt)
        {
            if (lastChance && health.health - realAmount <= 0)
            {
                health.health -= health.health - 1;
                StartCoroutine(LastChance());
            }
            else
            {
                health.health -= realAmount;
                if (health.health <= 0)
                {
                    Killed();
                }
            }
            onPlayerHurt?.Invoke();

        }
    }
    public void Killed()
    {
        health.alive = false;
        Debug.Log("Died");
    }
    IEnumerator LastChance()
    {
        canHurt = false;
        yield return new WaitForSeconds(lastChanceTime);
        canHurt = true;
        lastChance = false;
    }
}
