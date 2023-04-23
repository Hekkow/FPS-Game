using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] float walkSpeed;
    [SerializeField] float speed;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] int maxJumps;
    bool jumpPressed = false;
    bool canJump = true;
    int jumpedAmount = 0;

    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    float dashStartTime = 0;
    Vector3 dashDirection;

    [Header("Hook")]
    [SerializeField] float hookSpeed;
    [SerializeField] public float hookRange;
    [SerializeField] float hookCooldown;
    RaycastHit hit;
    float hookTime;
    float hookStartTime;
    Vector3 hookDirection;

    [Header("Momentum")]
    [SerializeField] float speedLossAngle;
    [SerializeField] float speedLossAmount;

    [Header("Gravity")]
    [SerializeField] float gravityUp;
    [SerializeField] float gravityDown;

    public enum MovementState
    {
        Run,
        Dash,
        Hook,
        WallRun
    }

    public MovementState currentState = MovementState.Run;

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
        switch (currentState)
        {
            case MovementState.Dash:
                if (Time.time - dashStartTime > dashTime) currentState = MovementState.Run;
                break;
            case MovementState.Hook:
                if (Time.time - hookStartTime > hookTime) currentState = MovementState.Run;
                break;
        }
        Jump(ref currentVelocity);
        switch (currentState)
        {
            case MovementState.Run:
                Move(ref currentVelocity);
                Gravity(ref currentVelocity, deltaTime);
                break;
            case MovementState.Dash:
                currentVelocity = dashDirection;
                break;
            case MovementState.Hook:
                currentVelocity = hookDirection;
                break;
        }
    }



    void Transition(MovementState state)
    {
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
            case MovementState.Run:
                return true;
            case MovementState.Dash:
                if (Time.time - dashStartTime < dashCooldown + dashTime) return false;
                dashStartTime = Time.time;
                if (MovePressed()) dashDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized * (dashForce + Motor.BaseVelocity.horizontalMagnitude() / 2);
                else dashDirection = Camera.main.transform.forward * dashForce;
                dashDirection = dashDirection.SetY(0);
                return true;
            case MovementState.Hook:
                if (Time.time - hookStartTime < hookCooldown + hookTime) return false;
                Motor.ForceUnground();
                hookStartTime = Time.time;
                hookDirection = Camera.main.transform.forward * (hookSpeed + Motor.BaseVelocity.horizontalMagnitude() / 2);
                hookTime = hit.distance / hookSpeed;
                return true;
            default:
                return false;
        }
    }
    void ExitState(MovementState state)
    {
        switch (state)
        {
            case MovementState.Run:
                break;
            case MovementState.Dash:
                speed += Motor.BaseVelocity.horizontalMagnitude() / 2;
                break;
            case MovementState.Hook:
                speed += Motor.BaseVelocity.horizontalMagnitude() / 2;
                break;
        }
    }



    void Move(ref Vector3 currentVelocity)
    {
        Vector3 direction;
        if (!MovePressed() && speed > walkSpeed) speed = currentVelocity.horizontalMagnitude().UpTo(walkSpeed);
        direction = transform.forward * moveInput.y + transform.right * moveInput.x;
        Vector3 targetMovementVelocity = direction * speed;
        currentVelocity = targetMovementVelocity.SetY(currentVelocity.y);
    }
    void Jump(ref Vector3 currentVelocity)
    {
        if (!jumpPressed || !canJump || jumpedAmount >= maxJumps) return;
        Transition(MovementState.Run);
        canJump = false;
        jumpedAmount++;
        Motor.ForceUnground();
        currentVelocity = currentVelocity.SetY(jumpForce);
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
    }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        if (!Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable && MovePressed()) { }
        if (hitPoint.y - transform.position.y > speedLossAngle && speed > walkSpeed) speed = (speed - speedLossAmount).UpTo(walkSpeed);
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
