using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Objects")]
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask obstacles;
    [SerializeField] Player player;

    [Header("Attack")] 
    [SerializeField] float attackRange;

    [Header("Vision")]
    [SerializeField] float viewRadiusBeforeDetected;
    [SerializeField] float viewRadiusAfterDetected;
    [SerializeField] float viewAngleBeforeDetected;
    [SerializeField] float viewAngleAfterDetected;
    [SerializeField] float detectionTime;

    [Header("Knockback")]
    [SerializeField] float ragdollMass;

    [Header("Pathfinding")]
    [SerializeField] bool pathfinding;
    [SerializeField] float distanceHeightMultiplier;
    [SerializeField] float tinyBitOfExtraHeight;
    [SerializeField] float enemyHeight;
    [SerializeField] float groundHeight;
    [SerializeField] float timeBeforeGroundCheck;
    [SerializeField] float extraDownwardsDistance;
    [Range(0,2)]
    [SerializeField] float walkSpeed;
    enum AnimationState
    {
        Idle,
        Punch,
        Walk
    }
    Animator animator;
    TMP_Text healthText;
    Transform target;
    Rigidbody rb;
    Health health;
    NavMeshAgent agent;
    AnimationState currentState;
    float viewRadius;
    float viewAngle;
    float timeDetected;
    bool playerDetected = false;
    bool detectedOnce = false;
    float fallingVelocity = 0;
    bool animationLocked = false;
    bool canDie = true;
    Vector3 lastSeenLocation;
    [SerializeField] GameObject rightArm;
    private void Start()
    {
        target = GameObject.Find("Player").transform;
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        healthText = transform.GetComponentInChildren<TMP_Text>();
        viewRadius = viewRadiusBeforeDetected;
        viewAngle = viewAngleBeforeDetected;
        agent = GetComponent<NavMeshAgent>();
        currentState = AnimationState.Walk;
        lastSeenLocation = transform.position;
        RunState(AnimationState.Idle);
    }
    void Update()
    {
        if (health.alive)
        {
            Vision();
            if (playerDetected)
            {
                if (Physics.CheckSphere(transform.position, attackRange, playerMask))
                {
                    RunState(AnimationState.Punch);
                }
                else
                {
                    RunState(AnimationState.Walk);
                    if (currentState == AnimationState.Walk)
                    {
                        agent.SetDestination(player.transform.position);
                    }
                }
            } 
            else
            {
                RunState(AnimationState.Idle);
            }
            fallingVelocity = rb.velocity.y;
        }
        
    }
    void RunState(AnimationState state)
    {
        if (currentState != state && !animationLocked)
        {
            if (state == AnimationState.Walk)
            {
                animator.CrossFade("Walk", 0, 0);
                animator.speed = walkSpeed;
            }
            else if (state == AnimationState.Idle)
            {
                animator.CrossFade("Idle", 0, 0);
                agent.SetDestination(lastSeenLocation);
            }
            else if (state == AnimationState.Punch)
            {
                animationLocked = true;
                Helper.AddDamage(rightArm, 20, 10, false, true);
                animator.CrossFade("Punch", 0, 0);
                animator.speed = 1;
                agent.SetDestination(transform.position);
                StartCoroutine(WaitUntilPunchDone());
            }
            currentState = state;
        }
    }
    IEnumerator WaitUntilPunchDone()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f);
        animationLocked = false;
        Destroy(rightArm.GetComponent<Damage>());
        RunState(AnimationState.Walk);
    }
    void Vision()
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
                }
                else playerDetected = false;
            }
            else playerDetected = false;
        }
        else playerDetected = false;
        if (!playerDetected && Time.time - timeDetected <= detectionTime && detectedOnce) playerDetected = true;
    }

    public void Damaged(float amount, object collision)
    {
        if (amount >= 1)
        {
            health.Damage(amount);
            UpdateHealthBar();
            TMP_Text damageNumbersText = Instantiate(Resources.Load<TMP_Text>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform);
            damageNumbersText.text = Mathf.RoundToInt(amount).ToString();
            DamageNumber dn = damageNumbersText.gameObject.AddComponent<DamageNumber>();
            dn.collision = (Collision)collision;
            if (!health.alive && canDie) Died();
        }
        
    }

    void UpdateHealthBar()
    {
        healthText.transform.LookAt(Camera.main.transform);
        healthText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        healthText.text = Helper.HealthToHashtags(health);
    }
    void Died()
    {
        canDie = false;
        StartCoroutine(IdleAnimation());
        Destroy(agent);
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;
        rb.mass = ragdollMass; // ragbody haha

        healthText.enabled = false;
        transform.Rotate(0, 0, 90); // flop haha
        gameObject.AddComponent<Pickup>();

        gameObject.AddComponent<NavMeshObstacle>();

        GameObject loot = Instantiate(Resources.Load<GameObject>("Prefabs/Loot"), transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        Outline outline = loot.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = new Color(0, 187, 255);
        outline.OutlineWidth = 10f;
    }
    IEnumerator IdleAnimation()
    {
        animator.CrossFade("Idle", 0, 0);
        yield return new WaitForEndOfFrame();
        Destroy(animator);
        Destroy(this);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (fallingVelocity < -20)
        {
            health.Damage(-fallingVelocity * 2.5f);
        }
    }
}
