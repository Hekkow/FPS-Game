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

    [Header("Pathfinding")]
    [SerializeField] protected float walkSpeed;

    protected enum AnimationState
    {
        Idle,
        Punch,
        Kick,
        Walk
    }
    protected TMP_Text healthText;
    protected NavMeshAgent agent;
    protected GameObject target;
    protected Health health;
    protected Rigidbody rb;
    float viewRadius;
    float viewAngle;
    float timeDetected;
    protected bool playerDetected = false;
    bool detectedOnce = false;
    protected bool animationLocked = false;
    protected bool canDie = true;
    protected Vector3 lastSeenLocation;
    protected virtual void Awake()
    {
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        healthText = GetComponentInChildren<TMP_Text>();
    }
    protected virtual void Start()
    {
        viewRadius = viewRadiusBeforeDetected;
        viewAngle = viewAngleBeforeDetected;
        lastSeenLocation = transform.position;
        StartCoroutine(Vision());
    }
    protected virtual void Update()
    {
        UpdateHealthBar();
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
    public void Damaged(float amount, object collision)
    {
        if (amount >= 1)
        {
            health.Damage(amount);
            if (health.alive)
            {
                if (collision is Collision)
                {
                    TMP_Text damageNumbersText = Instantiate(Resources.Load<TMP_Text>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform);
                    damageNumbersText.text = Mathf.RoundToInt(amount).ToString();
                    DamageNumber damageNumber = damageNumbersText.gameObject.AddComponent<DamageNumber>();
                    damageNumber.collision = (Collision)collision;
                }
                else
                {
                    // later
                }
            }
            else if (canDie) Died();
        }
    }
    protected virtual void Died() { }
    protected virtual void OnCollisionEnter(Collision collision) { }

    void UpdateHealthBar()
    {
        healthText.transform.LookAt(Camera.main.transform);
        healthText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        healthText.text = Helper.HealthToHashtags(health);
    }
    public virtual IEnumerator DisableAgentCoroutine()
    {
        animationLocked = true;
        rb.isKinematic = false;
        agent.enabled = false;
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !health.alive || Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height/2+0.3f)); 
        if (health.alive)
        {
            rb.isKinematic = true;
            agent.enabled = true;
            animationLocked = false;
        }
    }

}
