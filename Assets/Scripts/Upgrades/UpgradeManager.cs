using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> allUpgrades;
    Player player;
    void Start()
    {
        allUpgrades = Resources.LoadAll("Scriptables/Upgrades", typeof(Upgrade)).Cast<Upgrade>().ToList();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        Invoke(upgrade.functionName, 0);
    }

    void DoubleHealth()
    {
        GameObject.Find("Player").GetComponent<Health>().MultiplyMaxHealth(2);
    }
    void BulletSpeed()
    {
        player.bulletSpeedMultiplier *= 2;
    }
}
