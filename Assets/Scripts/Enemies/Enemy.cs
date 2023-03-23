using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Objects")]
    [SerializeField] protected LayerMask playerMask;
    [SerializeField] protected LayerMask obstacles;
    [SerializeField] protected Player player;

    [Header("Vision")]
    [SerializeField] float viewRadiusBeforeDetected;
    [SerializeField] float viewRadiusAfterDetected;
    [SerializeField] float viewAngleBeforeDetected;
    [SerializeField] float viewAngleAfterDetected;
    [SerializeField] float detectionTime;

    [Header("Knockback")]
    [SerializeField] protected float ragdollMass;
    [SerializeField] protected float knockedTime;


    [Header("Pathfinding")]
    [SerializeField] protected float walkAnimationSpeed;

    [Header("Other")]
    [SerializeField] float lookAroundSpeed;

    protected NavMeshAgent agent;
    protected GameObject target;
    protected Health health;
    protected Rigidbody rb;
    protected Animator animator;
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
    protected bool agentDisabled = false;
    protected float fallingVelocity;
    protected virtual void Awake()
    {
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthBar = GetComponent<HealthBar>();
    }
    protected virtual void Start()
    {
        viewRadius = viewRadiusBeforeDetected;
        viewAngle = viewAngleBeforeDetected;
        lastSeenLocation = transform.position;
        StartCoroutine(Vision());
    }
    protected virtual void Update() { }
    protected virtual void FixedUpdate()
    {
        fallingVelocity = rb.velocity.y;
    }
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
            yield return new WaitForSeconds(0.1f);
        }
    }
    public virtual void Damaged(float amount, object collision, object origin)
    {
        if (health.alive && amount >= 1)
        {
            health.Damage(amount);
            if (collision is Collision)
            {
                StartCoroutine(KnockbackCoroutine((Collision)collision, origin));
                Instantiate(Resources.Load<DamageNumber>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform).Init(amount, (Collision)collision);
            }
            else if (collision is Collider)
            {
                StartCoroutine(KnockbackCoroutine());
                Instantiate(Resources.Load<DamageNumber>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform).Init(amount, (Collider)collision);
            }
            else if (collision is null)
            { 
                Instantiate(Resources.Load<DamageNumber>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform).Init(amount, transform);
            }
            if (!health.alive && canDie) Killed();
        }
        if (!playerDetected) StartCoroutine(LookAround());
    }
    public virtual void Killed()
    {
        canDie = false;
        healthBar.Disable();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;
        rb.mass = ragdollMass;
        StartCoroutine(IdleThenDestroy());
        Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeLoot"), transform.position + new Vector3(0, 2, 0), Quaternion.identity);
    }
    IEnumerator IdleThenDestroy()
    {
        animator.CrossFade("Idle", 0, 0);
        gameObject.AddComponent<Pickup>();
        yield return new WaitForEndOfFrame();
        transform.Rotate(0, 0, 90);
        Destroy(animator);
        Destroy(agent);
        Destroy(this);
    }
    public virtual IEnumerator KnockbackCoroutine()
    {
        knocked = true;
        agent.enabled = false;
        Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0.01f, Camera.main.transform.forward.z).normalized;
        rb.AddForce(direction * Inventory.guns[0].bulletKnockback / 2, ForceMode.Impulse);
        yield return new WaitForSeconds(knockedTime);
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height / 2 + 0.3f));
        knocked = false;
        if (!agentDisabled) agent.enabled = true;
    }
    public virtual IEnumerator KnockbackCoroutine(Collision collision, object origin)
    {
        knocked = true;
        agent.enabled = false;
        
        if (origin is Damage damage && damage.thrown)
        {
            Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0.01f, Camera.main.transform.forward.z).normalized;
            rb.AddForce(direction * 1000, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(knockedTime);
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height / 2 + 0.3f));
        knocked = false;
        agent.enabled = true;
    }
    public virtual IEnumerator DisableAgentCoroutine()
    {
        agentDisabled = true;
        animationLocked = true;
        agent.enabled = false;
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !health.alive || Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height/2+0.1f));
        agentDisabled = false;
        if (health.alive && !knocked)
        {
            agent.enabled = true;
            animationLocked = false;
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

}
