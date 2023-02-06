using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Xml;

public class Console : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    UpgradeManager upgradeManager;
    [SerializeField] GameObject player;
    bool showConsole = false;
    string command;
    void Awake()
    {
        upgradeManager = GameObject.Find("GameManager").GetComponent<UpgradeManager>();
    }
    void OnEnable()
    {
        InputManager.playerInput.Player.Console.performed += ToggleConsole;
        InputManager.playerInput.UI.Execute.performed += ExecuteCommand;
        InputManager.playerInput.Player.Console.Enable();
        InputManager.playerInput.UI.Execute.Enable();
    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Console.Disable();
        InputManager.playerInput.UI.Execute.Disable();
    }
    void ToggleConsole(InputAction.CallbackContext obj)
    {

        InputManager.SwitchActionMap(InputManager.playerInput.UI);
        showConsole = !showConsole;
        command = "";
    }
    void OnGUI()
    {
        if (!showConsole) return;
        float y = Screen.height - 100;
        GUI.Box(new Rect(5, y, Screen.width/3, 30), "");
        GUI.SetNextControlName("console");
        command = GUI.TextField(new Rect(10, y + 5, Screen.width / 3 - 20, 20), command);
        GUI.FocusControl("console");
    }
    void ExecuteCommand(InputAction.CallbackContext obj)
    {
        string[] words = command.Split(' ');
        if (words[0] == "upgrade")
        {
            if (words.Length == 2)
            {
                upgradeManager.ApplyUpgrade(upgradeManager.FindUpgrade(words[1]));
            }
            else
            {
                for (int i = 0; i < int.Parse(words[2]); i++)
                {
                    upgradeManager.ApplyUpgrade(upgradeManager.FindUpgrade(words[1]));
                }
            }


        }
        else if (words[0] == "print")
        {
            if (words[1] == "upgrades")
            {
                foreach (KeyValuePair<Upgrade, int> entry in UpgradeManager.ownedUpgrades)
                {
                    Debug.Log(entry.Key.name + " x " + entry.Value);
                }
            }
        }
        else if (words[0] == "spawn")
        {
            if (words[1] == "enemy")
            {
                if (words.Length == 2)
                {
                    //Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10);
                    Instantiate(Resources.Load<GameObject>("Prefabs/Enemy"), Camera.main.transform.TransformPoint(Vector3.forward * 10), Quaternion.identity);
                }
                //else
                //{
                //    //for (int i = 0; i < int.Parse(words[2]); i++)
                //    //{
                //    //    upgradeManager.ApplyUpgrade(upgradeManager.FindUpgrade(words[1]));
                //    //}
                //}
            }
            
        }
        else if (words[0] == "help")
        {
            Debug.Log("print upgrades");
            Debug.Log("upgrade upgradename [amount]");
        }

        else
        {
            if (command != "") Debug.Log("that command dont exist bruv");
        }
        InputManager.SwitchActionMap(InputManager.playerInput.Player);
        showConsole = !showConsole;
        command = "";
    }
    //void OpenConsole(InputAction.CallbackContext obj)
    //{
    //    inputField.gameObject.SetActive(true);
    //    inputField.Select();
    //}
    //public void ExecuteCommand(string command)
    //{
    //    Debug.Log(command);
    //    inputField.text = "";
    //    inputField.gameObject.SetActive(false);
    //}
}
