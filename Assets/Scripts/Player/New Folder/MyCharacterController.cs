using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyCharacterController : MonoBehaviour, ICharacterController
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
    bool canDash = true;
    bool dashing = false;
    float dashStartTime = 0;
    Vector3 dashDirection;

    [Header("Hook")]
    [SerializeField] float hookSpeed;
    [SerializeField] float hookRange;
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
        InputManager.playerInput.Player.Jump.performed += JumpPressed;
        InputManager.playerInput.Player.Jump.canceled += JumpReleased;
        InputManager.playerInput.Player.Jump.Enable();
        InputManager.playerInput.Player.Dash.performed += Dash;
        InputManager.playerInput.Player.Dash.Enable();
        InputManager.playerInput.Player.Attachment.performed += Hook;
        InputManager.playerInput.Player.Attachment.Enable();

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
        currentVelocity = new Vector3(targetMovementVelocity.x, currentVelocity.y, targetMovementVelocity.z);
    }
    void Jump(ref Vector3 currentVelocity)
    {
        if (!jumpPressed || !canJump || jumpedAmount >= maxJumps) return;
        canJump = false;
        jumpedAmount++;
        Motor.ForceUnground();
        currentVelocity = new Vector3(currentVelocity.x, jumpForce, currentVelocity.z);
    }
    void JumpPressed(InputAction.CallbackContext obj) {
        jumpPressed = true;
        canJump = true;

    }
    void JumpReleased(InputAction.CallbackContext obj) {
        jumpPressed = false;
    }

    void Gravity(ref Vector3 currentVelocity, float deltaTime)
    {
        float gravity;
        if (currentVelocity.y > 0) gravity = gravityUp;
        else gravity = gravityDown;
        currentVelocity -= new Vector3(0, gravity * deltaTime, 0);
    }
    void CheckDash(ref Vector3 currentVelocity)
    {
        if (dashing)
        {
            if (Time.time - dashStartTime > dashTime)
            {
                dashing = false;
                canDash = true;
            }
            if (dashing)
            {
                currentVelocity = dashDirection;
            }

        }
    }
    void Dash(InputAction.CallbackContext obj)
    {
        if (!canDash) return;
        canDash = false;
        dashStartTime = Time.time;
        dashing = true;
        dashDirection = Camera.main.transform.forward * dashForce;
        dashDirection = new Vector3(dashDirection.x, 0, dashDirection.z);
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
    void Hook(InputAction.CallbackContext obj)
    {
        if (!canHook) return;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.SphereCast(ray, 0.01f, out RaycastHit hit, hookRange)) {
            canHook = false;
            hookStartTime = Time.time;
            hooking = true;
            hookDirection = Camera.main.transform.forward * hookSpeed;
            hookTime = hit.distance/hookSpeed;
        }
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
