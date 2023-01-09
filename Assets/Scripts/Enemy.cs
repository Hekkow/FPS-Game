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
        Vector3 endPos = agent.currentOffMeshLinkData.endPos;
        Vector3 startPos = agent.currentOffMeshLinkData.startPos;
        bool down = startPos.y - endPos.y > 2;
        //float velY = Mathf.Sqrt(2 * -Physics.gravity.y * (endPos.y+0.5f - startPos.y));
        //float velY = (endPos.y-(startPos.y*1)+(1/2*Physics.gravity.y*Mathf.Pow(1, 2)))/1*2;
        float velY;
        float velX;
        float velZ;
        if (down)
        {
            velY = 0;
            velX = (endPos.x - transform.position.x);
            velZ = (endPos.z - transform.position.z);
            Debug.Log(velX);
        }
        else
        {
            velY = Mathf.Sqrt(2 * -Physics.gravity.y * (endPos.y+1f - startPos.y));
            velX = (endPos.x - transform.position.x);
            velZ = (endPos.z - transform.position.z);
            Debug.Log(velX);

        }
        //velY = 2 * (endPos.y + 1f - startPos.y) / 1;
        //Debug.Log(velY);
        //float velY = Mathf.Sqrt(-2 * Mathf.Abs(endPos.y - startPos.y + 0.5f) * -9.81f);
        //float velX = 0;
        //float velZ = 0;

        jumping = true;
        rb.isKinematic = false;
        agent.enabled = false;
        if (down)
        {
            while (Physics.Raycast(transform.position, -Vector3.up, 1 + 0.1f))
            {
                rb.velocity = new Vector3(velX, velY, velZ);

                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            rb.velocity = new Vector3(velX, velY, velZ);
        }
        //Debug.Log(Mathf.Sqrt(-2 * (endPos.y - startPos.y + 0.5f) * -9.81f));
        //Debug.Log(2 * (endPos.y + 500 - startPos.y) * Physics.gravity.y);
        //rb.AddForce(((endPos-transform.position).normalized * 80) + (Vector3.up * 120), ForceMode.Impulse);
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
