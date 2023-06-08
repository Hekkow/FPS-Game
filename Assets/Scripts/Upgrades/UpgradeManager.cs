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

public class UpgradeManager : MonoBehaviour
{
    public static List<UpgradeInfo> ownedUpgrades;
    public static List<Upgrade> allUpgrades;
    public static event Action onUpgrade;
    
    void Start()
    {
        ownedUpgrades = new List<UpgradeInfo>();
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
                ownedUpgrades.Add(new UpgradeInfo(upgrade, 1, upgradeSlot));
            }
            else if (ownedUpgrades[upgradeIndex].amount < upgrade.maxAmount)
            {
                upgrade.Activate();
                ownedUpgrades[upgradeIndex].ChangeAmount(1);
                if (ownedUpgrades[upgradeIndex].amount <= 0) ownedUpgrades.RemoveAt(upgradeIndex);
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
            if (upgradeIndex == -1)
            {
                return;
            }
            upgrade.Deactivate(upgradeSlot);
            ownedUpgrades[upgradeIndex].ChangeAmount(-1);
            if (ownedUpgrades[upgradeIndex].amount <= 0) ownedUpgrades.RemoveAt(upgradeIndex);
            Inventory.ResetBulletsAfterUpgrade();
            onUpgrade?.Invoke();
        }
    }
    public static int HasUpgrade(Upgrade upgrade, UpgradeSlot upgradeSlot)
    {
        for (int i = 0; i < ownedUpgrades.Count; i++)
        {
            if (ownedUpgrades[i].upgrade == upgrade && ownedUpgrades[i].upgradeSlot == upgradeSlot)
            {
                return i;
            }
        }
        return -1;
    }
    public static int HasUpgrade(Upgrade upgrade)
    {
        for (int i = 0; i < ownedUpgrades.Count; i++)
        {
            if (ownedUpgrades[i].upgrade == upgrade && ownedUpgrades[i].upgradeSlot == Inventory.guns[0].upgradeSlot || ownedUpgrades[i].upgradeSlot == Player.slot)
            {
                return i;
            }
        }
        return -1;
    }
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
    public static List<Upgrade> RandomUpgrades(int amount)
    {
        List<Upgrade> list = new List<Upgrade>();
        int counter = 0;
        while (list.Count < amount && counter < 200)
        {
            int randomNumber = UnityEngine.Random.Range(0, allUpgrades.Count); 
            Upgrade upgrade = allUpgrades[randomNumber];
            if (HasUpgrade(upgrade) == -1 && !list.Contains(upgrade))
            {
                list.Add(upgrade);

            }
            counter++;
        }
        return list;
    }
    public static void PrintAllUpgrades()
    {
        for (int i = 0; i < allUpgrades.Count; i++)
        {
            Debug.Log(allUpgrades[i].upgradeName);
        }
    }
    public static void PrintOwnedUpgrades()
    {
        for (int i = 0; i < ownedUpgrades.Count; i++)
        {
            Debug.Log(ownedUpgrades[i].upgrade.upgradeName + " x " + ownedUpgrades[i].amount + " " + ownedUpgrades[i].upgradeSlot);
        }
    }
}

