using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerOther : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    InputManager input;
    Transform leftHandLocation;
    Transform rightHandLocation;

    Player player;
    bool throwCooldown = false;

    void Awake()
    {
        input = gameManager.GetComponent<InputManager>();
        player = GetComponent<Player>();
        leftHandLocation = transform.Find("Camera").Find("Left Hand Location");
        rightHandLocation = transform.Find("Camera").Find("Right Hand Location");
    }
    void Update()
    {
        if (transform.position.y < -100)
        {
            GameManager.Spawn();
        }
        if (input.interactRight) {
            Pickup item = GetClosest();
            
            if (item != null)
            {
                Loot loot = item.GetComponent<Loot>();
                if (loot == null)
                {
                    if (Inventory.HoldingItem(Inventory.Hand.Right))
                    {
                        Drop(Inventory.Hand.Right);
                    }
                    PickUp(item, Inventory.Hand.Right);
                }
                else
                {
                    loot.Pickup();
                    input.interactRight = false;
                }

            }
        }
        if (input.interactLeft)
        {
            Pickup item = GetClosest();
            if (item != null)
            {
                if (Inventory.HoldingItem(Inventory.Hand.Left))
                {
                    Drop(Inventory.Hand.Left);
                }
                PickUp(item, Inventory.Hand.Left);
            }
            
        }
        if (input.throwItem)
        {
            if (throwCooldown == false) {
                if (Inventory.HoldingItem(Inventory.Hand.Left) && !Inventory.HoldingItem(Inventory.Hand.Right))
                {
                    ThrowItem(Inventory.Hand.Left);
                }
                else if (Inventory.HoldingItem(Inventory.Hand.Right) && !Inventory.HoldingItem(Inventory.Hand.Left))
                {
                    ThrowItem(Inventory.Hand.Right);
                }
                else {
                    if (input.leftMouse && Inventory.HoldingItem(Inventory.Hand.Left))
                    {
                        ThrowItem(Inventory.Hand.Left);
                    }
                    if (input.rightMouse && Inventory.HoldingItem(Inventory.Hand.Right))
                    {
                        ThrowItem(Inventory.Hand.Right);
                    }
                }
            }
        }
        else 
        {
            throwCooldown = false;
        }
        if (input.switchHands)
        {
            Inventory.SwitchHands();
            if (rightHandLocation.childCount > 0)
            {
                Transform item = rightHandLocation.GetChild(0);
                item.parent = leftHandLocation;
                Helper.ToggleComponent<Gun>(item.gameObject, true);
            }
            if (leftHandLocation.childCount > 0)
            {
                Transform item = leftHandLocation.GetChild(0);
                item.parent = rightHandLocation;
                Helper.ToggleComponent<Gun>(item.gameObject, true);
                

            }
        }
        if (Inventory.HoldingItem(Inventory.Hand.Right))
        {
            GameObject item = rightHandLocation.GetChild(0).gameObject;

            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
        if (Inventory.HoldingItem(Inventory.Hand.Left))
        {
            GameObject item = leftHandLocation.GetChild(0).gameObject;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }
    void Drop(Inventory.Hand hand)
    {
        Transform item;
        if (hand == Inventory.Hand.Left) item = leftHandLocation.GetChild(0);
        else item = rightHandLocation.GetChild(0);
        Helper.MakePhysical(item.gameObject, true);
        Helper.ToggleComponent<Gun>(item.gameObject, false);
        Helper.ToggleComponent<Animator>(item.gameObject, false);
        item.transform.localScale *= 2;
        item.parent = GameObject.Find("Objects").transform;
        item.AddComponent<Pickup>();
        Helper.ApplyLayerToChildren(item.gameObject, "Ground");
        Inventory.EmptyHand(hand);
    }
    void PickUp(Pickup item, Inventory.Hand hand)
    {
        if (hand == Inventory.Hand.Right) item.transform.parent = rightHandLocation;
        else if (hand == Inventory.Hand.Left) item.transform.parent = leftHandLocation;
        Helper.MakePhysical(item.gameObject, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale /= 2;
        Helper.ApplyLayerToChildren(item.gameObject, "Weapon");
        Inventory.ReplaceHand(item.gameObject, hand);
        Helper.ToggleComponent<Gun>(item.gameObject, true);
        Helper.ToggleComponent<Animator>(item.gameObject, true);
        Destroy(item);
    }
    void ThrowItem(Inventory.Hand hand)
    {
        GameObject item;
        if (hand == Inventory.Hand.Left)
        {
            item = leftHandLocation.GetChild(0).gameObject;
        }
        else
        {
            item = rightHandLocation.GetChild(0).gameObject;
        }
        Drop(hand);
        CustomPhysics.ThrowItem(item, player.throwStartDistance, player.throwForce);
        Helper.AddDamage(item, player.throwDamage, player.bulletKnockback, true, true);
        throwCooldown = true;
    }
    Pickup GetClosest()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, player.pickupDistance) && hit.collider.GetComponent<Pickup>() != null)
        {
            return hit.collider.gameObject.GetComponent<Pickup>();
        }
        Pickup[] objects = FindObjectsOfType<Pickup>();
        Pickup closest = null;
        float dot = -2;
        for (int i = 0; i < objects.Length; i++)
        {
            float distance = Vector3.Dot(Camera.main.transform.InverseTransformPoint(objects[i].transform.position).normalized, Vector3.forward);
            if (distance > dot)
            {
                dot = distance;
                closest = objects[i];
            }
        }
        if (dot >= player.dotProductLimit && Vector3.Distance(transform.position, closest.transform.position) <= player.pickupDistance)
        {
            return closest;
        }
        return null;
    }
}