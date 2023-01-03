using UnityEngine;

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
    int jumps = 0;

    void Awake()
    {
        player = GetComponent<Player>();
        groundCheck = transform.Find("Ground Check");
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        groundMask = LayerMask.GetMask("Ground");
    }


    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, player.groundDistance, groundMask);

        // if either grounded or still has more jumps left
        if (grounded && !input.jump)
        {
            gravity = 0;
            jumps = 0;
        }

        // resets if grounded
        if (input.jumpDown && (grounded || OnSlope() || jumps < player.maxJumps))
        {
            Jump();
        }

        // for jumping, if going up then gravity is lower, if going down then gravity is higher

        if (rb.velocity.y < 0 || !input.jump)
        {
            gravity = player.gravityDown;
        }
        else
        {
            gravity = player.gravityUp;
        }

        AdjustDrag();
        SpeedControl();
        ApplyGravity();
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * player.jumpForce, ForceMode.Impulse);
        jumps++;
    }
    void AdjustDrag()
    {
        if (grounded)
        {
            rb.drag = player.groundDrag;
        }
        else
        {
            rb.drag = player.airDrag;
        }
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
            if (moveInput.y == 0 && moveInput.x == 0 && !input.jump)
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
