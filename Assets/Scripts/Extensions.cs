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
    public static float HorizontalMagnitude(this Vector3 v)
    {
        return Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.z, 2));
    }
    public static float MakeMin(this float f, float x)
    {
        if (f < x) f = x;
        return f;
    }
    public static float MakeMax(this float f, float x)
    {
        if (f > x) f = x;
        return f;
    }
    public static bool TryGetComponentInParent<T>(this GameObject self, out T component)
    {
        component = self.GetComponentInParent<T>();
        if (component == null)
            return false;
        return true;
    }
    public static bool TryGetComponentInParent<T>(this Component self, out T component)
    {
        component = self.GetComponentInParent<T>();
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
    public static RaycastHit? ClosestHit(this Transform transform, out int right, int amount, float startingAngle, float range, float forwardAmount)
    {
        right = 0;
        List<RaycastHit> hits = new List<RaycastHit>();
        for (float i = startingAngle; i < 360 + startingAngle; i += (360 / amount))
        {
            Vector3 direction = Quaternion.Euler(0, i, 0) * transform.right;
            Debug.DrawRay(transform.position, direction * range, Color.green, 0.1f);
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, range)) {
                hits.Add(hit);
                right = (int)(i / (360/amount))+1;
            }
        }
        if (hits.Count == 0) return null;
        RaycastHit closest = hits[0];
        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].distance < closest.distance) {
                closest = hits[i];
            }
        }
        return closest;
        
    }
    public static void AddExplosionNoFalloff(this Rigidbody rb, Vector3 hitPoint, float force, float upForce, ForceMode forceMode)
    {
        Vector3 direction = rb.position - hitPoint;
        rb.AddForce((direction.normalized * force).AddY(upForce), forceMode); 
    }
    public static Vector3 ExplosionVector(this Vector3 position, Vector3 hitPoint, float force, float upForce)
    {
        Vector3 direction = position - hitPoint;
        return (direction.normalized * force).AddY(upForce);
    }
    public static void Explode(this Vector3 position, float explosionRadius, float force, float upForce, float damage)
    {
        Collider[] colliders = Physics.OverlapSphere(position, explosionRadius, ~LayerMask.GetMask("Bullet"));
        List<ILaunchable> launchables = new List<ILaunchable>();
        List<IDamageable> damageables = new List<IDamageable>();

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponentInParent(out ILaunchable launchable) && !launchables.Contains(launchable))
            {
                launchables.Add(launchable);
                launchable.Launch(position, force, upForce);
            }
            if (collider.TryGetComponentInParent(out IDamageable damageable) && !damageables.Contains(damageable))
            {
                damageables.Add(damageable);
                damageable.Damaged(damage, collider.transform.position, null);
            }
        }
    }
    public static void DestroyAllChildren(this Transform transform)
    {
        while (transform.childCount > 0)
        {
            Object.DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}