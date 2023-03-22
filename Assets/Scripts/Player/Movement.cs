using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    InputAction movementInput;
    Player player;
    Rigidbody rb;
    Vector3 direction;
    int jumps = 0;
    float gravity;
    bool grounded;
    bool jumping;
    bool applyingGravity = true;
    bool jumpHeld = false;
    bool canJump = true;
    bool dashing = false;

    void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = player.groundDrag;

    }
    void OnEnable()
    {
        movementInput = InputManager.playerInput.Player.Move;
        movementInput.Enable();

        InputManager.playerInput.Player.Jump.performed += JumpPressed;
        InputManager.playerInput.Player.Jump.canceled += NotJumpPressed;
        InputManager.playerInput.Player.Jump.Enable();

        InputManager.playerInput.Player.Dash.performed += Dash;
        InputManager.playerInput.Player.Dash.Enable();
    }
    void OnDisable()
    {
        movementInput.Disable();
        InputManager.playerInput.Player.Jump.Disable();
        InputManager.playerInput.Player.Dash.Disable();
    }
    void FixedUpdate()
    {
        Move();
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, player.groundDistance + 1)) grounded = true;
        else grounded = false;
        if (jumpHeld)
        {
            if (canJump)
            {
                Jump();
                canJump = false;
            }
        }
    }
    void Update()
    {
        DragControl();
        SpeedControl();
        if (applyingGravity)
        {
            ApplyGravity();
        }
    }
    void Jump()
    {
        StopCoroutine(ResetJump());
        StartCoroutine(ResetJump());
    }
    void JumpPressed(InputAction.CallbackContext obj)
    {
        jumpHeld = true;
    }
    void NotJumpPressed(InputAction.CallbackContext obj)
    {
        jumpHeld = false;

    }
    IEnumerator ResetJump()
    {

        jumping = true;
        jumps++;
        canJump = false;
        dashing = false;
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(transform.up * player.jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        while (jumpHeld && !grounded) {
            yield return new WaitForEndOfFrame();
        }
        if (jumps < player.maxJumps)
        {
            canJump = true;
        }
        yield return new WaitUntil(() => grounded);
        canJump = true;
        jumps = 0;
        jumping = false;
    }
    void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVelocity.magnitude > player.maxSpeed)
        {
            Vector3 newVelocity = flatVelocity.normalized * player.maxSpeed;
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }
    }
    void DragControl()
    {
        if (!grounded || jumping) rb.drag = player.airDrag;
        else rb.drag = player.groundDrag;
    }
    void Move()
    {

        Vector2 moveInput = movementInput.ReadValue<Vector2>();
        if (moveInput.y == 0 && moveInput.x == 0 && !grounded)
        {
            rb.velocity = new (0, rb.velocity.y, 0);
        }
        else
        {
            direction = transform.forward * moveInput.y + transform.right * moveInput.x;
            rb.AddForce(direction.normalized * player.speed, ForceMode.Force);
        }
    }
    void ApplyGravity()
    {
        if (rb.velocity.y > player.gravitySwitchY)
        {
            gravity = player.gravityUp;
        }
        else
        {
            gravity = player.gravityDown;
        }
        
        rb.velocity += new Vector3(0, -1 * (gravity * Time.deltaTime), 0);
    }
    void Dash(InputAction.CallbackContext obj)
    {
        if (player.canDash)
        {
            StartCoroutine(StartDash());
        }
    }
    IEnumerator StartDash()
    {
        Vector2 moveInput = movementInput.ReadValue<Vector2>();
        dashing = true;
        applyingGravity = false;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        Vector3 dashDirection;
        if (moveInput.x == 0 && moveInput.y == 0)
            dashDirection = transform.forward;
        else
        {
            dashDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        }
        float timeStarted = Time.time;
        while (Time.time - timeStarted < player.dashTime && dashing)
        {
            rb.AddForce(dashDirection.normalized * player.dashForce * Time.deltaTime, ForceMode.Force);
            yield return new WaitForEndOfFrame();
        }
        rb.useGravity = true;
        applyingGravity = true;
        dashing = false;
    }
}
