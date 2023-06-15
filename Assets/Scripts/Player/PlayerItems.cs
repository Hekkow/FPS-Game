using KinematicCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItems : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Transform leftHandLocation;
    [SerializeField] Transform rightHandLocation;
    [SerializeField] Transform itemsParent;
    [SerializeField] Transform gunsParent;

    public event Action onGunSwitch;

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
        InputManager.playerInput.Player.SwitchDown.Disable();
    }
    void FixedUpdate()
    {
        if (transform.position.y < -100)
        {
            player.GetComponent<KinematicCharacterMotor>().SetPosition(new Vector3(0, 100, 0));
        }
    }
    void DropItem()
    {
        Transform item = leftHandLocation.GetChild(0);
        item.gameObject.MakePhysical(true);
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        item.localScale *= 2;
        item.parent = itemsParent;
        item.AddComponent<Pickup>();
        item.gameObject.ApplyLayerToChildren("Ground");
        holdingItem = false;
    }
    void DropGun(Gun gun)
    {
        Transform item = gun.transform;
        item.gameObject.MakePhysical(true);
        gun.shootAnimator.Rebind();
        gun.shootAnimator.Update(0f);
        gun.enabled = false;
        item.gameObject.GetComponent<Animator>().enabled = false;
        item.localScale *= 2;
        item.parent = gunsParent;
        item.AddComponent<Pickup>();
        item.gameObject.ApplyLayerToChildren("Ground");
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        
    }
    void PickupItem(Transform item)
    {
        if (holdingItem)
        {
            DropItem();
        }
        item.parent = leftHandLocation;
        item.gameObject.MakePhysical(false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale /= 2;
        item.gameObject.ApplyLayerToChildren("Weapon");
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        Destroy(item.GetComponent<Pickup>());
        holdingItem = true;
    }
    void PickupGun(Transform item)
    {
        Gun gun = item.GetComponent<Gun>();
        if (item.TryGetComponent(out GunThrowDamage gunThrow)) Destroy(gunThrow);
        Gun gunToDrop = Inventory.guns?[0];
        if (Inventory.HasGuns() >= 3)
        {
            Inventory.PickupGun(gun);
            DropGun(gunToDrop);
            UpgradeManager.SwitchGuns(gunToDrop, gun);
        }
        else
        {
            Inventory.PickupGun(gun);
            UpgradeManager.GetFirstOpenSlot(gun);
        }
        if (Inventory.HasGuns() > 1)
        {
            Inventory.guns[1].gameObject.SetActive(false);
        }
        item.parent = rightHandLocation;
        item.gameObject.MakePhysical(false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale /= 2;
        item.gameObject.ApplyLayerToChildren("Weapon");
        item.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        Inventory.guns[0].enabled = true;
        item.gameObject.GetComponent<Animator>().enabled = true;
        Destroy(item.GetComponent<Pickup>());
        onGunSwitch?.Invoke();

    }
    void ThrowItem()
    {
        GameObject item = leftHandLocation.GetChild(0).gameObject;
        item.SetActive(true);
        DropItem();
        item.AddComponent<ThrowDamage>();
        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
    void ThrowGun()
    {
        GameObject item = Inventory.guns[0].gameObject;
        UpgradeManager.DropGun(Inventory.guns[0]);
        DropGun(Inventory.guns[0]);
        Inventory.DropGun();
        
        item.AddComponent<ThrowDamage>();

        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.None;
        }
        if (Inventory.HasGun()) Inventory.guns[0].gameObject.SetActive(true);
        onGunSwitch?.Invoke();

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
        else if (Inventory.HasGun()) ThrowGun();
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
            onGunSwitch?.Invoke();

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