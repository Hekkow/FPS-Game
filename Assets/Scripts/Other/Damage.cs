using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Timeline;

public class Damage : MonoBehaviour
{
    public float knockback = 0;
    public float damage = 0;
    public bool thrown = false;
    public bool oneTime = false;
    public bool environment = true;
    public float velocityMagnitude;

    bool didDamage = false;

    // for non solid things like animations
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        velocityMagnitude = rb.velocity.magnitude;
    }

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
        if (!didDamage)
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
                if (environment)
                {
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {

                        //Debug.Log(velocityMagnitude * rb.mass / 10);
                        //Debug.Log(gameObject.name + " velocity: " + velocityMagnitude + " mass: " + rb.mass + " / 10 " + (velocityMagnitude * rb.mass / 10));
                        damage = velocityMagnitude * rb.mass / 10;
                    }
                }
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
                Player player = collision.gameObject.GetComponent<Player>();
                if (player == null)
                {
                    collisionObjectHealth.Damage(damage);
                    StartCoroutine(ResetDidDamage());

                }
            }
            if (oneTime)
            {
                Destroy(this);
            }
        }
    }
    IEnumerator ResetDidDamage()
    {
        didDamage = true;
        yield return new WaitForSeconds(0.5f);
        didDamage = false;
    }
    void DamageNumbers(Collision collision)
    {
        TMP_Text damageNumbersText = Instantiate(Resources.Load<TMP_Text>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform);
        damageNumbersText.text = Mathf.RoundToInt(damage).ToString();
        DamageNumber dn = damageNumbersText.gameObject.AddComponent<DamageNumber>();
        dn.collision = collision;
    }
    void HitMarker()
    {
        GameObject.Find("Reticle").GetComponent<HitMarker>().Mark();
    }
}
