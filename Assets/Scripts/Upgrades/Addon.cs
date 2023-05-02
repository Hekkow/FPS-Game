using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
public class RocketLauncher : Addon
{
    float radius = 10;
    float force = 50;
    float upForce = 0.3f;
    public override void Activate()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
        {
            Collider[] colliders = Physics.OverlapSphere(hit.point, radius);
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out Rigidbody rb))
                {
                    if (collider.TryGetComponentInParent(out Enemy enemy))
                    {
                        StartCoroutine(enemy.DisableAgent());
                    }
                    rb.AddExplosionNoFalloff(hit.point, force, radius, ForceMode.Impulse); 
                }
                else if (collider.TryGetComponent(out Movement movement))
                {
                    movement.AddExtraForce(movement.transform.position.ExplosionVector(hit.point, force, radius));
                }
            }
        }
        //Debug.Log("ROCKET");
    }
}
public class Hook : Addon
{
    float baseForce = 1000;
    float enemyMultiplier = 20;
    public override void Activate()
    {
        Movement player = GameObject.Find("Player").GetComponent<Movement>();
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.SphereCast(ray, 0.01f, out RaycastHit hit, player.hookRange))
        { 
            if (hit.rigidbody != null && hit.transform.GetComponentInParent<Enemy>())
            {
                PullIn(hit, ray); 
            }
            else
            {
                player.Hook(hit);
            }
        }
    }
    void PullIn(RaycastHit hit, Ray ray)
    {
        Rigidbody target = hit.rigidbody;
        Transform player = GameObject.Find("Player").transform;
        target.velocity.SetY(0);
        float pullForce = baseForce; 
        if (target.TryGetComponentInParent(out Enemy enemy))
        {
            enemy.Ragdoll();
            //StartCoroutine(enemy.DisableAgentCoroutine());
            pullForce *= enemyMultiplier;
            //enemyTarget = enemy.transform;
            target = enemy.pelvis;
            //enemyTarget.position = player.position;

        }
        
        foreach (Rigidbody rb in enemy.GetComponentsInChildren<Rigidbody>())
        {

            //rb.AddExplosionForce(hit.distance* Mathf.Sqrt(3), hit.point + ray.direction * 2, 5, Mathf.Sqrt(hit.distance/10), ForceMode.Impulse);
        }

        //Debug.DrawRay(target.position, (player.transform.position - target.position).normalized * pullForce, Color.green, 10);
        //Vector3 velocity = (player.transform.position - target.position).normalized * pullForce;
        //if (velocity.y < 5) velocity.SetY(5);
        //target.AddForce(velocity, ForceMode.Acceleration);
        

    }
    IEnumerator EnemyHook(RaycastHit hit, Vector3 startingPosition)
    {
        float startTime = Time.time;
        float hookSpeed = 10;
        float hookTime = hit.distance / hookSpeed;
        while (Time.time - startTime < hookTime)
        {
            
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
        RocketLauncher,
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
            case AddonName.RocketLauncher:
                Inventory.guns[0].addon = Inventory.guns[0].gameObject.AddComponent<RocketLauncher>();
                break;
        }
    }
}

