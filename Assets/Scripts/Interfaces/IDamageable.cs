using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damaged(float amount, Vector3 hitPoint, Component origin);
    void Killed();
}
