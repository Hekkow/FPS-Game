using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public abstract class Addon
{
    protected float cooldown;
    public abstract void Activate();
}
public class Hook : Addon
{
    public override void Activate()
    {
        Movement player = GameObject.Find("Player").GetComponent<Movement>();
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.SphereCast(ray, 0.01f, out RaycastHit hit, player.hookRange))
        {
            player.Hook(hit);
        }
    }
}
public static class AddonManager
{
    public enum AddonName
    {
        Hook,
    }
    public static void AddAddon(AddonName name)
    {
        switch (name)
        {
            case AddonName.Hook:
                Inventory.guns[0].addon = new Hook();
                break;
        }
    }
}

