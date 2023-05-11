using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy : MonoBehaviour, ILaunchable
{
    Rigidbody pelvis;
    public void Init(Rigidbody pelvis)
    {
        this.pelvis = pelvis;
    }
    public void Launch(Vector3 hitPoint, float force, float upForce)
    {
        pelvis.AddExplosionNoFalloff(hitPoint, force * 2, upForce * 2, ForceMode.VelocityChange);
    }
}
