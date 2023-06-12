using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUpgrades : MonoBehaviour
{
    public GunSlot[] allGunSlots;
    public static Gun[] guns;
    private void Start()
    {
        allGunSlots = UpgradeManager.allGunSlots;
        guns = Inventory.guns;
    }
}
