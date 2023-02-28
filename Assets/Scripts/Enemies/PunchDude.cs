using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchDude : Enemy
{
    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] GameObject rightArm;

    
    AnimationState currentState;
    float fallingVelocity = 0;

    Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }
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
                    RunState(AnimationState.Punch);
                }
                else
                {
                    RunState(AnimationState.Walk);
                    if (currentState == AnimationState.Walk)
                    {
                        agent.SetDestination(target.transform.position);
                    }
                }
            }
            else if (agent.enabled)
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
        bool previouslyLocked = animationLocked;
        animationLocked = true;
        
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f);
        if (!previouslyLocked) animationLocked = false;
        Destroy(rightArm.GetComponent<Damage>());
        RunState(AnimationState.Walk);
    }
    protected override void Died()
    {
        canDie = false;
        StartCoroutine(IdleThenDestroy());
        GameObject loot = Instantiate(Resources.Load<GameObject>("Prefabs/Loot"), transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        Outline outline = loot.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = new Color(0, 187, 255);
        outline.OutlineWidth = 10f;
    }
    IEnumerator IdleThenDestroy()
    {
        animator.CrossFade("Idle", 0, 0);
        yield return new WaitForEndOfFrame();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;
        rb.mass = ragdollMass;
        healthText.enabled = false;
        transform.Rotate(0, 0, 90);
        Destroy(animator);
        Destroy(agent);
        Destroy(this);
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        if (fallingVelocity < -10)
        {
            Damaged(-fallingVelocity * 2.5f, collision);
        }
    }
    public override IEnumerator DisableAgentCoroutine()
    {
        yield return StartCoroutine(base.DisableAgentCoroutine());
    }
}
