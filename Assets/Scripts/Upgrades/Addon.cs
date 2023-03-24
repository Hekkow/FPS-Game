using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Addon : MonoBehaviour
{
    protected float cooldown;
    public abstract void Activate();
}
public class Scope : Addon
{
    public override void Activate()
    {
        Debug.Log("TEST");
    }
}
public class Hook : Addon
{
    float hookRange = 40;
    bool canActivate = true;
    float hookSpeed = 1f;
    Hook()
    {
        cooldown = 5f;
    }
    public override void Activate()
    {
        if (!canActivate) return;
        //canActivate = false;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hookRange, Color.green, 10);
        
        if (Physics.SphereCast(ray, 0.01f, out RaycastHit hit, hookRange))
        {
            if (hit.rigidbody != null)
            {
                StartCoroutine(PullIn(hit)); 
            }
            else
            {
                StartCoroutine(MoveTowards(hit));
            }
        }
    }
    IEnumerator MoveTowards(RaycastHit hit)
    {
        Rigidbody rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float startTime = Time.time;
        float hookTime = hit.distance / (hookSpeed / (1f / 60f));
        rb.GetComponent<Movement>().applyingGravity = false;
        while (Time.time - startTime < hookTime)
        {
            Vector3 destination = Vector3.MoveTowards(rb.transform.position, hit.point, hookSpeed);
            rb.MovePosition(destination);
            yield return new WaitForFixedUpdate();
        }
        //StartCoroutine(RenableHook());
        rb.GetComponent<Movement>().applyingGravity = true;

        //rb.AddForce((hit.point - position)*hookForce, ForceMode.Force);
    }
    IEnumerator RenableHook()
    {
        yield return new WaitForSeconds(cooldown);
        canActivate = true;
    }
    IEnumerator PullIn(RaycastHit hit)
    {
        Rigidbody target = hit.rigidbody;
        Transform player = GameObject.Find("Player").transform;
        target.velocity = new Vector3(target.velocity.x, 0, target.velocity.z);
        float startTime = Time.time;
        float hookTime = hit.distance / (hookSpeed / (1f / 60f));
        if (target.TryGetComponent(out Enemy enemy))
        {
            StartCoroutine(enemy.DisableAgentCoroutine());
        }
        while (Time.time - startTime < hookTime)
        {
            Vector3 destination = Vector3.MoveTowards(target.transform.position, player.transform.position, hookSpeed);
            target.MovePosition(destination);
            yield return new WaitForFixedUpdate();
        } 
    }
}
public class GrenadeLauncher : Addon
{
    public override void Activate()
    {
        throw new System.NotImplementedException();
    }
}
public static class AddonManager
{
    public enum AddonName
    {
        Scope,
        Hook,
    }
    public static void AddAddon(AddonName name)
    {
        switch (name)
        {
            case AddonName.Scope:
                Inventory.guns[0].addon = Inventory.guns[0].gameObject.AddComponent<Scope>();
                break;
            case AddonName.Hook:
                Inventory.guns[0].addon = Inventory.guns[0].gameObject.AddComponent<Hook>();
                break;
        }
    }
}

