using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Player player;
    Transform groundCheck;
    InputManager input;
    Rigidbody rb;
    LayerMask groundMask;
    RaycastHit slopeHit;
    Vector3 direction;
    bool grounded;
    float gravity;
    int jumps = 1;
    PlayerInputAction playerInput;
    InputAction movementInput;
    bool jumpHeld = false;
    bool canJump = true;

    void Awake()
    {
        player = GetComponent<Player>();
        groundCheck = transform.Find("Ground Check");
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        groundMask = LayerMask.GetMask("Ground");
        playerInput = new PlayerInputAction();
        Debug.Log(player.gravityUp);
        Debug.Log(player.gravityDown);

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
    }

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, player.groundDistance, groundMask);
        if (jumpHeld)
        {
            if (canJump && (grounded || OnSlope() || jumps < player.maxJumps))
            {
                Jump();
            }
        }
        
        SpeedControl();
        ApplyGravity();
    }
    void Jump()
    {
        //Debug.Log(transform.up * player.jumpForce);
        rb.velocity = new Vector3(0, 0, 0);
        //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * player.jumpForce, ForceMode.Impulse);
        jumps++;
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
        canJump = false;
        rb.drag = player.airDrag;
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => grounded);
        rb.drag = player.groundDrag;
        canJump = true;
        jumps = 0;
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

    void Move()
    {

        Vector2 moveInput = input.movement;
        bool sloped = OnSlope();
        if (sloped)
        {
            if (moveInput.y == 0 && moveInput.x == 0 && jumpHeld)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                rb.velocity = new Vector3(0, 0, 0);
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
                rb.AddForce(slopeMoveDirection * player.speed, ForceMode.Force);
            }
            rb.AddForce(Vector3.down * 80, ForceMode.Force);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        if (moveInput.y == 0 && moveInput.x == 0 && !grounded && !sloped)
        {
            direction = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(direction.normalized * -(player.speed *0.5f), ForceMode.Force);
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
            //Debug.Log(player.gravityUp);
            //Debug.Log(gravity);
        }
        else
        {
            gravity = player.gravityDown;
        }
        if (jumpHeld)
        {
            gravity /= player.gravityJumpHeld;
        }
        Debug.Log(gravity);
        rb.velocity += new Vector3(0, -1 * (gravity * Time.deltaTime), 0);
        Debug.Log(new Vector3(0, -1 * (gravity * Time.deltaTime), 0));
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
