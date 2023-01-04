using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows;
using System.Linq;

public class UI : MonoBehaviour {
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject upgradeBox;
    [SerializeField] GameObject pauseMenu;
    InputManager input;
    Upgrades upgrades;
    GameObject openMenu;
    GameObject HUD;
    
    [HideInInspector] public static bool inMenu = false;

    void Start()
    {
        input = gameManager.AddComponent<InputManager>();
        upgrades = player.GetComponent<Upgrades>();
        HUD = GameObject.Find("HUD");
    }
    

    void Update() 
    {
        if (input.upgradeMenu)
        {
            OpenOrClose("upgrade");
        }
        else if (input.escape)
        {
            OpenOrClose("pause");
        }
        
    }
    
    public void OpenMenu(string menu)
    {
        if (menu == "upgrade") {
            openMenu = upgradeMenu;
            Transform panel = openMenu.transform;
            for (int i = 0; i < panel.childCount; i++) // get rid of any upgrade boxes that might be there already
            {
                GameObject.Destroy(panel.GetChild(i).gameObject);
            }
            for (int i = 0; i < upgrades.maxUpgrades; i++)
            {
                GameObject upgrade = Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeBox"), panel);
                upgrade.transform.Find("Name").GetComponent<TMP_Text>().text = upgrades.allUpgrades[i].name;
                upgrade.transform.Find("Description").GetComponent<TMP_Text>().text = upgrades.allUpgrades[i].description;
                Button button = upgrade.GetComponent<Button>();
                int ivalue = i; // delegate takes by reference so we create new variable to keep value consistent
                button.onClick.AddListener(delegate{upgrades.Invoke(upgrades.allUpgrades[ivalue].name.Replace(" ", ""), 0);});
                button.onClick.AddListener(CloseMenu);
            }
        }
        else if (menu == "pause")
        {
            openMenu = pauseMenu;
        }
        inMenu = true;
        Helper.MakeChildrenVisible(HUD, false);
        openMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        input.look = new Vector2(0, 0);

    }
    void OpenOrClose(string name)
    {
        if (!inMenu)
        {
            inMenu = true;
            input.look = new Vector2(0, 0);
            OpenMenu(name);
        }
        else
        {

            inMenu = false;
            CloseMenu();
        }
    }
    public void CloseMenu()
    {
        Helper.MakeChildrenVisible(HUD, true);
        inMenu = false;
        openMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
