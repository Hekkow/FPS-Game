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

    bool playerDetected = false;
    bool detectedOnce = false;

    bool alreadyAttacked = false;

    bool jumping = false;
    bool once = false;

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
            //transform.LookAt(player.transform.position);

        }
        else
        {
            if (!jumping)
            {
                Jump();
            }
            
            //agent.CompleteOffMeshLink();
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
    void Jump()
    {
        //Vector3 startPos = agent.transform.position;
        Vector3 endPos = agent.currentOffMeshLinkData.endPos;
        StartCoroutine(WaitUntilLanded());
        //agent.transform.position = endPos;
        //transform.position = endPos;
        
        
    }
    IEnumerator WaitUntilLanded()
    {
        Vector3 startPos = agent.currentOffMeshLinkData.startPos;
        Vector3 endPos = agent.currentOffMeshLinkData.endPos;
        bool down = startPos.y > endPos.y;
        transform.position = new Vector3(startPos.x, transform.position.y, startPos.z);
        float distance = Mathf.Sqrt(Mathf.Pow(startPos.x - endPos.x, 2) + Mathf.Pow(startPos.z - endPos.z, 2));
        float extraHeight = 0.083333f + distance/10;
        float height = endPos.y - startPos.y + extraHeight;
        float angle = Mathf.Rad2Deg * Mathf.Atan(height / distance);
        float timeUp;
        float timeDown;
        if (angle < 0)
        {
            timeUp = Mathf.Sqrt((2 * -height) / -Physics.gravity.y);
            timeDown = Mathf.Sqrt((2 * extraHeight) / -Physics.gravity.y);
        }
        else
        {
            timeUp = Mathf.Sqrt((2 * height) / -Physics.gravity.y);
            timeDown = Mathf.Sqrt((2 * extraHeight) / -Physics.gravity.y);
        }
        float time = timeUp + timeDown;
        jumping = true;
        rb.isKinematic = false;
        agent.enabled = false;
        float horizontalVelocity = distance;
        float velocityY = (height / time) + (-Physics.gravity.y * time/2);
        Vector3 velocity = (endPos-startPos).normalized;
        rb.velocity = new Vector3((velocity.x * distance) / timeUp, velocityY, (velocity.z * distance) / timeUp);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -Vector3.up, 1 + 0.1f));
        jumping = false;
        rb.isKinematic = true;
        agent.enabled = true;
        agent.isStopped = false;
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
