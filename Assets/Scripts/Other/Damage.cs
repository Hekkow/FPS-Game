using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Timeline;

public class Damage : MonoBehaviour
{
    public float knockback;
    public float damage;
    public bool thrown;
    public bool oneTime;
    // for non solid things like animations

    void OnTriggerEnter(Collider collision)
    {
        Health collisionObjectHealth = collision.gameObject.GetComponent<Health>();
        if (collisionObjectHealth == null) // if health doesn't exist in current object, check parent object
        {
            Transform collisionObjectHealthParent = collision.gameObject.transform.parent;
            if (collisionObjectHealthParent != null)
            {
                collisionObjectHealth = collision.gameObject.transform.parent.GetComponent<Health>();
            }
        }
        if (collisionObjectHealth != null)
        {
            collisionObjectHealth.Damage(damage);
        }
    }

    // for solid things like bullets

    void OnCollisionEnter(Collision collision)
    {
        Health collisionObjectHealth = collision.gameObject.GetComponent<Health>();
        if (collisionObjectHealth == null)
        {
            Transform collisionObjectHealthParent = collision.gameObject.transform.parent;
            if (collisionObjectHealthParent != null)
            {
                collisionObjectHealth = collision.gameObject.transform.parent.GetComponent<Health>();
            }
        }
        if (collisionObjectHealth != null)
        {
            
            if (thrown)
            {
                CustomPhysics.BounceUpAndBack(gameObject);
            }
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null && collisionObjectHealth.alive)
            {
                enemy.KnockBack(knockback);
                DamageNumbers(collision);
                HitMarker();
            }
            collisionObjectHealth.Damage(damage);
        }
        if (oneTime)
        {
            Destroy(this);
        }
    }
    void DamageNumbers(Collision collision)
    {
        TMP_Text damageNumbersText = Instantiate(Resources.Load<TMP_Text>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform);
        damageNumbersText.text = damage.ToString();
        DamageNumber dn = damageNumbersText.gameObject.AddComponent<DamageNumber>();
        dn.collision = collision;
    }
    void HitMarker()
    {
        GameObject.Find("Reticle").GetComponent<HitMarker>().Mark();
    }
}
