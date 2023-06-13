using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeSlot
{
    public List<UpgradeInfo> upgrades = new List<UpgradeInfo>();
    public Gun gun;
    public override string ToString()
    {
        string str = "";
        if (gun != null) str += "Gun: " + gun.gameObject.name + "\n";
        str += upgrades.Count + " upgrades in this slot\n";
        for (int i = 0; i < upgrades.Count; i++)
        {
            str += upgrades[i].ToString();
        }
        return str;
    }
}
