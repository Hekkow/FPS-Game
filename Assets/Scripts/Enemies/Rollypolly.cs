using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Rollypolly : Enemy, IDamageable
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

    [Header("Other")]
    [SerializeField] float lookAroundSpeed;

    int walkingAgentID = -334000983;
    int rollingAgentID = -1372625422;


    bool playerHit = false;
    bool canRoll = true;
    bool rolling = false;
    enum AnimationState
    {
        Idle,
        Walk,
        Rollypolly,
        Rolling,
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
        if (health.alive && agent.enabled)
        {
            if (playerDetected)
            {
                if (Physics.CheckSphere(transform.position, attackRange, playerMask) && canRoll)
                {
                    RunState(AnimationState.Rollypolly);
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
                    agent.SetDestination(lastSeenLocation);
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
        Helper.AddDamage(gameObject, rollDamage, 10, false, true, false);
        while (Time.time - startTime <= rollingTime && !playerHit)
        {
            if (rolling) agent.SetDestination(target.transform.position);
            yield return new WaitForEndOfFrame();
        }
        animationLocked = false;
        playerHit = false;
        rolling = false;
        Destroy(GetComponent<Damage>());
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
            if (!playerDetected && !rolling) StartCoroutine(LookAround());
        }
    }
    IEnumerator LookAround()
    {
        float rotateAmount = 0;
        while (rotateAmount <= 360 && !playerDetected)
        {
            rotateAmount += lookAroundSpeed * Time.deltaTime;
            transform.Rotate(0, lookAroundSpeed * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }
    }
    public override IEnumerator KnockbackCoroutine()
    {
        StartCoroutine(base.KnockbackCoroutine());
        rolling = false;
        Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0.01f, Camera.main.transform.forward.z).normalized;
        if (Inventory.HasGun()) rb.AddForce(direction * Inventory.guns[0].bulletKnockback / 2, ForceMode.Impulse);
        yield return new WaitUntil(() => knocked); 
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height / 2 + 0.3f));
        rolling = true;
        agent.enabled = true;
    }
    public override IEnumerator KnockbackCoroutine(Collision collision, object origin)
    {
        StartCoroutine(base.KnockbackCoroutine(collision, origin));
        rolling = false;
        if (origin is Damage damage && damage.thrown)
        {
            Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0.01f, Camera.main.transform.forward.z).normalized;
            rb.AddForce(direction * 1000, ForceMode.Impulse);
        }
        yield return new WaitUntil(() => knocked);
        yield return new WaitUntil(() => Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agent.height / 2 + 0.3f));
        agent.enabled = true;
        rolling = true;
    }
    public override void Killed()
    {
        base.Killed();
        SwitchToWalk();
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
    void OnCollisionEnter(Collision collision)
    {
        if (rolling && collision.gameObject.GetComponent<Player>() != null && !playerHit)
        {
            playerHit = true;
        }
    }
}
