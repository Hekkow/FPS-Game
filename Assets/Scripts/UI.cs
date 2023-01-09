using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    PlayerCamera camera;

    [HideInInspector] public static bool inMenu = false;

    void Start()
    {
        camera = player.GetComponentInChildren<PlayerCamera>();
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
    public void OpenMenu(string menu, string category="")
    {

        inMenu = true;
        input.look = new Vector2(0, 0);

        if (menu == "upgrade") {
            openMenu = upgradeMenu;
            Transform panel = openMenu.transform;
            List<Upgrade> upgradeCategory;
            if (category == "defense") upgradeCategory = upgrades.defenseUpgrades;
            else upgradeCategory = upgrades.allUpgrades;
            for (int i = 0; i < panel.childCount; i++) // get rid of any upgrade boxes that might be there already
            {
                GameObject.Destroy(panel.GetChild(i).gameObject);
            }
            for (int i = 0; i < 3; i++)
            {
                Upgrade upgrade = upgradeCategory[Random.Range(0, upgradeCategory.Count)];
                GameObject upgradeBox = Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeBox"), panel);
                upgradeBox.transform.Find("Name").GetComponent<TMP_Text>().text = upgrade.name;
                upgradeBox.transform.Find("Description").GetComponent<TMP_Text>().text = upgrade.description;
                Button button = upgradeBox.GetComponent<Button>();
                button.onClick.AddListener(delegate{upgrades.Invoke(upgrade.name.Replace(" ", ""), 0);});
                button.onClick.AddListener(CloseMenu);
            }
        }
        else if (menu == "pause")
        {
            openMenu = pauseMenu;
        }
        HUD.GetComponent<Canvas>().enabled = false;
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
            OpenMenu(name);
        }
        else
        {
            CloseMenu();
        }
    }
    public void CloseMenu()
    {
        inMenu = false;
        HUD.GetComponent<Canvas>().enabled = true;
        openMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
