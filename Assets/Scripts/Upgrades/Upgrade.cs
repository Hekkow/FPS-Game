using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade
{
    public enum Category  {
        Gun,
        Mobility,
        Health
    }
    public string upgradeName;
    public int maxAmount;
    public Category category;
    public Gun gun;
    
    public abstract void Activate();
    public abstract void Deactivate();
    
}
public class AttackSpeed : Upgrade
{
    public AttackSpeed(params float[] parameters)
    {
        upgradeName = "Attack Speed";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].attackSpeed *= 2;
    }
    public override void Deactivate()
    {
        Inventory.guns[0].attackSpeed /= 2;
    }
}
public class BulletDamage : Upgrade
{
    public BulletDamage(params float[] parameters)
    {
        upgradeName = "Bullet Damage";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].bulletDamage *= 2;
    }
    public override void Deactivate()
    {
        //Inventory.guns[0].bulletDamage /= 2;
    }
}
public class BulletSpeed : Upgrade
{
    public BulletSpeed(params float[] parameters)
    {
        upgradeName = "Bullet Speed";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].bulletSpeed *= 2;
    }
    public override void Deactivate()
    {
        //UpgradeManager.bulletDamageMultiplier /= 2;
    }
}
public class BulletsPerShot : Upgrade
{
    public BulletsPerShot(params float[] parameters)
    {
        upgradeName = "Bullets Per Shot";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].bulletsPerShot += 8;
        Inventory.guns[0].bulletsPerMag += 8 * Inventory.guns[0].bulletsPerMag;
    }
    public override void Deactivate()
    {
        //UpgradeManager.bulletDamageMultiplier /= 2;
    }
}

public class GravityFlip : Upgrade
{
    public GravityFlip(params float[] parameters)
    {
        upgradeName = "Gravity Flip";
        maxAmount = 1;
    }
    public override void Activate()
    {
        Inventory.guns[0].gravityFlip = true;
    }
    public override void Deactivate()
    {
        //UpgradeManager.bulletDamageMultiplier /= 2;
    }
}

public class HealthBoost : Upgrade
{
    public HealthBoost(params float[] parameters)
    {
        upgradeName = "Health Boost";
        maxAmount = 100;
        category = Category.Health;
    }
    public override void Activate()
    {
        GameObject.Find("Player").GetComponent<Health>().MultiplyMaxHealth(2);
    }
    public override void Deactivate()
    {
        GameObject.Find("Player").GetComponent<Health>().MultiplyMaxHealth(1f/2f); 

    }
}