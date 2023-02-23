using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour {
    [Header("Objects")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject upgradeBox;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject tabMenu;

    [HideInInspector] public static bool inMenu = false;

}
