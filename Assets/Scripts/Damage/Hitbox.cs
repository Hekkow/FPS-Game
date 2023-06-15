using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : Damage
{
    void OnTriggerEnter(Collider collider)
    {
        if (didDamage) return;
        if (!collider.gameObject.TryGetComponent(out IDamageable damageable)) return;
        if (collider.gameObject.transform.root == transform.root) return;
        damageable.Damaged(damage, collider.transform.position, this);
        didDamage = true;
        Destroy(this);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (didDamage) return;
        if (!collision.gameObject.TryGetComponent(out IDamageable damageable)) return;
        if (collision.gameObject.transform.root == transform.root) return;
        damageable.Damaged(damage, collision.GetContact(0).point, this);
        didDamage = true;
        Destroy(this);
    }
    public void Init(float damage)
    {
        this.damage = damage;
    }
}
