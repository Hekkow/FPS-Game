using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    Player player;
    Transform groundCheck;
    Rigidbody rb;
    LayerMask groundMask;
    RaycastHit slopeHit;
    Vector3 direction;
    bool grounded;
    float gravity;
    int jumps = 1;
    bool jumping;
    
    PlayerInputAction playerInput;
    InputAction movementInput;
    bool jumpHeld = false;
    bool canJump = true;

    void Awake()
    {
        player = GetComponent<Player>();
        groundCheck = transform.Find("Ground Check");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        groundMask = LayerMask.GetMask("Ground");
        playerInput = new PlayerInputAction();
        rb.drag = player.groundDrag;

    }
    void OnEnable()
    {
        movementInput = playerInput.Player.Move;
        movementInput.Enable();

        playerInput.Player.Jump.performed += JumpPressed;
        playerInput.Player.Jump.canceled += NotJumpPressed;
        playerInput.Player.Jump.Enable();

    }
    void OnDisable()
    {
        movementInput.Disable();
        playerInput.Player.Jump.Disable();
    }

    void FixedUpdate()
    {
        Move();
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
        grounded = Physics.CheckSphere(groundCheck.position, player.groundDistance, groundMask);
        DragControl();
        SpeedControl();
        ApplyGravity();
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
        //direction = transform.forward * moveInput.y + transform.right * moveInput.x;
        //rb.AddForce(direction.normalized * player.speed, ForceMode.Force);

        //Vector2 moveInput = movementInput.ReadValue<Vector2>();

        //Debug.Log(moveInput);
        //bool sloped = OnSlope();
        //if (moveInput.x == 0 && moveInput.y == 0)
        //{
        //    rb.velocity = new Vector3(0, rb.velocity.y, 0);
        //}
        //if (sloped)
        //{
        //    if (moveInput.y == 0 && moveInput.x == 0 && !jumpHeld)
        //    {
        //        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //        rb.velocity = new Vector3(0, 0, 0);
        //    }
        //    else
        //    {
        //        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //        Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        //        rb.AddForce(slopeMoveDirection * player.speed, ForceMode.Force);
        //    }
        //    rb.AddForce(Vector3.down * 80, ForceMode.Force);
        //}
        //else
        //{
        //    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //}
        if (moveInput.y == 0 && moveInput.x == 0 && !grounded) // && !sloped
        {
            rb.velocity = new (0, rb.velocity.y, 0);
            //direction = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            //rb.AddForce(direction.normalized * -(player.speed * 0.1f), ForceMode.Force);
        }
        else
        {
            direction = transform.forward * moveInput.y + transform.right * moveInput.x;
            rb.AddForce(direction.normalized * player.speed, ForceMode.Force);
        }
        //Debug.Log(rb.velocity);
    }

    void ApplyGravity()
    {
        
        if (rb.velocity.y > player.gravitySwitchY)
        {
            gravity = player.gravityUp;
            if (jumpHeld)
            {
                gravity /= player.gravityJumpHeld;
            }
        }
        else
        {
            gravity = player.gravityDown;
        }
        
        rb.velocity += new Vector3(0, -1 * (gravity * Time.deltaTime), 0);
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, player.playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < player.maxSlopeAngle && angle != 0;
        }
        return false;
    }
    
}