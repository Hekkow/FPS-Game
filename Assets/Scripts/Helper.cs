using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

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
    public static void AddDamage(GameObject thing, float damage, float knockback, bool thrown)
    {
        Damage dmg = thing.AddComponent<Damage>();
        if (dmg == null)
        {
            dmg = thing.AddComponent<Damage>();
        }
        dmg.enabled = true;
        dmg.damage = damage;
        dmg.knockback = knockback;
        dmg.thrown = true;
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
    public static void MakeChildrenVisible(GameObject parent, bool on)
    {
        Renderer parentRenderer = parent.GetComponent<Renderer>();
        if (parentRenderer != null)
        {
            parentRenderer.enabled = on;
        }
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Renderer childRenderer = parent.transform.GetChild(i).GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.enabled = on;
            }
        }
    }
}
