using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeLoot : Loot
{
    public override void Damaged(float amount, Vector3 hitPoint, Component origin)
    {
        base.Damaged(amount, hitPoint, origin);
    }
    public override void Killed()
    {
        Debug.Log("ADD UPGRADE UI");
    }
}
