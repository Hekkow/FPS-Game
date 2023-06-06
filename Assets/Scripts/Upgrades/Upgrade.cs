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
        Gun gun = Inventory.guns[0];
        gun.pelletLayers += 1;
        if (gun.pelletsPerLayer.Count == 0) gun.pelletsPerLayer.Add(8);
        else gun.pelletsPerLayer.Add((int)(gun.pelletsPerLayer[gun.pelletsPerLayer.Count - 1] * 1.5f));
        gun.bulletsPerShot += gun.pelletsPerLayer[gun.pelletsPerLayer.Count - 1];
        gun.shotsPerMag -= 1;
        if (gun.shotsPerMag <= 3) gun.shotsPerMag = 3;
        gun.bulletsPerMag = gun.bulletsPerShot * gun.shotsPerMag;
        if (gun.pelletSpread <= 0) gun.pelletSpread = 0.1f;
        else gun.pelletSpread *= 0.9f;
        gun.bulletSize /= 1.5f;
        gun.reloadSpeed *= 0.9f;
        gun.bulletDamage *= 0.9f;
    }
}
public class Minigun : Upgrade
{
    public Minigun(params float[] parameters)
    {
        upgradeName = "Minigun";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Gun gun = Inventory.guns[0];
        if (gun.bulletSpread <= 0) gun.bulletSpread = 0.5f;
        else gun.bulletSpread *= 1.3f;
        gun.attackSpeed *= 3;
        gun.bulletsPerMag *= 2;
        gun.shotsPerMag *= 2;
        gun.bulletSize /= 1.5f;
        gun.reloadSpeed *= 0.9f;
        gun.bulletDamage *= 0.9f;
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
        GameObject.Find("Player").GetComponent<Health>().health *= 2;
        GameObject.Find("Player").GetComponent<Health>().maxHealth *= 2;

    }
}
public class Bouncer : Upgrade
{
    public Bouncer(params float[] parameters)
    {
        upgradeName = "Bouncer";
        maxAmount = 1;
        category = Category.Gun;
    }
    public override void Activate()
    {
        Inventory.guns[0].bouncer = true;
    }
}