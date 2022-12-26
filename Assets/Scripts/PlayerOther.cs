using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerOther : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    InputManager input;
    Transform handLocation;

    Player player;

    void Awake()
    {
        input = gameManager.GetComponent<InputManager>();
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
        else if (input.punch)
        {
            StartCoroutine(Punch());
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
    IEnumerator Punch()
    {
        Vector3 initialPosition = handLocation.localPosition;
        Quaternion initialRotation = handLocation.localRotation;
        handLocation.localPosition = new Vector3(0, 0, 2);
        handLocation.localRotation = Quaternion.Euler(0, -110, -40);
        Damage fist = handLocation.GetChild(0).AddComponent<Damage>();
        fist.GetComponent<Collider>().enabled = true;

        fist.damage = 99;
        yield return new WaitForSeconds(0.1f);
        fist.GetComponent<Collider>().enabled = false;

        Destroy(fist);
        handLocation.localPosition = initialPosition;
        handLocation.localRotation = initialRotation;
    }

}