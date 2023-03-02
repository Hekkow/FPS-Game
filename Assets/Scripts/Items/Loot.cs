using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour, IDamageable
{
    Transform player;
    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }
    public void Damaged(float amount, object collision)
    {
        GameObject upgradesBox = Instantiate(Resources.Load<GameObject>("Prefabs/Upgrade"), transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        Vector3 aPlaneZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Vector3 bPlaneZ = Vector3.ProjectOnPlane(player.forward, Vector3.up);
        Quaternion quaternion = Quaternion.FromToRotation(aPlaneZ, bPlaneZ);
        upgradesBox.transform.rotation = quaternion;
        Destroy(gameObject);
    }
}
