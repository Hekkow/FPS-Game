using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class Inventory
{
    public static GameObject leftHand;
    public static GameObject rightHand;
    public enum Hand { Left, Right };
    public static void ReplaceHand(GameObject newHand, Hand hand)
    {
        if (hand == Hand.Left)
        {
            leftHand = newHand;
        }
        else if (hand == Hand.Right)
        {
            rightHand = newHand;
        }
    }
    public static void EmptyHand(Hand hand)
    {
        if (hand == Hand.Left)
        {
            leftHand = null;
        }
        else if (hand == Hand.Right)
        {
            rightHand = null;
        }
    }
    public static bool HoldingItem()
    {
        return leftHand != null || rightHand != null;
    }
    public static bool HoldingItem(Hand hand)
    {
        if (hand == Hand.Left) return leftHand != null;
        else if (hand == Hand.Right) return rightHand != null;
        else return false;

    }
    public static bool HasGun(Hand hand)
    {
        if (hand == Hand.Left && HoldingItem(Hand.Left) && leftHand.GetComponent<Gun>() != null) return true;
        if (hand == Hand.Right && HoldingItem(Hand.Right) && rightHand.GetComponent<Gun>() != null) return true;
        return false;
    }
    public static void SwitchHands()
    {
        GameObject temp = leftHand;
        leftHand = rightHand;
        rightHand = temp;
    }

}
