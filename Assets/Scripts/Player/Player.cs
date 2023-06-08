using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static PlayerSlot slot = new PlayerSlot();
    [Header("Mouse Sensitivity")]
    public float sensitivity;
    public float controllerSensitivity;

    [Header("Items")]
    public float throwForce;
    public float throwDamage;
    public float throwStartDistance;
    public float throwKnockback;
    public float pickupDistance;
    public float dotProductLimit;

    [Header("Speed")]

    public float speed;
    public float maxSpeed;
    public float groundDrag;
    public float airDrag;

    [Header("Jump")]

    public float gravityUp;
    public float gravityDown;
    public float gravityJumpHeld;
    public float jumpForce;
    public float gravitySwitchY;
    public int maxJumps;

    [Header("Dash")]
    public bool canDash = false;
    public float dashTime;
    public float dashForce;

    [Header("Upgrades")]
    public int maxUpgrades = 3;

    [Header("Ground Check")]

    public float groundDistance;

    [Header("Slope Check")]
    public float playerHeight;
    public float maxSlopeAngle;

    [Header("Punch")]
    public float punchDamage;

}
