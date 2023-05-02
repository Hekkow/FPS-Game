using KinematicCharacterController;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour, ICharacterController
{
    [Header("References")]
    [SerializeField] Player player;
    public KinematicCharacterMotor Motor;

    [HideInInspector] public float yRotation;
    [HideInInspector] public Vector2 mouseInput;
    InputAction movementInput;
    Vector2 moveInput;

    [Header("Move")]
    [SerializeField, Rename("Walk Speed")] float walkSpeed;
    [SerializeField, Rename("Current Max Speed")] float speed;

    [Header("Jump")]
    [SerializeField, Rename("Speed")] float jumpSpeed;
    [SerializeField, Rename("Max Jumps")] int maxJumps;
    bool jumpPressed = false;
    bool canJump = true;
    int jumpedAmount = 0;

    [Header("Dash")]
    [SerializeField, Rename("Speed Multiplier")] float dashSpeedMultiplier;
    [SerializeField, Rename("Time (s)")] float dashTime;
    [SerializeField, Rename("Cooldown")] float dashCooldown;
    [SerializeField, Rename("Minimum Speed")] float minimumDashSpeed;
    float dashStartTime = 0;
    Vector3 dashVelocity;

    [Header("Hook")]
    [SerializeField, Rename("Speed")] float hookSpeed;
    [SerializeField, Rename("Max Range")] public float hookRange;
    [SerializeField, Rename("Cooldown")] float hookCooldown;
    RaycastHit hit;
    float hookTime;
    float hookStartTime;
    Vector3 hookVelocity;

    [Header("Explosion")]
    [SerializeField, Rename("Against Velocity")] float explosionAgainstVelocity;
    [SerializeField, Rename("With Velocity")] float explosionWithVelocity;
    Vector3 explosionVelocity;
    List<Vector3> extraForces = new List<Vector3>();

    [Header("Momentum")]
    [SerializeField] float speedLossAngle;
    [SerializeField] float speedLossAmount;

    [Header("Gravity")]
    [SerializeField, Rename("Up")] float gravityUp;
    [SerializeField, Rename("Down")] float gravityDown;


    public enum MovementState
    {
        Default,
        Dash,
        Hook,
        Explosion
    }

    public MovementState currentState = MovementState.Default;

    void Start()
    {
        Motor.CharacterController = this;
    }
    void OnEnable()
    {
        movementInput = InputManager.playerInput.Player.Move;
        movementInput.Enable();
        InputManager.playerInput.Player.Jump.performed += _ => JumpPressed();
        InputManager.playerInput.Player.Jump.canceled += _ => JumpReleased();
        InputManager.playerInput.Player.Jump.Enable();
        InputManager.playerInput.Player.Dash.performed += _ => Dash();
        InputManager.playerInput.Player.Dash.Enable();

    }
    void Update()
    {
        moveInput = movementInput.ReadValue<Vector2>();
    }
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // checks if anything is done and switches to base state if done
        switch (currentState)
        {
            case MovementState.Dash:
                if (Time.time - dashStartTime > dashTime) Transition(MovementState.Default);
                break;
            case MovementState.Hook:
                if (Time.time - hookStartTime > hookTime) Transition(MovementState.Default);
                break;
        }
        // jump is so anything can get cancelled. doesn't have its own state
        Jump(ref currentVelocity);
        switch (currentState)
        {
            case MovementState.Default:
                Move(ref currentVelocity);
                Gravity(ref currentVelocity, deltaTime);
                break;
            case MovementState.Dash:
                currentVelocity = dashVelocity;
                break;
            case MovementState.Hook:
                currentVelocity = hookVelocity;
                break;
            case MovementState.Explosion:
                ExplosionMove(ref currentVelocity, deltaTime);
                Gravity(ref currentVelocity, deltaTime);
                break;

        }
        ExtraForces(ref currentVelocity);
    }

    public void AddExtraForce(Vector3 force)
    {
        extraForces.Add(force);
    }
    void ExtraForces(ref Vector3 currentVelocity)
    {
        if (extraForces.Count > 0) currentVelocity = Vector3.zero;
        else return;
        bool grounded = Motor.GroundingStatus.FoundAnyGround;
        for (int i = 0; i < extraForces.Count; i++)
        {
            if (extraForces[i].y > 0 && grounded)
            {
                Motor.ForceUnground();
                grounded = false;

            }
            currentVelocity += extraForces[i];
        }
        if (!grounded) Transition(MovementState.Explosion);
        explosionVelocity = currentVelocity;
        //if (extraForces.Count > 0) speed = currentVelocity.horizontalMagnitude().MakeMin(speed);
        extraForces.Clear();
    }

    public void Transition(MovementState state)
    {
        if (currentState == state) return;
        MovementState oldState = currentState;
        if (EnterState(state))
        {
            ExitState(oldState);
            currentState = state;
        }
    }
    bool EnterState(MovementState state)
    {
        switch (state)
        {
            case MovementState.Dash:
                if (Time.time - dashStartTime < dashCooldown + dashTime) return false;
                dashStartTime = Time.time;
                Vector3 direction;
                float dashSpeed = (dashSpeedMultiplier * Motor.BaseVelocity.HorizontalMagnitude()).MakeMin(minimumDashSpeed);
                if (MovePressed()) direction = transform.forward * moveInput.y + transform.right * moveInput.x;
                else direction = transform.forward;
                dashVelocity = direction.normalized.SetY(0) * dashSpeed;
                return true;
            case MovementState.Hook:
                if (Time.time - hookStartTime < hookCooldown + hookTime) return false;
                Motor.ForceUnground();
                hookStartTime = Time.time;
                hookVelocity = Camera.main.transform.forward * (hookSpeed + Motor.BaseVelocity.HorizontalMagnitude());
                hookTime = hit.distance / hookSpeed;
                return true;
            case MovementState.Explosion:
                //speed = walkSpeed;
                return true;
            default:
                return true;
        }
    }
    void ExitState(MovementState state)
    {
        switch (state)
        {
            default:
                break;
        }
    }



    void Move(ref Vector3 currentVelocity)
    {
        if (!MovePressed() && speed > walkSpeed) speed = currentVelocity.HorizontalMagnitude().MakeMin(walkSpeed);
        Vector3 direction = transform.forward * moveInput.y + transform.right * moveInput.x;
        Vector3 targetMovementVelocity = direction * speed;
        currentVelocity = targetMovementVelocity.SetY(currentVelocity.y);
    }
    void ExplosionMove(ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 direction = transform.forward * moveInput.y + transform.right * moveInput.x;
        Vector3 targetMovementVelocity = direction * speed;
        float playerVelocity;
        float sameDirectionCurrent = Vector3.Dot(currentVelocity, explosionVelocity);
        float sameDirectionTarget = Vector3.Dot(targetMovementVelocity, explosionVelocity);

        if (sameDirectionTarget < 0) { playerVelocity = explosionAgainstVelocity; }
        else { playerVelocity = explosionWithVelocity; }
        currentVelocity += (playerVelocity * deltaTime * targetMovementVelocity).SetY(0);
        Debug.Log((sameDirectionCurrent > 0) + " " + (sameDirectionTarget > 0) + " " + currentVelocity.HorizontalMagnitude() + " " +speed); 
        if (sameDirectionCurrent < 0 && sameDirectionTarget < 0 && currentVelocity.HorizontalMagnitude() >= speed && MovePressed())
        {
            //Debug.Log($"{currentVelocity.HorizontalMagnitude()} >= {speed}");
            Transition(MovementState.Default);
        }
    }
    void Jump(ref Vector3 currentVelocity)
    {
        if (!jumpPressed || !canJump || jumpedAmount >= maxJumps) return;
        MovementState previousState = currentState;
        canJump = false;
        jumpedAmount++;
        Motor.ForceUnground();
        if (previousState == MovementState.Dash || previousState == MovementState.Hook || previousState == MovementState.Explosion)
        {
            speed = Motor.BaseVelocity.HorizontalMagnitude().MakeMin(speed);
        }
        Transition(MovementState.Default);

        currentVelocity = currentVelocity.SetY(jumpSpeed);
        
    }
    void Gravity(ref Vector3 currentVelocity, float deltaTime)
    {
        float gravity;
        if (currentVelocity.y > 0) gravity = gravityUp;
        else gravity = gravityDown;
        currentVelocity = currentVelocity.AddY(-gravity * deltaTime);
    }
    void Dash()
    {
        Transition(MovementState.Dash);
    }
    public void Hook(RaycastHit raycastHit)
    {
        hit = raycastHit;
        Transition(MovementState.Hook);
    }

     
    
    bool MovePressed()
    {
        return !(moveInput.x == 0 && moveInput.y == 0);
    }
    void JumpPressed()
    {
        jumpPressed = true;
        canJump = true;
    }
    void JumpReleased()
    {
        jumpPressed = false;
    }



    public void AfterCharacterUpdate(float deltaTime)
    {
        
    }
    public void BeforeCharacterUpdate(float deltaTime)
    {
        
    }
    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }
    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        canJump = true;
        jumpedAmount = 0;
        if (!MovePressed() && speed > walkSpeed) speed = walkSpeed;
        if (currentState == MovementState.Explosion) Transition(MovementState.Default);
    }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        if (hitPoint.y - transform.position.y > speedLossAngle && speed > walkSpeed) speed = (speed - speedLossAmount).MakeMin(walkSpeed);
    }
    public void PostGroundingUpdate(float deltaTime)
    {
        
    }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        
    }
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        currentRotation = Quaternion.Euler(0, yRotation, 0);
    }
}
