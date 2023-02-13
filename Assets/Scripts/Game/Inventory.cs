using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public static Gun[] guns = new Gun[2];
    public static void PickupGun(Gun gun)
    {
        if (guns[1] == null)
        {
            guns[1] = guns[0];
            guns[0] = gun;
        }
        else 
        {
            guns[0] = gun;
        }
    }
    public static void SwitchGun()
    {
        Gun tmp = guns[0];
        guns[0] = guns[1];
        guns[1] = tmp;
    }
    public static void DropGun()
    {
        guns[0] = guns[1];
        guns[1] = null;
    }
    public static bool HasGuns(int i)
    {
        return guns[i] != null;
    }
}
