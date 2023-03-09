using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damaged(float amount, object collision, object origin);
    void Killed();
}
