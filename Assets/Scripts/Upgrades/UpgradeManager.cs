using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static List<UpgradeInfo> ownedUpgrades;
    public static List<Upgrade> allUpgrades;
    public static event Action onUpgrade;
    public class UpgradeInfo
    {
        public Upgrade upgrade;
        public int slot;
        public int amount = 0; 
        public void SetSlot(int slot)
        {
            this.slot = slot;
        }
        public void ChangeAmount(int changeBy)
        {
            this.amount += changeBy;
        }
        public UpgradeInfo(Upgrade upgrade, int slot, int amount)
        {
            this.upgrade = upgrade;
            this.slot = slot;
            this.amount = amount;
        }
    }
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
            int upgradeIndex = HasUpgrade(upgrade);
            if (upgradeIndex == -1)
            {
                upgrade.Activate();
                if (upgrade.category == Upgrade.Category.Gun) ownedUpgrades.Add(new UpgradeInfo(upgrade, Inventory.guns[0].slot, 1));
                else ownedUpgrades.Add(new UpgradeInfo(upgrade, -1, 1));
                Inventory.ResetBulletsAfterUpgrade();
                onUpgrade?.Invoke();
                return true;
            }
            else if (ownedUpgrades[upgradeIndex].amount < upgrade.maxAmount)
            {
                upgrade.Activate(); 
                ownedUpgrades[upgradeIndex].ChangeAmount(1);
                Inventory.ResetBulletsAfterUpgrade();
                onUpgrade?.Invoke();
                return true;
            }
        }
        return false;
    }
    public static int HasUpgrade(Upgrade upgrade)
    {
        for (int i = 0; i < ownedUpgrades.Count; i++)
        {
            if (ownedUpgrades[i].upgrade == upgrade)
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
            if (ownedUpgrades[i].upgrade.category == Upgrade.Category.Gun)
            {
                Debug.Log(ownedUpgrades[i].upgrade.upgradeName + " at slot " + ownedUpgrades[i].slot + " x " + ownedUpgrades[i].amount);
            }
            else
            {
                Debug.Log(ownedUpgrades[i].upgrade.upgradeName + " x " + ownedUpgrades[i].amount);
            }
        }
    }
}
