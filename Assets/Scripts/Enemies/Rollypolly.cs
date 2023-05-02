using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Rollypolly : Enemy
{
    [Header("Colliders")]
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] BoxCollider boxCollider;

    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float rollDamage;
    [SerializeField] float rollingTime;
    [SerializeField] float rollCooldown;

    [Header("Roll Stats")]
    [SerializeField] float rollSpeed;
    [SerializeField] float rollAngularSpeed;
    [SerializeField] float rollAcceleration;
    [SerializeField] float rollRadius;
    [SerializeField] float rollHeight;
    [SerializeField] float rollBaseOffset;

    [Header("Walk Stats")]
    [SerializeField] float walkSpeed;
    [SerializeField] float walkAngularSpeed;
    [SerializeField] float walkAcceleration;
    [SerializeField] float walkRadius;
    [SerializeField] float walkHeight;
    [SerializeField] float walkBaseOffset;

    int walkingAgentID = -334000983;
    int rollingAgentID = -1372625422;


    bool playerHit = false;
    bool canRoll = true;
    bool rolling = false;
    public enum AnimationState
    {
        Idle,
        Walk,
        Rollypolly,
        Rolling,
        Flinch
    }
    public AnimationState currentState;
    protected override void Start()
    {
        base.Start();
        currentState = AnimationState.Walk;
        RunState(AnimationState.Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (health.alive && agent.enabled)
        {
            if (playerDetected)
            {
                if (Physics.CheckSphere(transform.position, attackRange, playerMask) && canRoll && !playerHit)
                {
                    RunState(AnimationState.Rollypolly);
                }
                else
                {
                    RunState(AnimationState.Walk);
                    if (currentState == AnimationState.Walk)
                    {
                        SetDestinationTarget();
                    }
                }
            }
            else
            {
                RunState(AnimationState.Idle);
            }
        }
    }
    void RunState(AnimationState state)
    {
        if (currentState != state && !animationLocked)
        {
            switch (state)
            {
                case AnimationState.Walk:
                    animator.CrossFade("Walk", 0, 0);
                    if (agent.isOnNavMesh) agent.SetDestination(lastSeenLocation);
                    animator.speed = walkAnimationSpeed;
                    break;
                case AnimationState.Idle:
                    animator.CrossFade("Idle", 0, 0);
                    agent.SetDestination(transform.position);
                    break;
                case AnimationState.Rollypolly:
                    agent.velocity = Vector3.zero;
                    canRoll = false;
                    animator.CrossFade("Rollypolly", 0, 0);
                    animator.speed = 1;
                    SwitchToRoll();
                    StartCoroutine(WaitUntilRollypollyDone());
                    break;
                case AnimationState.Flinch:
                    animator.CrossFade("Idle", 0, 0);
                    break;
                case AnimationState.Rolling:
                    animationLocked = true;
                    animator.CrossFade("Rolling", 0, 0);
                    StartCoroutine(Rolling());
                    break;
            }
            currentState = state;
        }
    }
    IEnumerator Rolling()
    {
        float startTime = Time.time;
        rolling = true;
        gameObject.GetOrAdd<Hitbox>().Init(rollDamage);
        while (Time.time - startTime <= rollingTime && !playerHit) 
        {
            if (rolling && agent.isOnNavMesh) agent.SetDestination(target.transform.position);
            yield return new WaitForEndOfFrame();
        }
        playerHit = false;
        Destroy(GetComponent<Damage>());
        rolling = false;
        startTime = Time.time;
        agent.speed /= 2;
        while (Time.time - startTime <= 1)
        {
            if (agent.isOnNavMesh) agent.SetDestination(target.transform.position);
            agent.speed -= 1f;
            yield return new WaitForSeconds(0.1f);
        }
        
        animationLocked = false;
        SwitchToWalk();
        RunState(AnimationState.Walk);
        StartCoroutine(RollyCooldown());
    }
    void SwitchToRoll()
    {
        if (agent.enabled)
        {
            agent.agentTypeID = rollingAgentID;
            agent.speed = rollSpeed;
            agent.angularSpeed = rollAngularSpeed;
            agent.acceleration = rollAcceleration;
            agent.height = rollHeight;
            agent.radius = rollRadius;
            agent.baseOffset = rollBaseOffset;
            agent.autoTraverseOffMeshLink = false;
        }
        boxCollider.enabled = false;
        sphereCollider.enabled = true;

    }
    void SwitchToWalk()
    {
        if (agent.enabled)
        {
            agent.agentTypeID = walkingAgentID;
            agent.speed = walkSpeed;
            agent.angularSpeed = walkAngularSpeed;
            agent.acceleration = walkAcceleration;
            agent.height = walkHeight;
            agent.radius = walkRadius;
            agent.baseOffset = walkBaseOffset;
            agent.autoTraverseOffMeshLink = true;
        }
        sphereCollider.enabled = false;
        boxCollider.enabled = true;
    }
    IEnumerator RollyCooldown()
    {
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }
    IEnumerator WaitUntilRollypollyDone()
    {
        animationLocked = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f);
        animationLocked = false;
        RunState(AnimationState.Rolling);
    }
    public override void Killed()
    {
        base.Killed();
        SwitchToWalk();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (rolling && collision.gameObject.GetComponent<Player>() != null && !playerHit)
        {
            RunState(AnimationState.Walk);
            playerHit = true;
        }
    }
}
