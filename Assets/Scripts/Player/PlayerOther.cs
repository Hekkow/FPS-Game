using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PlayerOther : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Transform leftHandLocation;
    [SerializeField] Transform rightHandLocation;
    [SerializeField] GameEvent onWeaponChange;
    [SerializeField] Transform itemsParent;
    [SerializeField] Transform gunsParent;
    Player player;
    bool canSwitch = true;
    bool holdingItem = false;

    void Awake()
    {
        player = GetComponent<Player>();
    }
    void OnEnable()
    {
        InputManager.playerInput.Player.Interact.performed += PickupThing;
        InputManager.playerInput.Player.Throw.performed += ThrowThing;
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
    

    void Update()
    {
        if (transform.position.y < -100)
        {
            GameManager.Spawn();
        }
        //if (rightHandLocation.childCount > 0)
        //{
        //    Transform item = Inventory.guns[0].transform;
        //    item.localPosition = Vector3.zero;
        //    item.localRotation = Quaternion.identity;
        //}
        //if (leftHandLocation.childCount > 0)
        //{
        //    Transform item = leftHandLocation.GetChild(0);
        //    item.localPosition = Vector3.zero;
        //    item.localRotation = Quaternion.identity;
        //}
    }
    void DropItem()
    {
        Transform item = leftHandLocation.GetChild(0);
        Helper.MakePhysical(item.gameObject, true);
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        item.localScale *= 2;
        item.parent = itemsParent;
        item.AddComponent<Pickup>();
        Helper.ApplyLayerToChildren(item.gameObject, "Ground");
        holdingItem = false;
    }
    void DropGun()
    {
        Transform item = Inventory.guns[0].transform;
        Helper.MakePhysical(item.gameObject, true);
        Inventory.guns[0].shootAnimator.Play("idle", 0, 0);
        Inventory.guns[0].enabled = false;
        item.gameObject.GetComponent<Animator>().enabled = false;
        item.localScale *= 2;
        item.parent = gunsParent;
        item.AddComponent<Pickup>();
        Helper.ApplyLayerToChildren(item.gameObject, "Ground");
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        Inventory.DropGun();
    }
    void PickupItem(Transform item)
    {
        if (holdingItem)
        {
            DropItem();
        }
        item.parent = leftHandLocation;
        Helper.MakePhysical(item.gameObject, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale /= 2;
        Helper.ApplyLayerToChildren(item.gameObject, "Weapon");
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        Destroy(item.GetComponent<Pickup>());
        holdingItem = true;
    }
    void PickupGun(Transform item)
    {
        List<Upgrade> removedUpgrades = new List<Upgrade>();
        if (Inventory.HasGuns() == 3)
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
            item.GetComponent<Gun>().slot = -1;
            DropGun();
        }
        item.gameObject.GetComponent<Gun>().slot = Inventory.slotCount;
        Inventory.PickupGun(item.gameObject.GetComponent<Gun>());
        for (int i = 0; i < removedUpgrades.Count; i++)
        {
            UpgradeManager.ActivateUpgrade(removedUpgrades[i]);
        }
        if (Inventory.HasGuns() > 1)
        {
            Inventory.guns[1].gameObject.SetActive(false);
        }

        item.parent = rightHandLocation;
        Helper.MakePhysical(item.gameObject, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale /= 2;
        Helper.ApplyLayerToChildren(item.gameObject, "Weapon");
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        Inventory.guns[0].enabled = true;
        item.gameObject.GetComponent<Animator>().enabled = true;
        Destroy(item.GetComponent<Pickup>());
        onWeaponChange.Raise(null, null);
    }
    void ThrowItem()
    {
        GameObject item = leftHandLocation.GetChild(0).gameObject;
        DropItem();
        CustomPhysics.ThrowItem(item, player.throwStartDistance, player.throwForce);
        Helper.AddDamage(item, player.throwDamage, player.throwKnockback, true, true);
    }
    void ThrowGun()
    {
        GameObject item = Inventory.guns[0].gameObject;
        DropGun();
        CustomPhysics.ThrowItem(item, player.throwStartDistance, player.throwForce);
        Helper.AddDamage(item, player.throwDamage, player.throwKnockback, true, true);
        if (Inventory.HasGun()) Inventory.guns[0].gameObject.SetActive(true);
        onWeaponChange.Raise(null, null);
    }
    void PickupThing(InputAction.CallbackContext obj)
    {
        Pickup item = GetClosest();
        if (item != null)
        {
            if (item.GetComponent<Gun>() != null) PickupGun(item.transform);
            else PickupItem(item.transform);
        }
    }
    void ThrowThing(InputAction.CallbackContext obj)
    {
        if (holdingItem) ThrowItem();
        else ThrowGun();
    }
    void Switch(InputAction.CallbackContext obj)
    {
        if (canSwitch)
        {
            if (Inventory.HasGuns() >= 2)
            {
                Inventory.guns[0].gameObject.SetActive(false);
                Inventory.SwitchGun(up: obj.ReadValue<float>() > 0);
                Inventory.guns[0].gameObject.SetActive(true);
            }
            StartCoroutine(RenableSwitch());
            onWeaponChange.Raise(null, null);

        }
    }
    IEnumerator RenableSwitch()
    {
        canSwitch = false;
        yield return new WaitForSeconds(0.05f);
        canSwitch = true;
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