using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickDude : Enemy, IDamageable
{
    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float kickDamage;
    [SerializeField] GameObject rightFoot;

    [Header("Other")]
    [SerializeField] float lookAroundSpeed;

    enum AnimationState
    {
        Idle,
        Walk,
        Kick,
        Flinch
    }
    AnimationState currentState;
    protected override void Awake()
    {
        base.Awake();
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
                    RunState(AnimationState.Kick);
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
                    agent.SetDestination(lastSeenLocation);
                    animator.speed = walkAnimationSpeed;
                    break;
                case AnimationState.Idle:
                    animator.CrossFade("Idle", 0, 0);
                    agent.SetDestination(transform.position);
                    break;
                case AnimationState.Kick:
                    Helper.AddDamage(rightFoot, kickDamage, 10, false, true, false);
                    animator.CrossFade("Kick", 0, 0);
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
        Destroy(rightFoot.GetComponent<Damage>());
        RunState(AnimationState.Walk);
    }
    public override void Damaged(float amount, object collision, object origin)
    {
        base.Damaged(amount, collision, origin);
        if (health.alive && amount >= 1)
        {
            if (collision is Collision)
            {
                StartCoroutine(KnockbackCoroutine((Collision)collision, origin));
                Instantiate(Resources.Load<DamageNumber>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform).Init(amount, (Collision)collision);
            }
            else
            {
                StartCoroutine(KnockbackCoroutine());
                Instantiate(Resources.Load<DamageNumber>("Prefabs/DamageNumbers"), Vector3.zero, Quaternion.identity, GameObject.Find("HUD").transform).Init(amount, (Collider)collision);
            }
        }
        if (!playerDetected) StartCoroutine(LookAround());
    }
    IEnumerator LookAround()
    {
        while (transform.localRotation.y < 360 && !playerDetected)
        {
            transform.Rotate(0, lookAroundSpeed * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }
    }
    public override IEnumerator KnockbackCoroutine()
    {
        StartCoroutine(base.KnockbackCoroutine());
        Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0.01f, Camera.main.transform.forward.z).normalized;
        rb.AddForce(direction * Inventory.guns[0].bulletKnockback / 2, ForceMode.Impulse);
        yield return new WaitUntil(() => knocked);
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height / 2 + 0.3f));
        agent.enabled = true;
    }
    public override IEnumerator KnockbackCoroutine(Collision collision, object origin)
    {
        StartCoroutine(base.KnockbackCoroutine(collision, origin));
        if (origin is Damage damage && damage.thrown)
        {
            Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0.01f, Camera.main.transform.forward.z).normalized;
            rb.AddForce(direction * 1000, ForceMode.Impulse);
        }
        yield return new WaitUntil(() => knocked);
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height / 2 + 0.3f));
        agent.enabled = true;
    }
    public override void Killed()
    {
        base.Killed();
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
    public override IEnumerator DisableAgentCoroutine()
    {
        yield return StartCoroutine(base.DisableAgentCoroutine());
    }
}
