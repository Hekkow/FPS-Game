using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSlotManager : MonoBehaviour
{
    public static List<GunSlot> usedSlots = new List<GunSlot>();
    public static List<GunSlot> unusedSlots = new List<GunSlot>();

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            unusedSlots.Add(new GunSlot());
            Debug.Log(unusedSlots[i]);
        }
    }
    public static void SwitchGuns(Gun switchFrom, Gun switchTo)
    {
        GunSlot gunSlot = switchFrom.upgradeSlot;
        switchFrom.upgradeSlot = null;
        switchTo.upgradeSlot = gunSlot;
        unusedSlots.Add(gunSlot);
        usedSlots.Remove(gunSlot);
    }
    public static void GetFirstOpenSlot(Gun gun)
    { 
        gun.upgradeSlot = unusedSlots[0];
        usedSlots.Add(gun.upgradeSlot);
        unusedSlots.Remove(gun.upgradeSlot);
    }
}
