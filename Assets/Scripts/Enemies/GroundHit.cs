using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHit : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.transform.root == transform.root) return;
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
        if (this.TryGetComponentInParent(out Enemy enemy))
        {
            if (enemy.canSwitchInAir) enemy.inAir = false;
        }
        else Destroy(this); 
    }
}
