using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeInfo
{
    [SerializeReference, SubclassPicker] public Upgrade upgrade;
    public int amount = 0;
    public void ChangeAmount(int changeBy)
    {
        amount += changeBy;
    }
    public UpgradeInfo(Upgrade upgrade, int amount)
    {
        this.upgrade = upgrade;
        this.amount = amount;
    }
    public override string ToString()
    {
        return $"{amount} x {upgrade}";
    }
}
public class UpgradeManager : MonoBehaviour
{
    public static List<Upgrade> allUpgrades;
    public static GunSlot[] allGunSlots = new GunSlot[3];
    public static event Action onUpgrade; 
    
    void Start()
    {
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
        for (int i = 0; i < allGunSlots.Length; i++) {
            allGunSlots[i] = new GunSlot();
        }
    }
    public static bool ActivateUpgrade(Upgrade upgrade, int amount = 1)
    {
        if (upgrade == null) return false;
        int index = FindSlot(Inventory.guns[0].gunSlot);
        for (int i = 0; i < amount; i++)
        {
            int upgradeIndex = HasUpgrade(upgrade, allGunSlots[index]);
            if (upgradeIndex == -1)
            {
                upgrade.Activate();
                UpgradeInfo upgradeInfo = new UpgradeInfo(upgrade, 1);
                allGunSlots[index].upgrades.Add(upgradeInfo);
            }
            else if (allGunSlots[index].upgrades[upgradeIndex].amount < upgrade.maxAmount)
            {
                upgrade.Activate();
                allGunSlots[index].upgrades[upgradeIndex].ChangeAmount(1);
            }
            else return false;
        }
        Inventory.ResetBulletsAfterUpgrade();
        onUpgrade?.Invoke();
        return true;
    }
    public static void DeactivateUpgrade(Upgrade upgrade, UpgradeSlot upgradeSlot, int amount = 1)
    {
        if (upgrade == null) return;
        int upgradeIndex = HasUpgrade(upgrade, upgradeSlot);
        if (upgradeIndex == -1) return;
        for (int i = 0; i < amount; i++)
            upgrade.Deactivate(upgradeSlot);
        upgradeSlot.upgrades[upgradeIndex].ChangeAmount(-amount);
        if (upgradeSlot.upgrades[upgradeIndex].amount <= 0) upgradeSlot.upgrades.RemoveAt(upgradeIndex);
        Inventory.ResetBulletsAfterUpgrade();
        onUpgrade?.Invoke();
    }
    public static int HasUpgrade(Upgrade upgrade, UpgradeSlot upgradeSlot)
    {
        for (int i = 0; i < upgradeSlot.upgrades.Count; i++)
        {
            if (upgradeSlot.upgrades[i].upgrade.upgradeName == upgrade.upgradeName) return i;
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
    public static void PrintAllUpgrades()
    {
        for (int i = 0; i < allUpgrades.Count; i++)
        {
            Debug.Log(allUpgrades[i].upgradeName);
        }
    }
    public static void PrintOwnedUpgrades()
    {
        //for (int i = 0; i < playerSlot.upgrades.Count; i++)
        //{
        //    Debug.Log(playerSlot.upgrades[i].upgrade.upgradeName + " x " + playerSlot.upgrades[i].amount);
        //}
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
        DeactivateLockedUpgrades(switchFrom);
        GunSlot gunSlot = switchFrom.gunSlot;
        switchFrom.gunSlot = null;
        switchTo.gunSlot = gunSlot;
        int slotIndex = FindSlot(gunSlot);
        allGunSlots[slotIndex].gun = switchTo;
        switchFrom.gunSlot = null;
    }
    public static void GetFirstOpenSlot(Gun gun)
    {
        int index = -1;
        for (int i = 0; i < allGunSlots.Length; i++)
            if (allGunSlots[i].gun == null)
            {
                index = i;
                break;
            }
        gun.gunSlot = allGunSlots[index];
        if (allGunSlots[index].gun != null) allGunSlots[index].gun.gunSlot = null;
        allGunSlots[index].gun = gun;
        ActivateLockedUpgrades(gun);
        
    }
    public static void ActivateLockedUpgrades(Gun gun)
    {
        for (int i = 0; i < gun.lockedUpgrades.upgrades.Count; i++)
            ActivateUpgrade(gun.lockedUpgrades.upgrades[i].upgrade, gun.lockedUpgrades.upgrades[i].amount);
    }
    public static void DeactivateLockedUpgrades(Gun gun)
    {
        for (int i = 0; i < gun.lockedUpgrades.upgrades.Count; i++)
            DeactivateUpgrade(gun.lockedUpgrades.upgrades[i].upgrade, gun.gunSlot, gun.lockedUpgrades.upgrades[i].amount);
    }
    public static void DropGun(Gun gun)
    {
        int index = -1;
        for (int i = 0; i < allGunSlots.Length; i++)
        {
            if (gun.gunSlot == allGunSlots[i]) 
            {
                index = i;
                
                break;
            }
        }
        DeactivateLockedUpgrades(gun);
        allGunSlots[index].gun.gunSlot = null;
        allGunSlots[index].gun = null;
        (allGunSlots[0], allGunSlots[1], allGunSlots[2]) = (allGunSlots[1], allGunSlots[2], allGunSlots[0]);
    }
    public static int FindSlot(UpgradeSlot upgradeSlot)
    {
        for (int i = 0; i < allGunSlots.Length; i++)
        { 
            if (upgradeSlot.gun != null && allGunSlots[i].gun == upgradeSlot.gun) return i; 
        }
        return -1;
    }

}

