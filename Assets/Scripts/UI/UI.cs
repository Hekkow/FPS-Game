using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour {
    [HideInInspector] public static bool inMenu = false;
    [SerializeField] Transform upgrades;
    [SerializeField] Transform guns;
    [SerializeField] GameObject canvas;
    [SerializeField] float gunsDistanceFromTop;
    void OnEnable()
    {
        InputManager.playerInput.Player.Tab.performed += _ => OpenUpgrades();
        InputManager.playerInput.Player.Tab.Enable();
        InputManager.playerInput.UI.Tab.performed += _ => OpenUpgrades();
        InputManager.playerInput.UI.Tab.Enable();
    }
    void OpenUpgrades()
    {
        if (inMenu)
        {
            InputManager.SwitchActionMap(InputManager.playerInput.Player);
            upgrades.gameObject.SetActive(false);
            guns.gameObject.SetActive(false);
            guns.DestroyAllChildren();
            for (int i = 0; i < UpgradeManager.gunSlots.Length; i++)
            {
                upgrades.GetChild(i).DestroyAllChildren();
            }
            inMenu = false;
        }
        else
        {
            InputManager.SwitchActionMap(InputManager.playerInput.UI);

            upgrades.gameObject.SetActive(true);
            guns.gameObject.SetActive(true);
            float width = guns.GetComponent<RectTransform>().rect.width;
            float height = guns.GetComponent<RectTransform>().rect.height;
            for (int i = 0; i < UpgradeManager.gunSlots.Length; i++)
            {
                if (UpgradeManager.gunSlots[i].gun != null)
                {
                    Vector3 position = new Vector3((width / 3 * i) - (width / 2) + (width / 3 / 2), height / 2 - gunsDistanceFromTop, 0);
                    GameObject gun = Instantiate(UpgradeManager.gunSlots[i].gun.gameObject);
                    Destroy(gun.GetComponent<Gun>());
                    gun.transform.SetParent(guns);
                    gun.transform.localPosition = position;
                    gun.ApplyLayerToChildren("UI");
                    gun.transform.localScale = new Vector3(30, 30, 30);
                    gun.SetActive(true);
                }
                for (int j = 0; j < UpgradeManager.gunSlots[i].gunSlot.upgrades.Count; j++)
                {
                    for (int n = 0; n < UpgradeManager.gunSlots[i].gunSlot.upgrades[j].amount; n++)
                    {
                        GameObject upgradeBox = Instantiate(Resources.Load<GameObject>("Prefabs/Upgrade UI"), upgrades.GetChild(i));
                        TMP_Text text = upgradeBox.GetComponentInChildren<TMP_Text>();
                        text.text = UpgradeManager.gunSlots[i].gunSlot.upgrades[j].upgrade.upgradeName;
                    }
                }
            }
            inMenu = true;
        }
    }
}
