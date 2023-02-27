using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask obstacles;
    [SerializeField] Player player;

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
        Walk,
        Dead
    }
    [HideInInspector] public bool knocked = false;
    Animator animator;
    TMP_Text healthText;
    Transform target;
    Rigidbody rb;
    Health health;
    NavMeshAgent agent;
    Damage damage;

    float viewRadius;
    float viewAngle;
    float timeDetected;
    bool playerDetected = false;
    bool detectedOnce = false;
    float fallingVelocity = 0;
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
        animator.Play("Idle", 0, 0);
        damage = GetComponentInChildren<Damage>();
        damage.damage = 0;
    }
    void Update()
    {
        Vision();
        if (Physics.CheckSphere(transform.position, attackRange, playerMask)) // if player's in attack range
        {
            agent.SetDestination(transform.position);
            AttackPlayer();
        }
        else if (pathfinding && playerDetected)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
            {
                animator.Play("Walk", 0, 0);
                animator.speed = walkSpeed;
            }
            agent.SetDestination(player.transform.position);
        }
        else
        {
            animator.Play("Idle");
            agent.SetDestination(transform.position);
        }
        UpdateHealthBar();
        if (!health.alive)
        {
            Died();
            return;
        }
        fallingVelocity = rb.velocity.y;
    }
    IEnumerator WaitUntilPunchDone()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f);
        damage.damage = 0;

    }
    void AttackPlayer()
    {
        damage.damage = 20;
        animator.CrossFade("Punch", 0, 0);
        animator.speed = 1;
        StartCoroutine(WaitUntilPunchDone());
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
    void UpdateHealthBar()
    {
        healthText.transform.LookAt(Camera.main.transform);
        healthText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        healthText.text = Helper.HealthToHashtags(health);
    }
    void Died()
    {
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
