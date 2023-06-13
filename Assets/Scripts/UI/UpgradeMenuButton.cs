using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeMenuButton : MonoBehaviour, IPointerClickHandler
{
    public Upgrade upgrade;
    public UpgradeSlot upgradeSlot;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("left click");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            UpgradeManager.DeactivateUpgrade(upgrade, upgradeSlot);
            Destroy(gameObject);
        }
    }
    public void Init(Upgrade upgrade, UpgradeSlot upgradeSlot)
    {
        this.upgrade = upgrade;
        this.upgradeSlot = upgradeSlot;
    }
}
