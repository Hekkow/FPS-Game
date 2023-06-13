using System;
using UnityEngine;
[Serializable]
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
    public abstract void Activate();
    public abstract void Deactivate(UpgradeSlot slot);
}
public class AttackSpeed : Upgrade
{
    public AttackSpeed()
    {
        upgradeName = "Attack Speed";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].gunSlot.attackSpeed *= 2;
    }
    public override void Deactivate(UpgradeSlot slot)
    {
        GunSlot gunSlot = slot as GunSlot;
        gunSlot.attackSpeed /= 2;
    }
}
public class BulletDamage : Upgrade
{
    public BulletDamage()
    {
        upgradeName = "Bullet Damage";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].gunSlot.bulletDamage *= 2;
    }
    public override void Deactivate(UpgradeSlot slot) { }
}
public class ReloadSpeed : Upgrade
{
    public ReloadSpeed()
    {
        upgradeName = "Reload Speed";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Inventory.guns[0].gunSlot.reloadSpeed *= 2;
    }
    public override void Deactivate(UpgradeSlot slot) { }
}
public class Dash : Upgrade
{
    public Dash()
    {
        upgradeName = "Dash";
        maxAmount = 100;
        category = Category.Mobility; 
    }
    public override void Activate()
    {
        GameObject.Find("Player").GetComponent<Player>().canDash = true;
    }
    public override void Deactivate(UpgradeSlot slot) { }
}
public class Pellets : Upgrade
{
    public Pellets()
    {
        upgradeName = "Pellets";
        maxAmount = 100;
    }
    public override void Activate()
    {
        GunSlot gun = Inventory.guns[0].gunSlot;
        gun.pelletLayers += 1;
        if (gun.pelletsPerLayer.Count == 0) gun.pelletsPerLayer.Add(8);
        else gun.pelletsPerLayer.Add((int)(gun.pelletsPerLayer[gun.pelletsPerLayer.Count - 1] * 1.5f));
        gun.bulletsPerShot += gun.pelletsPerLayer[gun.pelletsPerLayer.Count - 1];
        gun.shotsPerMag -= 1;
        if (gun.shotsPerMag <= 3) gun.shotsPerMag = 3;
        gun.bulletsPerMag = gun.bulletsPerShot * gun.shotsPerMag;
    }
    public override void Deactivate(UpgradeSlot slot) {
        GunSlot gunSlot = slot as GunSlot;
        //gunSlot.pelletLayers -= 1;
        //gunSlot.bulletsPerShot -= gunSlot.pelletsPerLayer[gunSlot.pelletLayers];
        //gunSlot.shotsPerMag += 1;
        //gunSlot.bulletsPerMag = gunSlot.bulletsPerShot * gunSlot.shotsPerMag;
    }
}
public class Minigun : Upgrade
{
    public Minigun()
    {
        upgradeName = "Minigun";
        maxAmount = 100;
    }
    public override void Activate()
    {
        Gun gun = Inventory.guns[0];
        if (gun.gunSlot.bulletSpread <= 0) gun.gunSlot.bulletSpread = 0.5f;
        else gun.gunSlot.bulletSpread *= 1.3f;
        gun.gunSlot.attackSpeed *= 3;
        gun.gunSlot.bulletsPerMag *= 2;
        gun.gunSlot.shotsPerMag *= 2;
        gun.gunSlot.bulletSize /= 1.5f;
        gun.gunSlot.reloadSpeed *= 0.9f;
        gun.gunSlot.bulletDamage *= 0.9f;
    }
    public override void Deactivate(UpgradeSlot slot) { }
}
public class HealthBoost : Upgrade
{
    public HealthBoost()
    {
        upgradeName = "Health Boost";
        maxAmount = 100;
        category = Category.Health;
    }
    public override void Activate()
    {
        Health health = GameObject.Find("Player").GetComponent<Health>();
        health.maxHealth *= 2;
        health.health *= 2; 
    }
    public override void Deactivate(UpgradeSlot slot) {
        Health health = GameObject.Find("Player").GetComponent<Health>();
        health.maxHealth /= 2;
        if (health.maxHealth < health.health) health.health = health.maxHealth;
    }
}
public class Bouncer : Upgrade
{
    public Bouncer()
    {
        upgradeName = "Bouncer";
        maxAmount = 1;
        category = Category.Gun;
    }
    public override void Activate()
    {
        Inventory.guns[0].gunSlot.bouncer = true;
    }
    public override void Deactivate(UpgradeSlot slot) {
        GunSlot gunSlot = slot as GunSlot;
        gunSlot.bouncer = false;
    }
}