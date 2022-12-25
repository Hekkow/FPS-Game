using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    public string name;
    public string description;
}


public class Upgrades : MonoBehaviour
{
    Player player;
    public List<Upgrade> allUpgrades;
    public List<Upgrade> playerUpgrades;
    public int maxUpgrades;
    Health playerHealth;
    Upgrade doubleJumpUpgrade;
    Upgrade doubleHealthUpgrade;
    Upgrade attackSpeedUpgrade;
    void Start()
    {
        player = GetComponent<Player>();
        playerHealth = GetComponent<Health>();
        allUpgrades = new List<Upgrade>();
        playerUpgrades = new List<Upgrade>();
        allUpgrades.Add(doubleHealthUpgrade = new Upgrade
        {
            name = "Double Health",
            description = "doubles health"
        });
        allUpgrades.Add(attackSpeedUpgrade = new Upgrade
        {
            name = "Attack Speed",
            description = "doubles attack speed"
        });
        allUpgrades.Add(doubleJumpUpgrade = new Upgrade
        {
            name = "Double Jump",
            description = "doubles jump"
        });
    }

    public void DoubleJump()
    {
        playerUpgrades.Add(doubleJumpUpgrade);
        player.maxJumps *= 2;

    }
    public void AttackSpeed()
    {
        playerUpgrades.Add(attackSpeedUpgrade);
        player.attackSpeed *= 2;
    }
    public void DoubleHealth()
    {
        playerUpgrades.Add(doubleHealthUpgrade);
        playerHealth.MultiplyMaxHealth(2);
    }
    

}
