using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static Damage AddDamage(GameObject thing, float damage, float knockback, bool thrown, bool oneTime, bool punch)
    {
        Damage dmg = thing.GetComponent<Damage>();
        if (dmg == null)
        {
            dmg = thing.AddComponent<Damage>();
        }
        dmg.enabled = true;
        dmg.damage = damage;
        dmg.knockback = knockback;
        dmg.thrown = thrown;
        dmg.oneTime = oneTime;
        dmg.punch = punch;
        dmg.environment = false;
        return dmg;
    }
}
