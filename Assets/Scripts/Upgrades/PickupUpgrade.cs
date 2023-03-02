using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupUpgrade : MonoBehaviour, IDamageable
{
    List<Transform> upgradeBoxes = new List<Transform>();
    public Upgrade upgrade;
    Transform parent;
    bool upgraded = false;

    private void Start()
    {
        parent = transform.parent;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            upgradeBoxes.Add(transform.parent.GetChild(i));
        }
    }
    public void Damaged(float amount, object collision)
    {
        if (!upgraded)
        {
            upgraded = true;
            UpgradeManager.ActivateUpgrade(upgrade);
            StartCoroutine(Destroys());
        }
        
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
