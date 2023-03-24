using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Loot : MonoBehaviour, IDamageable
{
    [SerializeField] protected float health;
    
    protected bool opened = false;
    
    public virtual void Damaged(float amount, object collision, object origin) {
        if (health <= 0)
        {
            Killed();
        }
    }
    public virtual void Killed() { }
    
}
