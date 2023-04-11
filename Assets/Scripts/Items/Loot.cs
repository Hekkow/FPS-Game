using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Loot : MonoBehaviour, IDamageable
{
    protected bool opened = false;
    
    public virtual void Damaged(float amount, object collision, object origin) {
        Killed();
    }
    public virtual void Killed() { }
    
}
