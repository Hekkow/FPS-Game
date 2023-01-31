using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]

public class Gun : ScriptableObject
{
    public new string name;
    public string description;
    public float bulletSpeed;
    public float bulletSize;
    public float attackSpeed;
    public float bulletSpread;
    public float bulletKnockback;
    public int bulletsPerTap; // burst
    public int bulletsPerShot; // shotgun
    public int bulletDamage;
    public bool allowHold;
}
