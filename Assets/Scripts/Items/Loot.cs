using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Loot : MonoBehaviour, IDamageable
{
    public enum Type
    {
        Upgrade,
        Addon
    }
    public Type type;
    [SerializeField] float health;
    Transform player;
    bool opened = false;
    
    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }
    public void Damaged(float amount, object collision, object origin)
    {
        if (origin is Gun || origin is Damage damage && (damage.thrown || damage.punch) && type == Type.Upgrade) {
            health -= amount; 
            if (health <= 0)
            {
                Killed();
            }
        }
        else
        {
            if (TryGetComponent(out Rigidbody rb))
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
    public void Killed()
    {
        if (type == Type.Upgrade)
        {
            StartCoroutine(WaitFrameThenOpen());
        }
        else if (type == Type.Addon)
        {
            Inventory.guns[0].addon = new Scope();
            Destroy(gameObject);
        }
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
