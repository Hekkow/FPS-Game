using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeLoot : Loot
{
    Transform player;
    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }
    public override void Damaged(float amount, object collision, object origin)
    {
        base.Damaged(amount, collision, origin);
    }
    public override void Killed()
    {
        Debug.Log("ADD UPGRADE UI");
    }
}
