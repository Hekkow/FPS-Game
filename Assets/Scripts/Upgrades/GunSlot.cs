using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GunSlot : UpgradeSlot
{
    //public int bulletsPerShot = 1;
    //public int bulletsPerTap = 1;
    //public float bulletSize = 0.1f;
    //public float bulletSpeed = 200;
    //public float bulletDamage = 34;
    //public float bulletKnockback = 5000000;
    //public float attackSpeed = 1;
    //public int pelletLayers = 0;
    //public float pelletSpread = 0;
    //public List<int> pelletsPerLayer = new List<int>();
    //public float bulletSpread = 0;
    //public float reloadSpeed = 1f;
    //public int bulletsPerMag = 6;
    //public int bulletsLeft = 6;
    //public int shotsPerMag = 6;
    //public bool bouncer = false;
    [HideInInspector] public int bulletsPerShot = 1;
    [HideInInspector] public int bulletsPerTap = 1;
    [HideInInspector] public float bulletSize = 0.1f;
    [HideInInspector] public float bulletSpeed = 200;
    [HideInInspector] public float bulletDamage = 34;
    [HideInInspector] public float bulletKnockback = 5000000;
    [HideInInspector] public float attackSpeed = 1;
    [HideInInspector] public int pelletLayers = 0;
    [HideInInspector] public float pelletSpread = 0;
    [HideInInspector] public List<int> pelletsPerLayer = new List<int>();
    [HideInInspector] public float bulletSpread = 0;
    [HideInInspector] public float reloadSpeed = 1f;
    [HideInInspector] public int bulletsPerMag = 6;
    [HideInInspector] public int bulletsLeft = 6;
    [HideInInspector] public int shotsPerMag = 6;
    [HideInInspector] public bool bouncer = false;
}
