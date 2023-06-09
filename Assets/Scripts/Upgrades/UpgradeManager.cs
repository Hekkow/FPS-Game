using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInfo
{
    public Upgrade upgrade;
    public UpgradeSlot upgradeSlot;
    public int amount = 0;
    public void ChangeAmount(int changeBy)
    {
        this.amount += changeBy;
    }
    public UpgradeInfo(Upgrade upgrade, int amount, UpgradeSlot upgradeSlot)
    {
        this.upgrade = upgrade;
        this.amount = amount;
        this.upgradeSlot = upgradeSlot;
    }
}
public class GunUpgrades
{
    public Gun gun;
    public GunSlot gunSlot;
    public bool used;
    public GunUpgrades(Gun gun, GunSlot gunSlot, bool used)
    {
        this.gun = gun;
        this.gunSlot = gunSlot;
        this.used = used;
    }
}
public class UpgradeManager : MonoBehaviour
{
    public static PlayerSlot playerSlot;
    public static GunUpgrades[] gunSlots = new GunUpgrades[3];
    public static List<Upgrade> allUpgrades;
    public static event Action onUpgrade;
    
    void Start()
    {
        playerSlot = new PlayerSlot(); 
        for (int i = 0; i < 3; i++)
        {
            gunSlots[i] = new GunUpgrades(null, new GunSlot(), false);
        }
        allUpgrades = new List<Upgrade> 
        {
            new AttackSpeed(),
            new BulletDamage(),
            new Pellets(),
            new HealthBoost(),
            new ReloadSpeed(),
            new Minigun(),
            new Dash(),
            new Bouncer(),
        };
    }
    public static bool ActivateUpgrade(Upgrade upgrade)
    {
        if (upgrade != null)
        {
            UpgradeSlot upgradeSlot;
            if (upgrade.category == Upgrade.Category.Gun) upgradeSlot = Inventory.guns[0].upgradeSlot;
            else upgradeSlot = Player.slot;
            int upgradeIndex = HasUpgrade(upgrade, upgradeSlot);
            if (upgradeIndex == -1)
            {
                upgrade.Activate();
                UpgradeInfo upgradeInfo = new UpgradeInfo(upgrade, 1, upgradeSlot);
                upgradeSlot.upgrades.Add(upgradeInfo);
            }
            else if (upgradeSlot.upgrades[upgradeIndex].amount < upgrade.maxAmount)
            {
                upgrade.Activate();
                upgradeSlot.upgrades[upgradeIndex].ChangeAmount(1);
            }
            else return false;
            Inventory.ResetBulletsAfterUpgrade();
            onUpgrade?.Invoke();
            return true;
        }
        return false;
    }
    public static void DeactivateUpgrade(Upgrade upgrade)
    {
        if (upgrade != null)
        {
            UpgradeSlot upgradeSlot;
            if (upgrade.category == Upgrade.Category.Gun) upgradeSlot = Inventory.guns[0].upgradeSlot;
            else upgradeSlot = Player.slot;
            int upgradeIndex = HasUpgrade(upgrade, upgradeSlot);
            if (upgradeIndex == -1) return;
            upgrade.Deactivate(upgradeSlot);
            upgradeSlot.upgrades[upgradeIndex].ChangeAmount(-1);
            if (upgradeSlot.upgrades[upgradeIndex].amount <= 0) upgradeSlot.upgrades.RemoveAt(upgradeIndex);
            Inventory.ResetBulletsAfterUpgrade();
            onUpgrade?.Invoke();
        }
    }
    public static int HasUpgrade(Upgrade upgrade, UpgradeSlot upgradeSlot)
    {
        for (int i = 0; i < upgradeSlot.upgrades.Count; i++)
        {
            if (upgradeSlot.upgrades[i].upgrade == upgrade) return i;
        }
        return -1;
    }
    //public static int HasUpgrade(Upgrade upgrade)
    //{
    //    for (int i = 0; i < ownedUpgrades.Count; i++)
    //    {
    //        if (ownedUpgrades[i].upgrade == upgrade && ownedUpgrades[i].upgradeSlot == Inventory.guns[0].upgradeSlot || ownedUpgrades[i].upgradeSlot == Player.slot)
    //        {
    //            return i;
    //        }
    //    }
    //    return -1;
    //}
    public static Upgrade FindUpgrade(string name)
    {
        Upgrade upgrade = null;
        for (int i = 0; i < allUpgrades.Count; i++)
        {
            if (allUpgrades[i].upgradeName.ToLower().Replace(" ", "") == name.ToLower())
            {
                upgrade = allUpgrades[i];
            }
        }
        return upgrade;
    }
    //public static List<Upgrade> RandomUpgrades(int amount)
    //{
    //    List<Upgrade> list = new List<Upgrade>();
    //    int counter = 0;
    //    while (list.Count < amount && counter < 200)
    //    {
    //        int randomNumber = UnityEngine.Random.Range(0, allUpgrades.Count); 
    //        Upgrade upgrade = allUpgrades[randomNumber];
    //        if (HasUpgrade(upgrade) == -1 && !list.Contains(upgrade))
    //        {
    //            list.Add(upgrade);

