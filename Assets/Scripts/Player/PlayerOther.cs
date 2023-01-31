using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOther : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    Transform leftHandLocation;
    Transform rightHandLocation;

    PlayerInputAction playerInput;

    Player player;
    bool throwCooldown = false;
    bool throwDown;
    bool leftDown;
    bool rightDown;

    void Awake()
    {
        player = GetComponent<Player>();
        leftHandLocation = transform.Find("Camera").Find("Left Hand Location");
        rightHandLocation = transform.Find("Camera").Find("Right Hand Location");
        playerInput = new PlayerInputAction();
    }
    void OnEnable()
    {
        playerInput.Player.InteractLeft.performed += PickupLeft;
        playerInput.Player.InteractRight.performed += PickupRight;
        playerInput.Player.Throw.performed += (obj) => throwDown = true;
        playerInput.Player.Throw.canceled += (obj) => throwDown = false;
        playerInput.Player.ShootLeft.performed += (obj) => leftDown = true;
        playerInput.Player.ShootRight.performed += (obj) => rightDown = true;
        playerInput.Player.ShootLeft.canceled += (obj) => leftDown = false;
        playerInput.Player.ShootRight.canceled += (obj) => rightDown = false;

        playerInput.Player.ShootRight.Enable();
        playerInput.Player.ShootLeft.Enable();
        playerInput.Player.InteractLeft.Enable();
        playerInput.Player.InteractRight.Enable();
        playerInput.Player.Throw.Enable();
    }
    void PickupLeft(InputAction.CallbackContext obj)
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
    void PickupRight(InputAction.CallbackContext obj)
    {
        Pickup item = GetClosest();

        if (item != null)
        {
            if (Inventory.HoldingItem(Inventory.Hand.Right))
            {
                Drop(Inventory.Hand.Right);
            }
            PickUp(item, Inventory.Hand.Right);

        }
    }
    void OnDisable()
    {
        playerInput.Player.ShootRight.Disable();
        playerInput.Player.ShootLeft.Disable();
        playerInput.Player.InteractLeft.Disable();
        playerInput.Player.InteractRight.Disable();
        playerInput.Player.Throw.Disable();

    }
    void Update()
    {
        if (transform.position.y < -100)
        {
            GameManager.Spawn();
        }

        if (throwDown)
        {
            if (throwCooldown == false)
            {
                if (Inventory.HoldingItem(Inventory.Hand.Left) && !Inventory.HoldingItem(Inventory.Hand.Right))
                {
                    ThrowItem(Inventory.Hand.Left);
                }
                else if (Inventory.HoldingItem(Inventory.Hand.Right) && !Inventory.HoldingItem(Inventory.Hand.Left))
                {
                    ThrowItem(Inventory.Hand.Right);
                }
                else
                {
                    if (leftDown && Inventory.HoldingItem(Inventory.Hand.Left))
                    {
                        ThrowItem(Inventory.Hand.Left);
                    }
                    if (rightDown && Inventory.HoldingItem(Inventory.Hand.Right))
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
    }
    void Drop(Inventory.Hand hand)
    {
        Transform item;
        if (hand == Inventory.Hand.Left) item = leftHandLocation.GetChild(0);
        else item = rightHandLocation.GetChild(0);
        Helper.MakePhysical(item.gameObject, true);
        Helper.ToggleComponent<Shoot>(item.gameObject, false);
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
        Helper.ToggleComponent<Shoot>(item.gameObject, true);
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
        Helper.AddDamage(item, player.throwDamage, player.throwKnockback, true, true);
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