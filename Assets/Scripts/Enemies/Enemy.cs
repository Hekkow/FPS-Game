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
    protected NavMeshAgent agent;
    protected SkinnedMeshRenderer skin;
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

    [Header("Knockback")]
    [SerializeField] protected float ragdollMass;
    [SerializeField] protected float knockedTime;

    [Header("Pathfinding")]
    [SerializeField] protected float walkAnimationSpeed;

    [Header("Disable Agent")]
    [SerializeField] float disableTime;
    [SerializeField] float height;

    [Header("Flinch")]
    [SerializeField] protected Color blinkColor;
    [SerializeField] protected Color blinkEmissionColor;
    [SerializeField] protected float blinkEmissionIntensity;
    [SerializeField] protected float blinkDuration;
    protected float blinkTimer;
    protected bool knocked = false;
    Coroutine blinkCoroutine;

    [Header("Death")]
    [SerializeField] protected float deathForce;
    [SerializeField] protected float lootSpawnOffset;
    protected bool canDie = true;
    Collider deathCollider;
    Vector3 deathDirection;

    [Header("States")]
    public bool animationLocked = false;

    [Header("Other")]
    [SerializeField] float lookAroundSpeed;

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
        if (collision == null) dn.Init(amount, transform);
        else if (collision is Collider) dn.Init(amount, (Collider)collision);
        else if (collision is Collision) dn.Init(amount, (Collision)collision); 

        if (health.alive && amount >= 1)
        {
            health.health -= amount;

            if (health.health <= 0 && canDie)
            { 
                deathDirection = (transform.position - ((Component)origin).gameObject.transform.position).normalized * deathForce;
                if (deathDirection.y < 50) deathDirection.y = 50;
                if (collision != null)
                {
                    if (collision is Collider) deathCollider = (Collider)collision;
                    else if (collision is Collision) deathCollider = ((Collision)collision).collider;
                }
                Killed();
            }
            else
            {
                if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
                blinkCoroutine = StartCoroutine(Blink());
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
        Ragdoll();
        if (deathCollider == null) rb.AddForce(deathDirection, ForceMode.VelocityChange);
        else deathCollider.GetComponentInParent<Rigidbody>().AddForce(deathDirection, ForceMode.VelocityChange);

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
    public virtual IEnumerator DisableAgent()
    {
        animator.enabled = false;
        animationLocked = true;
        agent.enabled = false;
        rb.isKinematic = false;
        RaycastHit hit = default;
        yield return new WaitForSeconds(disableTime);
        yield return new WaitUntil(() => !health.alive || Physics.Raycast(transform.position, -transform.up, out hit, height));
        if (health.alive && !knocked)
        {
            agent.enabled = true;
            animationLocked = false;
            animator.enabled = true;
            
            if (rb.velocity.y < -10)
            { 
                if (hit.collider != null) Damaged(rb.velocity.y * -2.5f, null, hit.collider);
            }
            rb.isKinematic = true;
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
