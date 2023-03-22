using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Console : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject player;
    [HideInInspector] public List<string> commandHistory;

    int current;
    bool showConsole = false;
    string command;

    void Start()
    {
        if (commandHistory == null)
        {
            commandHistory = new List<string>();
        }
        current = commandHistory.Count;
    }
    void OnEnable()
    {
        InputManager.playerInput.Player.Console.performed += ToggleConsole;
        InputManager.playerInput.Console.Execute.performed += ExecuteCommand;
        InputManager.playerInput.Console.Previous.performed += PreviousCommand;
        InputManager.playerInput.Console.Next.performed += NextCommand;

        InputManager.playerInput.Player.Console.Enable();
        InputManager.playerInput.Console.Execute.Enable();
        InputManager.playerInput.Console.Previous.Enable();
        InputManager.playerInput.Console.Next.Enable();

    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Console.Disable();
        InputManager.playerInput.Console.Execute.Disable();
        InputManager.playerInput.Console.Previous.Disable();
        InputManager.playerInput.Console.Next.Disable();
    }
    void ToggleConsole(InputAction.CallbackContext obj)
    {
        InputManager.SwitchActionMap(InputManager.playerInput.Console);
        current = commandHistory.Count;
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
            if (words[1] == "random")
            {
                for (int i = 0; i < int.Parse(words[2]); i++)
                {
                    List<Upgrade> upgrade = UpgradeManager.RandomUpgrades(1);
                    UpgradeManager.ActivateUpgrade(upgrade[0]);
                }
            }
            else
            {
                Upgrade upgrade = UpgradeManager.FindUpgrade(words[1]);
                if (upgrade != null)
                {
                    int amount = 1;
                    if (words.Length > 2) amount = int.Parse(words[2]);
                    for (int i = 0; i < amount; i++)
                    {
                        UpgradeManager.ActivateUpgrade(upgrade);
                    }
                }
                else
                {
                    Debug.Log("Upgrade not found");
                }
            }
        }
        else if (words[0] == "print")
        {
            if (words[1] == "upgrades")
            {
                UpgradeManager.PrintOwnedUpgrades();
                if (words.Length == 3 && words[2] == "amount") 
                {
                    Debug.Log(UpgradeManager.ownedUpgrades.Count); 
                }
            }
            else if (words[1] == "all")
            {
                if (words[2] == "upgrades")
                {
                    UpgradeManager.PrintAllUpgrades();
                }
            }
            else if (words[1] == "random")
            {
                if (words[2] == "upgrades")
                {
                    List<Upgrade> upgrades = UpgradeManager.RandomUpgrades(int.Parse(words[3]));
                    for (int i = 0; i < int.Parse(words[3]); i++)
                    {
                        Debug.Log(upgrades[i]);
                    }
                }
            }
            else if (words[1] == "slot")
            {
                Debug.Log(Inventory.guns[0].slot);
            }

        }
        else if (words[0] == "spawn")
        {
            if (words.Length == 2)
            {
                StartCoroutine(Spawn(words[1][0].ToString().ToUpper() + words[1].Substring(1), 1, 10, 0.3f));
            }
            else if (words.Length == 3)
            {
                StartCoroutine(Spawn(words[1][0].ToString().ToUpper() + words[1].Substring(1), int.Parse(words[2]), 10, 0.3f));
            }
            
        }
        else if (words[0] == "help")
        {
            Debug.Log("print upgrades");
            Debug.Log("upgrade upgradename [amount]");
        }
        else if (words[0] == "history")
        {
            for (int i = 0; i < commandHistory.Count; i++)
            {
                Debug.Log(commandHistory[i]);
            }
        }
        else if (words[0] == "hurt")
        {
            GameObject.Find("Player").GetComponent<PlayerHealth>().Damaged(int.Parse(words[1]), null, null);
        }
        else
        {
            if (command != "") Debug.Log("that command dont exist bruv");
        }

        if (command != "" && command != commandHistory[commandHistory.Count-1]) commandHistory.Add(command);

        InputManager.SwitchActionMap(InputManager.playerInput.Player);

        showConsole = !showConsole;
        command = "";
    }
    void PreviousCommand(InputAction.CallbackContext obj)
    {
        if (current > 0)
        {
            current--;

            command = commandHistory[current];
        }
    }
    void NextCommand(InputAction.CallbackContext obj)
    {
        if (current == commandHistory.Count-1) 
        {
            current = commandHistory.Count;
            command = "";
        }
        else if (current < commandHistory.Count)
        {
            current++;
            command = commandHistory[current];
        }
    }
    IEnumerator Spawn(string prefabPath, int amount, float distance, float timeBetween)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/" + prefabPath), Camera.main.transform.TransformPoint(Vector3.forward * distance), Quaternion.identity);
            yield return new WaitForSeconds(timeBetween);
        }
    }
}
