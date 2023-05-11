using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : Damage
{
    void OnTriggerEnter(Collider collision)
    {
        if (didDamage) return;
        if (!collision.gameObject.TryGetComponent(out IDamageable damageable)) return;
        if (collision.gameObject.transform.root == transform.root) return;
        damageable.Damaged(damage, collision, this);
        didDamage = true;
        Destroy(this);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (didDamage) return;
        if (!collision.gameObject.TryGetComponent(out IDamageable damageable)) return;
        if (collision.gameObject.transform.root == transform.root) return;
        damageable.Damaged(damage, collision, this);
        didDamage = true;
        Destroy(this);
    }
    public void Init(float damage)
    {
        this.damage = damage;
    }
}
