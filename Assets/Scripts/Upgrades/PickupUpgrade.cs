using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupUpgrade : MonoBehaviour, IDamageable
{
    List<Transform> upgradeBoxes = new List<Transform>();
    public Upgrade upgrade;
    Transform parent;
    bool upgraded = false;

    public event Action onUpgradeShot;

    void Start()
    {
        parent = transform.parent;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            upgradeBoxes.Add(transform.parent.GetChild(i));
            upgradeBoxes[i].GetComponent<PickupUpgrade>().onUpgradeShot += () => upgraded = true;
        }
    }
    public void Damaged(float amount, object collision, object origin)
    {
        if (upgraded) return;
        Killed();
        //if (!upgraded && (origin is Gun || ((origin is Damage damage && damage.thrown || ((Damage)origin).punch) && upgrade.category != Upgrade.Category.Gun)))
        //{
        //    Killed();
        //}
        
    }
    public void Killed()
    {
        onUpgradeShot.Invoke();
        upgraded = true;
        UpgradeManager.ActivateUpgrade(upgrade);
        StartCoroutine(Destroys());
    }
    IEnumerator Destroys()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < upgradeBoxes.Count; i++)
        {
            if (upgradeBoxes[i].name != gameObject.name)
            {
                Destroy(upgradeBoxes[i].gameObject);
            }
        }
        yield return new WaitForSeconds(0.6f);
        Destroy(parent.gameObject);
        Destroy(gameObject);
    }
}
