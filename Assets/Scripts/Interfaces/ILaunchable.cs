using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILaunchable
{
    void Launch(Vector3 hitPoint, float force, float upForce);
}
