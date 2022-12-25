using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UI : MonoBehaviour {
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject player;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject upgradeBox;
    [SerializeField] GameObject pauseMenu;
    PlayerControls input;
    Health health;
    Upgrades upgrades;
    TMP_Text fpsText;
    TMP_Text healthText;
    TMP_Text speedText;
    GameObject openMenu;
    [HideInInspector] public static bool inMenu = false;

    void Start()
    {
        input = gameManager.AddComponent<PlayerControls>();
        health = player.GetComponent<Health>();
        upgrades = player.GetComponent<Upgrades>();
        fpsText = gameObject.transform.Find("FPS").GetComponent<TMP_Text>();
        healthText = gameObject.transform.Find("Health").GetComponent<TMP_Text>();
        speedText = gameObject.transform.Find("Speed").GetComponent<TMP_Text>();
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
        PrintFPS();
        PrintHealth();
        PrintSpeed();
    }
    void PrintFPS()
    {
        fpsText.text = (Mathf.Round(1 / Time.unscaledDeltaTime)).ToString() + " FPS";
    }
    void PrintSpeed()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        float speed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        speedText.SetText("Speed: " + speed);
    }
    void PrintHealth()
    {
        
        healthText.text = "Health " + Helper.HealthToHashtags(health);
    }
    public void OpenMenu(string menu)
    {
        if (menu == "upgrade") {
            openMenu = upgradeMenu;
            Transform panel = openMenu.transform.GetChild(0);
            for (int i = 0; i < panel.childCount; i++) // get rid of any upgrade boxes that might be there already
            {
                GameObject.Destroy(panel.GetChild(i).gameObject);
            }
            for (int i = 0; i < upgrades.maxUpgrades; i++)
            {
                GameObject upgrade = Instantiate(upgradeBox, panel);
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
        openMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        
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
        inMenu = false;
        openMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
