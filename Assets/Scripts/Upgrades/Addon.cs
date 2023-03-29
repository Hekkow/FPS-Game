using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    float hookSpeed = 20;
    float overshootYAxis = 10;
    Vector3 velocityToSet;
    Vector3 grapplePoint;
    GameObject player;
    Movement playerMovement;
    Rigidbody playerRigidbody;
    public override void Activate()
    {
        player = GameObject.Find("Player");
        playerMovement = player.GetComponent<Movement>();
        playerRigidbody = player.GetComponent<Rigidbody>();
        playerMovement.applyingGravity = false;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        
        
        if (Physics.SphereCast(ray, 0.01f, out RaycastHit hit, hookRange))
        {
            if (hit.rigidbody != null)
            {
                PullIn(hit); 
            }
            //else
            //{
            //    grapplePoint = hit.point;
            //    Grapple();
            //}
        }
    }
    //void Grapple()
    //{
    //    Vector3 direction = (grapplePoint - player.transform.position).normalized; 
    //}
    









    void PullIn(RaycastHit hit)
    {
        Rigidbody target = hit.rigidbody;
        Transform player = GameObject.Find("Player").transform;
        target.velocity = new Vector3(target.velocity.x, 0, target.velocity.z);
        float startTime = Time.time;
        float hookTime = hit.distance / (hookSpeed / (1f / 60f));
        if (target.TryGetComponent(out Enemy enemy)) StartCoroutine(enemy.DisableAgentCoroutine());
        target.AddForce((player.transform.position - target.transform.position).normalized * 1000, ForceMode.Acceleration);
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

