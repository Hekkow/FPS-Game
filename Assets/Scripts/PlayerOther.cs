using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerOther : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    PlayerControls input;
    Transform handLocation;

    Player player;

    void Awake()
    {
        input = gameManager.GetComponent<PlayerControls>();
        player = GetComponent<Player>();
        handLocation = transform.Find("Camera").Find("Hand Location");
    }
    void Update()
    {
        if (transform.position.y < -100)
        {
            GameManager.Spawn();
        }
        if (input.interact) {
            Pickup item = GetClosest();
            if (item != null)
            {
                if (Inventory.HoldingItem())
                {
                    Drop();
                }
                PickUp(item);
            }
        }
        else if (input.throwItem)
        {
            if (Inventory.HoldingItem())
            {
                GameObject item = handLocation.GetChild(0).gameObject;
                Drop();
                CustomPhysics.ThrowItem(item, player.throwStartDistance, player.throwForce);
                AddDamage(item, player.throwDamage);   
            }
        }
        if (Inventory.HoldingItem())
        {
            GameObject item = handLocation.GetChild(0).gameObject;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
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
    void Drop()
    {
        Transform item = handLocation.GetChild(0);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        item.GetComponent<Collider>().enabled = true;
        item.transform.localScale *= 2;
        item.parent = GameObject.Find("Objects").transform;
        item.AddComponent<Pickup>();
        ApplyLayerToChildren(item.gameObject, "Ground");
        Inventory.EmptyHand();
    }
    void PickUp(Pickup item)
    {
        item.transform.parent = handLocation;
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<Collider>().enabled = false;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale /= 2;
        ApplyLayerToChildren(item.gameObject, "Weapon");
        Inventory.ReplaceHand(item.gameObject);
        Destroy(item);
    }
    void ApplyLayerToChildren(GameObject item, string layername)
    {
        item.layer = LayerMask.NameToLayer(layername);
        for (int i = 0; i < item.transform.childCount; i++) {
            item.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layername);
        }
    }
    void AddDamage(GameObject item, float throwDamage)
    {
        Damage damage = item.GetComponent<Damage>();
        if (damage == null)
        {
            damage = item.AddComponent<Damage>();
        }
        damage.enabled = true;
        damage.damage = throwDamage;
        damage.thrown = true;
    }
}