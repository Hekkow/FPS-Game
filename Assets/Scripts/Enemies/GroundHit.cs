using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHit : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.transform.root == transform.root) return;
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
        Enemy enemy = GetComponentInParent<Enemy>();
        if (enemy.canSwitchInAir) enemy.inAir = false;
    }
}
