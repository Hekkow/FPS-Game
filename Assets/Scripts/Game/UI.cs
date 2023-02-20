using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour {
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject upgradeBox;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject tabMenu;
    bool tabMenuActive = false;
    GameObject openMenu;
    GameObject HUD;
    new FPSCamera camera;

    [HideInInspector] public static bool inMenu = false;

    void Start()
    {
        InputManager.playerInput.Player.Tab.performed += OpenTab;
        InputManager.playerInput.Player.Tab.Enable();

    }
    void OpenTab(InputAction.CallbackContext obj)
    {
        if (tabMenuActive)
        {
            for (int i = 0; i < tabMenu.transform.childCount; i++)
            {
                Destroy(tabMenu.transform.GetChild(i).gameObject);
            }
            tabMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            tabMenuActive = false;
        }
        else
        {
            tabMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameObject upgradeBoxTmp = Instantiate(Resources.Load<GameObject>("Prefabs/Button"), tabMenu.transform);
            float width = upgradeBoxTmp.GetComponent<RectTransform>().sizeDelta.x;
            float height = upgradeBoxTmp.GetComponent<RectTransform>().sizeDelta.y;
            float count = UpgradeManager.ownedUpgrades.Count;
            Destroy(upgradeBoxTmp);
            int[] slots = new int[] { 0, 0, 0 };
            for (int i = 0; i < count; i++)
            {
                Upgrade upgrade = UpgradeManager.ownedUpgrades[i].upgrade;
                int slot = UpgradeManager.ownedUpgrades[i].slot;
                if (slot != -1)
                {
                    GameObject upgradeBox = Instantiate(Resources.Load<GameObject>("Prefabs/Button"), tabMenu.transform);
                    upgradeBox.GetComponentInChildren<TMP_Text>().text = upgrade.upgradeName;
                    float x = slots[slot]%2 * width - Screen.width / 2 + width / 2 + slot * (Screen.width / 3) +5 ;
                    float y = slots[slot]/2 * -height + Screen.height/2 - height/2 + 5;
                    upgradeBox.transform.localPosition = new Vector3(x, y, 0);
                    upgradeBox.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UpgradeClicked(upgradeBox));
                    slots[slot]++;
                }
            }
            tabMenuActive = true;
        }
    }
    void UpgradeClicked(GameObject box)
    {
        Debug.Log(box.name);
    }
    //void Start()
    //{
    //    camera = player.GetComponentInChildren<PlayerCamera>();
    //    HUD = GameObject.Find("HUD");
    //}


    //void Update() 
    //{
    //    //if (input.upgradeMenu)
    //    //{
    //    //    OpenOrClose("upgrade");
    //    //}
    //    //else if (input.escape)
    //    //{
    //    //    OpenOrClose("pause");
    //    //}
    //}
    //public void OpenMenu(string menu, string category="")
    //{

    //    inMenu = true;
    //    //input.look = new Vector2(0, 0);

    //    if (menu == "upgrade") {
    //        openMenu = upgradeMenu;
    //        Transform panel = openMenu.transform;
    //        List<Upgrade> randomUpgrades = upgrades.GetRandomFromCategory(3);
    //        for (int i = 0; i < panel.childCount; i++) // get rid of any upgrade boxes that might be there already
    //        {
    //            GameObject.Destroy(panel.GetChild(i).gameObject);
    //        }
    //        for (int i = 0; i < 3; i++)
    //        {
    //            GameObject upgradeBox = Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeBox"), panel);
    //            upgradeBox.transform.Find("Name").GetComponent<TMP_Text>().text = randomUpgrades[i].name;
    //            upgradeBox.transform.Find("Description").GetComponent<TMP_Text>().text = randomUpgrades[i].description;
    //            Button button = upgradeBox.GetComponent<Button>();
    //            button.onClick.AddListener(delegate { upgrades.Invoke(randomUpgrades[i].functionName, 0); });
    //            button.onClick.AddListener(CloseMenu);
    //        }
    //    }
    //    else if (menu == "pause")
    //    {
    //        openMenu = pauseMenu;
    //    }
    //    HUD.GetComponent<Canvas>().enabled = false;
    //    openMenu.SetActive(true);
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //    Time.timeScale = 0;
    //    //input.look = new Vector2(0, 0);

    //}
    //void OpenOrClose(string name)
    //{
    //    if (!inMenu)
    //    {
    //        OpenMenu(name);
    //    }
    //    else
    //    {
    //        CloseMenu();
    //    }
    //}
    //public void CloseMenu()
    //{
    //    inMenu = false;
    //    HUD.GetComponent<Canvas>().enabled = true;
    //    openMenu.SetActive(false);
    //    Time.timeScale = 1;
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //}
}
