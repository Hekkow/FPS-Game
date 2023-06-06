using DG.Tweening;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Objects")]
    [SerializeField] protected LayerMask playerMask;
    [SerializeField] protected LayerMask obstacles;
    [SerializeField] protected Transform target;
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected Animator animator;
    protected Health health;
    HealthBar healthBar;

    [Header("Vision")]
    [SerializeField] float viewRadiusBeforeDetected;
    [SerializeField] float viewRadiusAfterDetected;
    [SerializeField] float viewAngleBeforeDetected;
    [SerializeField] float viewAngleAfterDetected;
    [SerializeField] float detectionTime;
    [SerializeField] float visionInterval;
    float viewRadius;
    float viewAngle;
    float timeDetected;
    protected bool playerDetected = false;
    bool detectedOnce = false;
    protected Vector3 lastSeenLocation;


    [Header("Death")]
    [SerializeField] protected float lootSpawnOffset;
    protected bool canDie = true;

    [Header("States")]
    public bool animationLocked = false;

    [Header("Other")]
    [SerializeField] float lookAroundSpeed;
    [SerializeField] protected float walkAnimationSpeed;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthBar = GetComponent<HealthBar>();
        health = GetComponent<Health>();
        viewRadius = viewRadiusBeforeDetected;
        viewAngle = viewAngleBeforeDetected;
        lastSeenLocation = transform.position;
    }
    protected virtual void Start()
    {
        StartCoroutine(Vision());
    }
    protected virtual void Update() { }
    IEnumerator Vision()
    {
        while (health.alive)
        {
            Vector3 playerTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, playerTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (distanceToTarget < viewRadius)
                {
                    if (Physics.Raycast(transform.position, playerTarget, distanceToTarget, obstacles) == false)
                    {
                        playerDetected = true;
                        detectedOnce = true;
                        timeDetected = Time.time;
                        lastSeenLocation = target.transform.position;
                    }
                    else playerDetected = false;
                }
                else playerDetected = false;
            }
            else playerDetected = false;
            if (!playerDetected && Time.time - timeDetected <= detectionTime && detectedOnce) playerDetected = true;
            yield return new WaitForSeconds(visionInterval);
        }
    }
    public virtual void Damaged(float amount, object collision, object origin)
    {
        
        if (health.alive && amount >= 1)
        {
            health.health -= amount;
            if (health.health <= 0 && canDie)
            {
                Killed();
            }
        }
        if (!playerDetected) StartCoroutine(LookAround());
    }
    public virtual void Killed()
    {
        health.alive = false;
        canDie = false;
        agent.enabled = false;
        rb.isKinematic = true;
        healthBar.Disable();
        Destroy(rb);
        Destroy(animator);
        Destroy(agent);
    }
    public virtual IEnumerator LookAround()
    {
        while (transform.localRotation.y < 360 && !playerDetected)
        {
            transform.Rotate(0, lookAroundSpeed * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }
    }
    protected virtual void SetDestinationTarget()
    {
        if (!agent.SetDestination(target.transform.position))
        {
            if (Physics.Raycast(target.transform.position, -target.transform.up, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
