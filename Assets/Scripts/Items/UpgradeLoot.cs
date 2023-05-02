using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeLoot : Loot
{
    Transform player;
    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }
    public override void Damaged(float amount, object collision, object origin)
    {
        base.Damaged(amount, collision, origin);
        //if (origin is Gun || origin is Damage damage && (damage.thrown || damage.punch))
        //{
        //    base.Damaged(amount, collision, origin);
        //}
    }
    public override void Killed()
    {
        StartCoroutine(WaitFrameThenOpen());
    }
    IEnumerator WaitFrameThenOpen()
    {
        yield return new WaitForEndOfFrame();
        if (!opened)
        {
            GameObject upgradesBox = Instantiate(Resources.Load<GameObject>("Prefabs/Upgrade"), transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            Vector3 aPlaneZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            Vector3 bPlaneZ = Vector3.ProjectOnPlane(player.forward, Vector3.up);
            Quaternion quaternion = Quaternion.FromToRotation(aPlaneZ, bPlaneZ);
            upgradesBox.transform.rotation = quaternion;
            Destroy(gameObject);
        }
        opened = true;
    }
}
