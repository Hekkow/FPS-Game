using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
public class ReloadSpeed : Upgrade
{
    public ReloadSpeed(params float[] parameters)
    {
        upgradeName = "Reload Speed";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].reloadSpeed *= 2;
    }
    public override void Deactivate()
    {
        //UpgradeManager.bulletDamageMultiplier /= 2;
    }
}
public class Dash : Upgrade
{
    public Dash(params float[] parameters)
    {
        upgradeName = "Dash";
        maxAmount = 100;
        category = Category.Mobility; 
    }
    public override void Activate()
    {
        GameObject.Find("Player").GetComponent<Player>().canDash = true;
    }
    public override void Deactivate()
    {
        //UpgradeManager.bulletDamageMultiplier /= 2;
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
public class BulletSize : Upgrade
{
    public BulletSize(params float[] parameters)
    {
        upgradeName = "Bullet Size";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].bulletSize *= 1.5f;
    }
    public override void Deactivate()
    {
        //UpgradeManager.bulletDamageMultiplier /= 2;
    }
}
public class Pellets : Upgrade
{
    public Pellets(params float[] parameters)
    {
        upgradeName = "Pellets";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].pelletLayers += 1;
        if (Inventory.guns[0].pelletsPerLayer.Count == 0) Inventory.guns[0].pelletsPerLayer.Add(8);
        Inventory.guns[0].pelletsPerLayer.Add((int)(Inventory.guns[0].pelletsPerLayer[Inventory.guns[0].pelletsPerLayer.Count - 1] * 1.5f));
        Inventory.guns[0].bulletsPerShot += 8;
        Inventory.guns[0].shotsPerMag -= 1;
        if (Inventory.guns[0].shotsPerMag < 3)
        {
            Inventory.guns[0].shotsPerMag = 3;
            Inventory.guns[0].bulletsPerMag += 8 * (Inventory.guns[0].shotsPerMag);
        }
        else
        {
            Inventory.guns[0].bulletsPerMag += 8 * (Inventory.guns[0].shotsPerMag + 1) - Inventory.guns[0].bulletsPerShot;
        }
        if (Inventory.guns[0].bulletSpread <= 0) Inventory.guns[0].bulletSpread = 0.1f;
        else Inventory.guns[0].bulletSpread *= 0.9f;
        Inventory.guns[0].bulletSize /= 1.5f;
        Inventory.guns[0].reloadSpeed += 0.2f;
        Inventory.guns[0].bulletDamage *= 0.9f;
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