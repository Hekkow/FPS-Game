using KinematicCharacterController;
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

    [Header("Wallrun")]
    [SerializeField] float wallRunSpeed;
    [SerializeField, Rename("Amount of Rays")] int wallRunRaysAmount;
    [SerializeField, Rename("Starting Angle")] float wallRunRaysStartingAngle;
    [SerializeField, Rename("Ray Length")] float wallRunRaysLength;
    float lookDirection;
    public int forward = 1;
    Vector3 wallNormal;

    [Header("Momentum")]
    [SerializeField] float speedLossAngle;
    [SerializeField] float speedLossAmount;

    [Header("Gravity")]
    [SerializeField] float gravityUp;
    [SerializeField] float gravityDown;


    public enum MovementState
    {
        Default,
        Dash,
        Hook,
        WallRun,
        WallJump
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
                if (Time.time - dashStartTime > dashTime) currentState = MovementState.Default;
                break;
            case MovementState.Hook:
                if (Time.time - hookStartTime > hookTime) currentState = MovementState.Default;
                break;
            case MovementState.WallJump:
                if (MovePressed()) currentState = MovementState.Default;
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
                currentVelocity = dashDirection;
                break;
            case MovementState.Hook:
                currentVelocity = hookDirection;
                break;
            case MovementState.WallRun:
                WallRun(ref currentVelocity);
                break;
            case MovementState.WallJump:
                Gravity(ref currentVelocity, deltaTime);
                break;
        }
    }



    void Transition(MovementState state)
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
                if (MovePressed()) dashDirection = (Camera.main.transform.forward * moveInput.y + Camera.main.transform.right * moveInput.x).normalized * (dashForce + Motor.BaseVelocity.horizontalMagnitude() / 2);
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
            case MovementState.WallRun:
                RaycastHit? closestHit = transform.ClosestHit(out int connected, wallRunRaysAmount, wallRunRaysStartingAngle, wallRunRaysLength, 0);
                if (closestHit == null) return false;
                Vector3 wallForward = Vector3.Cross(closestHit.Value.normal, transform.up);
                float dotProduct = Vector3.Dot(Camera.main.transform.forward, wallForward);
                if (dotProduct < 0) forward = -1;
                else forward = 1;
                if (jumpedAmount == maxJumps) jumpedAmount--;
                return true;
            default:
                return true;
        }
    }
    void ExitState(MovementState state)
    {
        switch (state)
        {
            case MovementState.Dash:
                speed += Motor.BaseVelocity.horizontalMagnitude() / 2;
                break;
            case MovementState.Hook:
                speed += Motor.BaseVelocity.horizontalMagnitude() / 2;
                break;
            case MovementState.WallRun:
                //speed = Motor.BaseVelocity.horizontalMagnitude();
                break;
        }
    }



    void Move(ref Vector3 currentVelocity)
    {
        if (!MovePressed() && speed > walkSpeed) speed = currentVelocity.horizontalMagnitude().UpTo(walkSpeed);
        Vector3 direction = transform.forward * moveInput.y + transform.right * moveInput.x;
        Vector3 targetMovementVelocity = direction * speed;
        currentVelocity = targetMovementVelocity.SetY(currentVelocity.y);
    }
    void WallRun(ref Vector3 currentVelocity)
    {
        RaycastHit? hit = transform.ClosestHit(out int connected, wallRunRaysAmount, wallRunRaysStartingAngle, wallRunRaysLength, 0);
        if (connected == 0) Transition(MovementState.WallJump); // if no ray hits
        else if (hit != null)
        {
            wallNormal = hit.Value.normal;
            Vector3 wallForward = Vector3.Cross(hit.Value.normal, transform.up) * forward;
            Vector3 move = Camera.main.transform.forward * moveInput.y + Camera.main.transform.right * moveInput.x;
            float dotProduct = Vector3.Dot(move, wallForward); // gets how close desired movement is to wall forward
            if (Mathf.Abs(dotProduct) < 0.5f) { }
            else if (dotProduct < 0)
            {
                if (forward == 1) forward = -1;
                else if (forward == -1) forward = 1;
            }
            Quaternion rotation = Quaternion.LookRotation(wallForward);
            lookDirection = rotation.eulerAngles.y;
            speed = currentVelocity.horizontalMagnitude().UpTo(wallRunSpeed);
            currentVelocity = wallForward * speed;
            currentVelocity += -hit.Value.normal * 100; 
        }


    }
    void Jump(ref Vector3 currentVelocity)
    {
        if (!jumpPressed || !canJump || jumpedAmount >= maxJumps) return;
        canJump = false;
        jumpedAmount++;
        Motor.ForceUnground();
        if (currentState == MovementState.WallRun)
        {
            Transition(MovementState.WallJump);
            currentVelocity = ((wallNormal.normalized + currentVelocity.normalized).normalized * speed).SetY(jumpForce);
        }
        else {
            Transition(MovementState.Default);
            currentVelocity = currentVelocity.SetY(jumpForce);
        }
        
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
        if (currentState == MovementState.WallJump) Transition(MovementState.Default);
        canJump = true;
        jumpedAmount = 0;
        if (!MovePressed() && speed > walkSpeed) speed = walkSpeed;
    }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        if (!Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable && (MovePressed() || currentState == MovementState.WallJump)) {
            Transition(MovementState.WallRun);
        }
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
        switch (currentState)
        {
            case MovementState.WallRun:
                currentRotation = Quaternion.Euler(0, lookDirection, 0);
                break;
            default:
                currentRotation = Quaternion.Euler(0, yRotation, 0);
                break;
        }
        
    }
}
