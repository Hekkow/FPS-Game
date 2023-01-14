using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] float distanceHeightMultiplier;
    [SerializeField] float tinyBitOfExtraHeight;
    [SerializeField] float enemyHeight;
    [SerializeField] float groundHeight;
    [SerializeField] float timeBeforeGroundCheck;
    [SerializeField] float extraDownwardsDistance;

    [HideInInspector] public bool knocked = false;

    GameObject instantiatedAnimation;
    TMP_Text healthText;
    Transform target;
    Rigidbody rb;
    Health health;
    NavMeshAgent agent;
    new Camera camera;

    float viewRadius;
    float viewAngle;
    float timeDetected;
    float gravity = -Physics.gravity.y;
    bool playerDetected = false;
    bool detectedOnce = false;

    bool alreadyAttacked = false;

    bool jumping = false;

    IEnumerator knockCoroutine;

    private void Start()
    {
        target = GameObject.Find("Player").transform;
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        healthText = transform.GetComponentInChildren<TMP_Text>();
        viewRadius = viewRadiusBeforeDetected;
        viewAngle = viewAngleBeforeDetected;
        camera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        //agent.updateRotation = false;

        
    }
    void Update()
    {
        if (!agent.isOnOffMeshLink && !jumping)
        {
            agent.SetDestination(player.transform.position);

        }
        else
        {
            if (!jumping)
            {
                StartCoroutine(Jump());
            }
        }
        //if (!health.alive)
        //{
        //    Died();
        //    return;
        //}
        //Vision();
        //if (!knocked)
        //{
        //    if (Physics.CheckSphere(transform.position, attackRange, playerMask)) // if player's in attack range
        //    {
        //        AttackPlayer();
        //    }
        //}
        UpdateHealthBar();
    }
    IEnumerator Jump()
    {
        Vector3 startPos = agent.currentOffMeshLinkData.startPos;
        Vector3 endPos = agent.currentOffMeshLinkData.endPos;
        // controlled by rigidbody now
        jumping = true;
        rb.isKinematic = false;
        agent.enabled = false;

        // move towards starting location
        Vector3 velocityToStart = (startPos - transform.position).normalized * agent.speed;
        float distanceToStart = Mathf.Sqrt(Mathf.Pow(startPos.x - transform.position.x, 2) + Mathf.Pow(startPos.z - transform.position.z, 2));
        float timeToStart = distanceToStart / agent.speed;
        rb.velocity = new Vector3(velocityToStart.x, 0, velocityToStart.z);
        yield return new WaitForSeconds(timeToStart);
        rb.velocity = Vector3.zero;


        float actualTime = Time.time;
        float distance = Mathf.Sqrt(Mathf.Pow(startPos.x - endPos.x, 2) + Mathf.Pow(startPos.z - endPos.z, 2));
        float extraHeight = tinyBitOfExtraHeight + distance/distanceHeightMultiplier;
        float height = endPos.y - startPos.y + extraHeight;
        float angle = Mathf.Rad2Deg * Mathf.Atan(height / distance);
        float timeUp;
        float timeDown;
        if (angle < 0)
        {
            timeUp = Mathf.Sqrt((2 * extraHeight) / gravity);
            timeDown = Mathf.Sqrt(Mathf.Abs((2 * height) / gravity));
        }
        else
        {
            timeUp = Mathf.Sqrt((2 * height) / gravity);
            timeDown = Mathf.Sqrt((2 * extraHeight) / gravity);
        }
        float jumpTime = timeUp + timeDown;
        float velocityY = (height / jumpTime) + (gravity * jumpTime/2);
        Vector3 velocity = (endPos-startPos).normalized;

        // if going down, keep moving until off platform
        if (angle < 0)
        {
            while (Physics.Raycast(transform.position, -Vector3.up, enemyHeight + groundHeight))
            {
                rb.velocity = new Vector3((velocity.x * (distance+extraDownwardsDistance)) / timeDown, 0, (velocity.z * (distance+ extraDownwardsDistance)) / timeDown);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            rb.velocity = new Vector3((velocity.x * distance) / timeUp, velocityY, (velocity.z * distance) / timeUp);
        }
        // wait until landed
        yield return new WaitForSeconds(timeBeforeGroundCheck);
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -Vector3.up, enemyHeight + groundHeight));
        // reset
        jumping = false;
        rb.isKinematic = true;
        agent.enabled = true;
        agent.CompleteOffMeshLink();
    }
    void Vision()
    {
        bool playerDetected;
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
    void AttackPlayer()
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        if (!alreadyAttacked)
        {
            instantiatedAnimation = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyAttack"), transform.position, new Quaternion(1, 0, 0, 1));
            Helper.AddDamage(instantiatedAnimation.gameObject, attackDamage, attackDamage, false, true);
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
