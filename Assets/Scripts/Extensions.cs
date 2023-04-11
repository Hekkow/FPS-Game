using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public static class Extensions
{
    public static Vector3 AddY(this Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z);
    }
    public static Vector3 SetY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }
    public static bool TryGetComponentInParent<T>(this Component self, out T component)
    {
        component = self.GetComponentInParent<T>();
        if (component == null)
            return false;
        return true;
    }
    public static bool HasComponentInParent<T>(this Component self)
    {
        T component = self.GetComponentInParent<T>();
        if (component == null)
            return false;
        return true;
    }
    public static T GetOrAdd<T>(this GameObject obj) where T : Component
    {
        if (obj.GetComponent<T>() != null) return obj.GetComponent<T>();
        else return obj.AddComponent<T>();
    }
    public static void MakePhysical(this GameObject obj, bool physical)
    {
        if (physical)
        {
            obj.GetComponent<Rigidbody>().isKinematic = false;
            obj.GetComponent<Collider>().enabled = true;
        }
        else
        {
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.GetComponent<Collider>().enabled = false;
        }
    }
    public static void ApplyLayerToChildren(this GameObject obj, string layerName)
    {
        obj.layer = LayerMask.NameToLayer(layerName);
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }
    public static T GetRandom<T>(this IList<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}