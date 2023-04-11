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
    bool canDash = true;
    bool dashing = false;
    float dashStartTime = 0;
    Vector3 dashDirection;



    [Header("Hook")]
    [SerializeField] float hookSpeed;
    [SerializeField] public float hookRange;
    [SerializeField] float hookCooldown;
    bool canHook = true;
    bool hooking = false;
    float hookTime;
    float hookStartTime;
    Vector3 hookDirection;

    [Header("Physics")]
    [SerializeField] float gravityUp;
    [SerializeField] float gravityDown;


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
        Move(ref currentVelocity);
        Gravity(ref currentVelocity, deltaTime);
        Jump(ref currentVelocity);
        CheckDash(ref currentVelocity);
        CheckHook(ref currentVelocity);
    }
    void Move(ref Vector3 currentVelocity)
    {
        Vector3 direction = transform.forward * moveInput.y + transform.right * moveInput.x;
        Vector3 targetMovementVelocity = direction * speed;
        currentVelocity = targetMovementVelocity.SetY(currentVelocity.y);
    }
    void Jump(ref Vector3 currentVelocity)
    {
        if (!jumpPressed || !canJump || jumpedAmount >= maxJumps) return;
        CancelAll();
        canJump = false;
        jumpedAmount++;
        Motor.ForceUnground();
        currentVelocity = currentVelocity.SetY(jumpForce);
    }
    void JumpPressed() {
        jumpPressed = true;
        canJump = true;

    }
    void JumpReleased() {
        jumpPressed = false;
    }

    void Gravity(ref Vector3 currentVelocity, float deltaTime)
    {
        float gravity;
        if (currentVelocity.y > 0) gravity = gravityUp;
        else gravity = gravityDown;
        currentVelocity = currentVelocity.AddY(-gravity * deltaTime);
    }
    void CheckDash(ref Vector3 currentVelocity)
    {
        if (dashing)
        {
            if (Time.time - dashStartTime > dashTime) dashing = false;
            if (dashing) currentVelocity = dashDirection;
        }
    }
    void Dash()
    {
        if (Time.time - dashStartTime > dashCooldown + dashTime) canDash = true;
        if (!canDash) return;
        CancelAll();
        dashStartTime = Time.time;
        canDash = false;
        dashing = true;
        if (MovePressed()) dashDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized * dashForce;
        else dashDirection = Camera.main.transform.forward * dashForce;
        dashDirection = dashDirection.SetY(0);
    }
    bool MovePressed()
    {
        return !(moveInput.x == 0 && moveInput.y == 0);
    }
    void CheckHook(ref Vector3 currentVelocity)
    {
        if (!hooking) return;
        if (Time.time - hookStartTime > hookTime)
        {
            hooking = false;
            canHook = true;
        }
        if (hooking)
        {
            currentVelocity = hookDirection;
        }
    }
    public void Hook(RaycastHit hit)
    {
        if (Time.time - hookStartTime > hookCooldown + hookTime) canHook = true;
        if (!canHook) return;
        CancelAll();
        Motor.ForceUnground();
        hookStartTime = Time.time;
        canHook = false;
        hooking = true;
        hookDirection = Camera.main.transform.forward * hookSpeed;
        hookTime = hit.distance/hookSpeed;
    }
    public void CancelAll()
    {
        hooking = false;
        dashing = false;
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
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
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
