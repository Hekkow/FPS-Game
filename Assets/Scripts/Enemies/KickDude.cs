using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickDude : Enemy
{
    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float kickDamage;
    [SerializeField] GameObject kickFoot;

    enum AnimationState
    {
        Idle,
        Walk,
        Kick,
        Flinch
    }
    AnimationState currentState;
    protected override void Start()
    {
        base.Start();
        currentState = AnimationState.Walk;
        RunState(AnimationState.Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (health.alive)
        {
            if (playerDetected && agent.enabled)
            {
                if (Physics.CheckSphere(transform.position, attackRange, playerMask))
                {
                    RunState(AnimationState.Kick);
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
            else if (agent.enabled)
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
                case AnimationState.Kick:
                    Helper.AddDamage(kickFoot, kickDamage, 10, false, true, false);
                    animator.CrossFade("Kick", 0, 0);
                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                    animator.speed = 1;
                    agent.SetDestination(transform.position);
                    StartCoroutine(WaitUntilKickDone());
                    break;
                case AnimationState.Flinch:
                    animator.CrossFade("Idle", 0, 0);
                    break;
            }
            currentState = state;
        }
    }
    IEnumerator WaitUntilKickDone()
    {
        bool previouslyLocked = animationLocked;
        animationLocked = true;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f);
        if (!previouslyLocked) animationLocked = false;
        Destroy(kickFoot.GetComponent<Damage>());
        RunState(AnimationState.Walk);
    }
    
}
