using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    public float sensitivity;

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

    [Header("Upgrades")]
    public int maxUpgrades = 3;
    public float dashForce;

    [Header("Ground Check")]

    public float groundDistance;

    [Header("Slope Check")]
    public float playerHeight;
    public float maxSlopeAngle;

    [Header("Bullet Stats")]

    public float bulletSpeedMultiplier;
    public float bulletSizeMultiplier;
    public float attackSpeedMultiplier;
    public float bulletSpreadMultiplier;
    public float bulletKnockbackMultiplier;
    public int bulletsPerTapMultiplier; // burst
    public int bulletsPerShotMultiplier; // shotgun
    public int bulletDamageMultiplier;

}
