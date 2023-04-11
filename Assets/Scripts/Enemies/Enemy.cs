using DG.Tweening;
using System.Collections;
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
    [SerializeField] protected Rig rig;
    [SerializeField] public Rigidbody pelvis;
    [SerializeField] protected Transform leftFoot;
    [SerializeField] protected Transform rightFoot;


    [Header("Vision")]
    [SerializeField] float viewRadiusBeforeDetected;
    [SerializeField] float viewRadiusAfterDetected;
    [SerializeField] float viewAngleBeforeDetected;
    [SerializeField] float viewAngleAfterDetected;
    [SerializeField] float detectionTime;
    [SerializeField] float visionInterval;

    [Header("Knockback")]
    [SerializeField] protected float ragdollMass;
    [SerializeField] protected float knockedTime;

    [Header("Pathfinding")]
    [SerializeField] protected float walkAnimationSpeed;

    [Header("Disable Agent")]
    [SerializeField] float disableTime;

    [Header("Flinch")]
    [SerializeField] protected Color blinkColor;
    [SerializeField] protected Color blinkEmissionColor;
    [SerializeField] protected float blinkEmissionIntensity;
    [SerializeField] protected float blinkDuration;
    protected float blinkTimer;

    [Header("Death")]
    [SerializeField] protected float deathForce;
    [SerializeField] protected float lootSpawnOffset;

    [Header("Other")]
    [SerializeField] float lookAroundSpeed;

    protected NavMeshAgent agent;
    protected SkinnedMeshRenderer skin;
    protected Rigidbody rb;
    protected Animator animator;
    protected Health health;
    HealthBar healthBar;

    float viewRadius;
    float viewAngle;
    float timeDetected;
    protected bool playerDetected = false;
    bool detectedOnce = false;
    public bool animationLocked = false;
    protected bool canDie = true;
    protected Vector3 lastSeenLocation;
    protected bool knocked = false;
    protected float fallingVelocity;
    Vector3 deathDirection;
    Coroutine blinkCoroutine;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthBar = GetComponent<HealthBar>();
        health = GetComponent<Health>();
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    protected virtual void Start()
    {
        viewRadius = viewRadiusBeforeDetected;
        viewAngle = viewAngleBeforeDetected;
        lastSeenLocation = transform.position;
        StartCoroutine(Vision());
    }
    protected virtual void Update() { }
    //protected virtual void FixedUpdate()
    //{
    //    fallingVelocity = rb.velocity.y;
    //}
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
        DamageNumber dn = Instantiate(Resources.Load<DamageNumber>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform);
        if (collision is Collision)
        {
            dn.Init(amount, (Collision)collision);
        }
        else if (collision is Collider)
        {
            dn.Init(amount, (Collider)collision);
        }
        else if (collision is null)
        {
            dn.Init(amount, transform);
        }
        
        if (health.alive && amount >= 1)
        {
            
            health.health -= amount;

            if (health.health <= 0 && canDie)
            { 
                deathDirection = (transform.position - ((Component)origin).gameObject.transform.position).normalized * deathForce;
                if (deathDirection.y < 50) deathDirection.y = 50;
                Killed();
            }
            else
            {
                if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
                blinkCoroutine = StartCoroutine(Blink());
            }
        }
        //if (!playerDetected) StartCoroutine(LookAround());
    }
    public virtual void Killed()
    {
        health.alive = false;
        canDie = false;
        agent.enabled = false;
        Ragdoll();
        pelvis.AddForce(deathDirection, ForceMode.VelocityChange);
        Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeLoot"), transform.position.AddY(lootSpawnOffset), Quaternion.identity);
        healthBar.Disable();
        Destroy(GetComponent<RigBuilder>());
        Destroy(animator); 
        Destroy(agent);
        StartCoroutine(AddDeadEnemy());
        Destroy(this);
    }
    public virtual void Ragdoll()
    {
        animator.enabled = false;
        agent.enabled = false;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.AddComponent<Pickup>();
        }
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.isTrigger = false;
        }
    }
    public virtual void UnRagdoll()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.isTrigger = true;
        }
    }
    public virtual IEnumerator DisableAgentCoroutine()
    {
        animator.enabled = false;
        animationLocked = true;
        agent.enabled = false;
        yield return new WaitForSeconds(disableTime);
        //yield return new WaitUntil(() => health.alive && !Physics.Raycast(leftFoot.position, -leftFoot.up, 0.7031503f + 0.1f) || !Physics.Raycast(rightFoot.position, -rightFoot.up, 0.7031503f + 0.1f));
        yield return new WaitUntil(() => !health.alive || Physics.Raycast(transform.position, -transform.up, agent.height / 2 + 0.1f));
        if (health.alive && !knocked)
        {
            agent.enabled = true;
            animationLocked = false;
            animator.enabled = true;
            agent.nextPosition = pelvis.position;
        }
    }
    public virtual IEnumerator LookAround()
    {
        while (transform.localRotation.y < 360 && !playerDetected)
        {
            transform.Rotate(0, lookAroundSpeed * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }
    }
    void OnCollisionEnter()
    {
        if (fallingVelocity < -10)
        {
            Damaged(fallingVelocity * -2.5f, null, this);
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
    protected virtual IEnumerator Blink()
    {
        skin.material.EnableKeyword("_EMISSION");
        skin.material.SetColor("_BaseColor", blinkColor);
        skin.material.SetColor("_EmissionColor", blinkEmissionColor * blinkEmissionIntensity);
        yield return new WaitForSeconds(blinkDuration / 2);
        skin.material.SetColor("_BaseColor", Color.white);
        skin.material.SetColor("_EmissionColor", Color.black);
    }
    protected virtual IEnumerator AddDeadEnemy()
    {
        yield return new WaitForEndOfFrame();
        gameObject.AddComponent<DeadEnemy>();
    }
}
