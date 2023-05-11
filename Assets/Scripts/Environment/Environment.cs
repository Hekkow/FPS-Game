using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour, ILaunchable
{
    public Rigidbody rb;

    public void Launch(Vector3 hitPoint, float force, float upForce)
    {
        rb.AddExplosionNoFalloff(hitPoint, force, upForce * 15, ForceMode.Impulse);
    }

    void Reset()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        gameObject.AddComponent<EnvironmentDamage>();
        gameObject.AddComponent<Pickup>();
    }
    
}
