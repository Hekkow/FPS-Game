using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public static GameObject hand;
    public static void ReplaceHand(GameObject newHand)
    {
        hand = newHand;
    }
    public static void EmptyHand()
    {
        hand = null;
    }
    public static bool HoldingItem()
    {
        return hand != null;
    }
    public static bool HasGun()
    {
        if (hand != null && hand.GetComponent<Shoots>() != null)
        {
            return true;
        }
        return false;
    }
}