    //        }
    //        counter++;
    //    }
    //    return list;
    //}
    public static void PrintAllUpgrades()
    {
        for (int i = 0; i < allUpgrades.Count; i++)
        {
            Debug.Log(allUpgrades[i].upgradeName);
        }
    }
    public static void PrintOwnedUpgrades()
    {
        for (int i = 0; i < playerSlot.upgrades.Count; i++)
        {
            Debug.Log(playerSlot.upgrades[i].upgrade.upgradeName + " x " + playerSlot.upgrades[i].amount);
        }
        //for (int i = 0; i < usedSlots.Count; i++)
        //{
        //    Debug.Log("Gun slot " + (i + 1));
        //    for (int j = 0; j < usedSlots[i].gunSlot.upgrades.Count; j++)
        //    {
        //        UpgradeInfo upgradeInfo = usedSlots[i].gunSlot.upgrades[j];
        //        Debug.Log(upgradeInfo.upgrade.upgradeName + " x " + upgradeInfo.amount);
        //    }
        //}
    }
    public static void SwitchGuns(Gun switchFrom, Gun switchTo)
    {
        GunSlot gunSlot = switchFrom.upgradeSlot;
        switchFrom.upgradeSlot = null;
        switchTo.upgradeSlot = gunSlot;
        int slotIndex = FindSlot(gunSlot);
        gunSlots[slotIndex].gun = switchTo;
    }
    public static void GetFirstOpenSlot(Gun gun)
    {
        int index = -1;
        for (int i = 0; i < gunSlots.Length; i++)
        {
            if (gun.upgradeSlot == gunSlots[i].gunSlot)
            {
                gunSlots[i].used = true;
                return;
            }
        }
        for (int i = 0; i < gunSlots.Length; i++)
        {
            if (!gunSlots[i].used)
            {
                index = i;
                break;
            }
        }

        gun.upgradeSlot = gunSlots[index].gunSlot;
        if (gunSlots[index].gun != null) gunSlots[index].gun.upgradeSlot = null;
        gunSlots[index].gun = gun;
        gunSlots[index].used = true;
    }
    public static void DropGun(Gun gun)
    {
        for (int i = 0; i < gunSlots.Length; i++)
        {
            if (gun.upgradeSlot == gunSlots[i].gunSlot)
            {
                gunSlots[i].used = false;
                break;
            }
        }
        (gunSlots[0], gunSlots[1], gunSlots[2]) = (gunSlots[1], gunSlots[2], gunSlots[0]);
    }
    public static int FindSlot(UpgradeSlot upgradeSlot)
    {
        for (int i = 0; i < gunSlots.Length; i++) {
            if (gunSlots[i].gunSlot == upgradeSlot) return i;
        }
        return -1;
    }
}

