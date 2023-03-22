using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static string HealthToHashtags(Health health)
    {
        string hashtags = "";
        int currentHealth = Mathf.CeilToInt(health.GetHealth() / 10);
        int maxHealth = Mathf.CeilToInt(health.GetMaxHealth() / 10) - currentHealth;
        for (int i = 0; i < currentHealth; i++)
        {
            hashtags += "#";
        }
        for (int i = 0; i < maxHealth; i++)
        {
            hashtags += "-";
        }
        return hashtags;
    }
    public static void ToggleComponent<T>(GameObject thing, bool enabled) where T : Behaviour
    {
        T componentType = thing.GetComponent<T>();
        if (componentType != null)
        {
            componentType.enabled = false;
            if (enabled)
            {
                componentType.enabled = true;
            }
        }
    }
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
    public static void ApplyLayerToChildren(GameObject thing, string layername)
    {
        thing.layer = LayerMask.NameToLayer(layername);
        for (int i = 0; i < thing.transform.childCount; i++)
        {
            thing.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layername);
        }
    }
    public static void MakePhysical(GameObject thing, bool physical)
    {
        if (physical)
        {
            thing.GetComponent<Rigidbody>().isKinematic = false;
            thing.GetComponent<Collider>().enabled = true;
        }
        else
        {
            thing.GetComponent<Rigidbody>().isKinematic = true;
            thing.GetComponent<Collider>().enabled = false;
        }
    }
    public static T GetOrAdd<T>(GameObject thing) where T : Component
    {
        if (thing.GetComponent<T>() != null) return thing.GetComponent<T>();
        else return thing.AddComponent<T>();
    }
}
