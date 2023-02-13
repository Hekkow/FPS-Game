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
    [SerializeField] Transform leftHandLocation;
    [SerializeField] Transform rightHandLocation;

    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }
    void OnEnable()
    {
        InputManager.playerInput.Player.Interact.performed += PickupRight;
        InputManager.playerInput.Player.Throw.performed += ThrowItem;
        InputManager.playerInput.Player.Interact.Enable();
        InputManager.playerInput.Player.Throw.Enable();
    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Interact.Disable();
        InputManager.playerInput.Player.Throw.Disable();
    }
    void PickupRight(InputAction.CallbackContext obj)
    {
        Pickup item = GetClosest();
        if (item != null)
        {
            PickUp(item);
        }
    }
    
    void Update()
    {
        if (transform.position.y < -100)
        {
            GameManager.Spawn();
        }
        if (rightHandLocation.childCount > 0)
        {
            Transform item = rightHandLocation.GetChild(0);
            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;
        }
        if (leftHandLocation.childCount > 0)
        {
            Transform item = leftHandLocation.GetChild(0);
            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;
        }
    }
    void Drop(bool gun)
    {
        Transform item;
        if (gun && rightHandLocation.childCount > 0)
        {
            item = rightHandLocation.GetChild(0);
        }
        else if (leftHandLocation.childCount > 0)
        {
            item = leftHandLocation.GetChild(0);
        }
        else return;
        Helper.MakePhysical(item.gameObject, true);
        Helper.ToggleComponent<Gun>(item.gameObject, false);
        Helper.ToggleComponent<Animator>(item.gameObject, false);
        item.transform.localScale *= 2;
        item.parent = GameObject.Find("Objects").transform;
        item.AddComponent<Pickup>();
        Helper.ApplyLayerToChildren(item.gameObject, "Ground");
        Inventory.DropGun();
    }
    void PickUp(Pickup item)
    {
        bool gun = item.gameObject.GetComponent<Gun>();
        Drop(gun);
        if (gun)
        {
            item.transform.parent = rightHandLocation;
            Inventory.PickupGun(item.gameObject.GetComponent<Gun>());
        }
        else item.transform.parent = leftHandLocation;
        Helper.MakePhysical(item.gameObject, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale /= 2;
        Helper.ApplyLayerToChildren(item.gameObject, "Weapon");
        Helper.ToggleComponent<Gun>(item.gameObject, true);
        Helper.ToggleComponent<Animator>(item.gameObject, true);
        Destroy(item);
    }
    void ThrowItem(InputAction.CallbackContext obj)
    {
        GameObject item;
        bool gun = false;
        if (leftHandLocation.childCount > 0)
        {
            item = leftHandLocation.GetChild(0).gameObject;
        }
        else if (rightHandLocation.childCount > 0)
        {
            item = rightHandLocation.GetChild(0).gameObject;
            gun = true;
        }
        else return;
        Drop(gun);
        CustomPhysics.ThrowItem(item, player.throwStartDistance, player.throwForce);
        Helper.AddDamage(item, player.throwDamage, player.throwKnockback, true, true);
    }
    Pickup GetClosest()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, player.pickupDistance) && hit.collider.GetComponent<Pickup>() != null)
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