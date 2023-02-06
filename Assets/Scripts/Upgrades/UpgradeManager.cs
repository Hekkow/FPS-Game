using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static Dictionary<Upgrade, int> ownedUpgrades;
    public static List<Upgrade> allUpgrades;
    //public static List<Upgrade> ownedUpgrades;
    float[] parameters;
    Player player;
    Upgrade currentUpgrade;

    // mobility
    public static bool canDash = false;
    
    // stats
    public static float healthMultiplier = 1;
    public static float attackSpeedMultiplier = 1;
    public static float bulletDamageMultiplier = 1;
    public static float bulletSizeMultiplier = 1;
    public static float bulletSpeedMultiplier = 1;
    public static float bulletsPerShotAddition = 0;
    public static float bulletsPerTapAddition = 0;
    public static float bulletSpreadAddition = 0;

    // bullets
    public static bool gravityFlip = false;

    void Start()
    {
        ownedUpgrades = new Dictionary<Upgrade, int>();
        //ownedUpgrades = new List<Upgrade>();
        allUpgrades = Resources.LoadAll("Scriptables/Upgrades", typeof(Upgrade)).Cast<Upgrade>().ToList();
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    public Upgrade FindUpgrade(string name)
    {
        Upgrade upgrade = null;
        for (int i = 0; i < allUpgrades.Count; i++)
        {
            if (allUpgrades[i].name.ToLower().Replace(" ", "") == name.ToLower())
            {
                upgrade = allUpgrades[i];
            }
        }
        return upgrade;
    }
    public void ApplyUpgrade(Upgrade upgrade, params float[] parameters)
    {
        if (upgrade != null) {
            currentUpgrade = upgrade;
            this.parameters = parameters;
            if (ownedUpgrades.ContainsKey(upgrade))
            {
                if (ownedUpgrades[upgrade] < upgrade.maxAmount)
                {
                    ownedUpgrades[upgrade]++;
                }
            }
            else ownedUpgrades.Add(upgrade, 1);
            Invoke(upgrade.functionName, 0);
        }
        else
        {
            Debug.Log("Invalid upgrade");
        }
    }

    void Health()
    {
        GameObject.Find("Player").GetComponent<Health>().MultiplyMaxHealth(currentUpgrade.amount[0]);
    }
    void AttackSpeed()
    {
        attackSpeedMultiplier *= currentUpgrade.amount[0];
    }
    void BulletDamage()
    {
        bulletDamageMultiplier *= currentUpgrade.amount[0];
    }
    void BulletSize()
    {
        bulletSizeMultiplier *= currentUpgrade.amount[0];
    }
    void BulletSpeed()
    {
        bulletSpeedMultiplier *= currentUpgrade.amount[0];
    }
    void BulletsPerShot()
    {
        bulletsPerShotAddition += currentUpgrade.amount[0];
        bulletSpreadAddition += currentUpgrade.amount[1];
    }
    void BulletsPerTap()
    {
        bulletsPerTapAddition += currentUpgrade.amount[0];
    }
    void GravityFlip()
    {
        gravityFlip = true;
    }
}
