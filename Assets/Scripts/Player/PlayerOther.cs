using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOther : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Transform leftHandLocation;
    [SerializeField] Transform rightHandLocation;

    Player player;
    bool canSwitch = true;

    void Awake()
    {
        player = GetComponent<Player>();
    }
    void OnEnable()
    {
        InputManager.playerInput.Player.Interact.performed += PickupItem;
        InputManager.playerInput.Player.Throw.performed += ThrowItem;
        InputManager.playerInput.Player.SwitchDown.performed += Switch;
        InputManager.playerInput.Player.Interact.Enable();
        InputManager.playerInput.Player.Throw.Enable();
        InputManager.playerInput.Player.SwitchDown.Enable();

    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Interact.Disable();
        InputManager.playerInput.Player.Throw.Disable();
    }
    void Switch(InputAction.CallbackContext obj)
    {
        if (canSwitch)
        {
            if (Inventory.HasGun() >= 2)
            {
                Inventory.guns[0].gameObject.SetActive(false);
                Inventory.SwitchGun(up: obj.ReadValue<float>() > 0);
                Inventory.guns[0].gameObject.SetActive(true);
            }
            StartCoroutine(RenableSwitch());
        }
    }
    IEnumerator RenableSwitch()
    {
        canSwitch = false;
        yield return new WaitForSeconds(0.05f);
        canSwitch = true;
    }
    void PickupItem(InputAction.CallbackContext obj)
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
            item = Inventory.guns[0].transform;
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
        Gun gun = item.gameObject.GetComponent<Gun>();

        if (gun == null)
        {
            Drop(false);
            item.transform.parent = leftHandLocation;
        }
        if (gun != null)
        {
            List<Upgrade> removedUpgrades = new List<Upgrade>();

            if (Inventory.HasGun() == 3)
            {
                for (int i = 0; i < UpgradeManager.ownedUpgrades.Count; i++)
                {
                    if (UpgradeManager.ownedUpgrades[i].slot == Inventory.guns[0].slot)
                    {
                        int count = UpgradeManager.ownedUpgrades[i].amount;
                        for (int j = 0; j < count; j++)
                        {
                            removedUpgrades.Add(UpgradeManager.ownedUpgrades[i].upgrade);
                            UpgradeManager.DeactivateUpgrade(UpgradeManager.ownedUpgrades[i].upgrade);
                        }
                    }
                }
                gun.slot = -1;
                Drop(true);
            }
            item.gameObject.GetComponent<Gun>().slot = Inventory.slotCount;
            
            Inventory.PickupGun(item.gameObject.GetComponent<Gun>());
            for (int i = 0; i < removedUpgrades.Count; i++)
            {
                UpgradeManager.ActivateUpgrade(removedUpgrades[i]);
            }
            if (Inventory.HasGun() > 1)
            {
                Inventory.guns[1].gameObject.SetActive(false); 
            }
            item.transform.parent = rightHandLocation;
        }
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
        GameObject item = null;
        bool gun = false;
        if (leftHandLocation.childCount > 0)
        {
            item = leftHandLocation.GetChild(0).gameObject;
        }
        else if (rightHandLocation.childCount > 0)
        {
            item = Inventory.guns[0].gameObject; 
            gun = true;
        }
        else return;
        Drop(gun);
        CustomPhysics.ThrowItem(item, player.throwStartDistance, player.throwForce);
        Helper.AddDamage(item, player.throwDamage, player.throwKnockback, true, true);
        if (Inventory.HasGun() > 0) 
        {
            Inventory.guns[0].gameObject.SetActive(true);
        }
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