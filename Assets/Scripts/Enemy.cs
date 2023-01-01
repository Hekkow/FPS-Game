using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask obstacles;
    [SerializeField] GameObject attackAnimation;

    [Header("Attack")]
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float animationTime;

    [Header("Vision")]
    [SerializeField] float viewRadiusBeforeDetected;
    [SerializeField] float viewRadiusAfterDetected;
    [SerializeField] float viewAngleBeforeDetected;
    [SerializeField] float viewAngleAfterDetected;
    [SerializeField] float detectionTime;

    [Header("Knockback")]
    [SerializeField] float knockedTime;
    [SerializeField] float ragdollMass;

    [HideInInspector] public bool knocked = false;

    GameObject instantiatedAnimation;
    TMP_Text healthText;
    Transform player;
    Rigidbody rb;
    Health health;
    new Camera camera;

    float viewRadius;
    float viewAngle;
    float timeDetected;

    bool playerDetected = false;
    bool detectedOnce = false;

    bool alreadyAttacked = false;

    IEnumerator knockCoroutine;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        healthText = transform.GetComponentInChildren<TMP_Text>();
        viewRadius = viewRadiusBeforeDetected;
        viewAngle = viewAngleBeforeDetected;
        camera = Camera.main;
    }

    void Update()
    {
        if (!health.alive)
        {
            Died();
            return;
        }
        Vision();
        if (!knocked)
        {
            if (Physics.CheckSphere(transform.position, attackRange, playerMask)) // if player's in attack range
            {
                AttackPlayer();
            }
        }
        UpdateHealthBar();
        
    }
    
    void Vision()
    {
        Vector3 playerTarget = (player.transform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, playerTarget) < viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToTarget < viewRadius)
            {
                if (Physics.Raycast(transform.position, playerTarget, distanceToTarget, obstacles) == false)
                {
                    playerDetected = true;
                    detectedOnce = true;
                    timeDetected = Time.time;
                }
                else playerDetected = false;
            }
            else playerDetected = false;
        }
        else playerDetected = false;
        if (!playerDetected && Time.time - timeDetected <= detectionTime && detectedOnce) playerDetected = true;
    }
    void AttackPlayer()
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (!alreadyAttacked)
        {
            instantiatedAnimation = Instantiate(attackAnimation, transform.position, new Quaternion(1, 0, 0, 1));
            Helper.AddDamage(instantiatedAnimation, attackDamage, attackDamage, false);
            instantiatedAnimation.transform.SetParent(transform);
            StartCoroutine(AttackAnimationTime());
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    void ResetAttack()
    {
        alreadyAttacked = false;
    }
    IEnumerator AttackAnimationTime()
    {

        yield return new WaitForSeconds(animationTime);
        Destroy(instantiatedAnimation);
    }
    void UpdateHealthBar()
    {
        healthText.transform.LookAt(camera.transform);
        healthText.transform.rotation = Quaternion.LookRotation(camera.transform.forward);
        healthText.text = Helper.HealthToHashtags(health);
    }
    void Died()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;
        rb.mass = ragdollMass; // ragbody haha
        
        healthText.enabled = false;
        transform.Rotate(0, 0, 90); // flop haha
        gameObject.AddComponent<Pickup>();
        Destroy(instantiatedAnimation);
        Destroy(this);
    }
    public void KnockBack(float knockbackAmount)
    {
        knockCoroutine = KnockBackCoroutine(knockbackAmount);
        StopCoroutine(knockCoroutine);
        StartCoroutine(knockCoroutine);
    }
    IEnumerator KnockBackCoroutine(float knockbackAmount)
    {
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        knocked = true;
        CustomPhysics.KnockBack(gameObject, knockbackAmount);
        yield return new WaitForSeconds(knockedTime);
        rb.velocity = Vector3.zero;
        knocked = false;
    }
}
