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
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject player;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject upgradeBox;
    [SerializeField] GameObject pauseMenu;
    InputManager input;
    Health health;
    Upgrades upgrades;
    TMP_Text fpsText;
    TMP_Text healthText;
    TMP_Text speedText;
    GameObject openMenu;
    float fps;
    float minfps = 10000;
    float maxfps = 0;
    float averagefps;
    List<float> fpsList;
    [HideInInspector] public static bool inMenu = false;

    void Start()
    {
        fpsList = new List<float>();
        input = gameManager.AddComponent<InputManager>();
        health = player.GetComponent<Health>();
        upgrades = player.GetComponent<Upgrades>();
        fpsText = gameObject.transform.Find("FPS").GetComponent<TMP_Text>();
        healthText = gameObject.transform.Find("Health").GetComponent<TMP_Text>();
        speedText = gameObject.transform.Find("Speed").GetComponent<TMP_Text>();
        InvokeRepeating("PrintFPS", 1, 0.1f);
        InvokeRepeating("PrintSpeed", 0, 0.1f);
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
        PrintHealth();
    }
    void PrintFPS()
    {
        fps = Mathf.Round(1 / Time.unscaledDeltaTime);
        if (fps < minfps) { minfps = fps; }
        if (fps > maxfps) { maxfps = fps; }
        fpsList.Add(fps);
        if (fpsList.Count > 100)
        {
            fpsList.RemoveAt(0);

        }
        averagefps = Mathf.Round(fpsList.Average());

        fpsText.text = fps + " FPS\n"+ maxfps +" FPS MAX\n" + minfps +" FPS MIN\n " + averagefps +" FPS AVERAGE";
        

    }
    void PrintSpeed()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        int speed = Mathf.RoundToInt(new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude);
        speedText.SetText(speed + " SPEEDS");
    }
    void PrintHealth()
    {
        
        healthText.text = "Health " + Helper.HealthToHashtags(health);
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
