using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Decapitate : MonoBehaviour, IDamageable
{
    bool killed = false;
    public void Damaged(float amount, object collision, object origin)
    {
        if (!killed)
        {
            StartCoroutine(Unparent());
            killed = true;
        }
    }
    public void Killed() { }
    IEnumerator Unparent()
    {
        Animator animator = GetComponentInParent<Animator>();
        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;

        gameObject.transform.parent = null;
        float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        int nameHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        yield return new WaitForEndOfFrame();
        try { animator.Rebind(); } catch { animator.Rebind(); }
        animator.Play(nameHash, 0, time);
        Destroy(GetComponent<CharacterJoint>());
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
        }
        Destroy(this);
    }
}
