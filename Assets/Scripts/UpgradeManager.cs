using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    Player player;
    public Upgrade[] allUpgrades;
    void Start()
    {
        player = GetComponent<Player>();
        allUpgrades = Resources.LoadAll("Scriptables/Upgrades", typeof(Upgrade)).Cast<Upgrade>().ToArray();
    }
    public List<Upgrade> GetRandomFromCategory(int amount, Powerups.Category category = Powerups.Category.Any)
    {
        List<Upgrade> upgrades = new List<Upgrade>();
        while (upgrades.Count != amount)
        {
            int randomNumber = Random.Range(0, allUpgrades.Length);
            if ((allUpgrades[randomNumber].category == category || category == Powerups.Category.Any) && !upgrades.Contains(allUpgrades[randomNumber]))
            {
                upgrades.Add(allUpgrades[randomNumber]);
            }
        }
        return upgrades;
    }
    public void DoubleJump()
    {
        player.maxJumps *= 2;

    }
    public void AttackSpeed()
    {
        player.attackSpeedMultiplier *= 2;
    }
    public void DoubleHealth()
    {

    }
    

}
