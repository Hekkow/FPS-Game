using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddonLoot : Loot
{
    public AddonManager.AddonName addon;
    public override void Damaged(float amount, Vector3 hitPoint, Component origin)
    {
        if (origin is Gun)
        {
            base.Damaged(amount, hitPoint, origin);
        }
    }
    public override void Killed()
    {
        AddonManager.AddAddon(addon); 
        Destroy(gameObject);
    }
}
