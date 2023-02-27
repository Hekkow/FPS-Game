using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public static Gun[] guns = new Gun[3];
    public static int slotCount = 0;

    public static void PickupGun(Gun gun)
    {
        if (guns[0] == null)
        {
            guns[0] = gun;
            slotCount++;
        }
        else if (guns[1] == null)
        {
            guns[1] = guns[0];
            guns[0] = gun;
            slotCount++;
        }
        else if (guns[2] == null)
        {
            guns[2] = guns[1];
            guns[1] = guns[0];
            guns[0] = gun;
            slotCount++;
        }
        else
        {
            guns[0] = gun;
        }
    }
    public static bool SwitchGun(bool up)
    {
        if (guns[0] == null) { return false; }
        else if (guns[1] == null) { return false; }
        else if (guns[2] == null)
        {
            Gun tmp = guns[0];
            guns[0] = guns[1];
            guns[1] = tmp;
            return true;
        }
        else
        {
            if (up)
            {
                Gun tmp = guns[0];
                guns[0] = guns[1];
                guns[1] = guns[2];
                guns[2] = tmp;
                return true;
            }
            else
            {
                Gun tmp = guns[2];
                guns[2] = guns[1];
                guns[1] = guns[0];
                guns[0] = tmp;
                return true;
            }
        }
    }
    public static void DropGun()
    {
        if (guns[0] == null) { }
        else if (guns[1] == null)
        {
            guns[0] = null;
            slotCount--;
        }
        else if (guns[2] == null)
        {
            guns[0] = guns[1];
            guns[1] = null;
            slotCount--;
        }
        else
        {
            guns[0] = guns[1];
            guns[1] = guns[2];
            guns[2] = null;
            slotCount--;
        }
    }
    public static int HasGuns()
    {
        if (guns[0] == null) return 0;
        if (guns[1] == null) return 1;
        if (guns[2] == null) return 2;
        return 3;
    }
    public static bool HasGun()
    {
        return guns[0] != null;
    }
    public static void ResetBullets()
    {
        if (guns[0] != null) guns[0].ResetBulletsAfterUpgrade();
        if (guns[1] != null) guns[1].ResetBulletsAfterUpgrade();
        if (guns[2] != null) guns[2].ResetBulletsAfterUpgrade();
    }
}
