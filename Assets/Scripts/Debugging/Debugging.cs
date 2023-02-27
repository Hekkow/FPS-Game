using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Debugging : MonoBehaviour
{
    [SerializeField] Animator enemyAnimator;
    void OnEnable()
    {
        //InputManager.playerInput.Player.RightClick.performed += EnemyPunch;
        //InputManager.playerInput.Player.RightClick.Enable();
    }
    void Awake()
    {
        //Damage[] buttonObjs = FindObjectsOfType<Damage>();

        //for (int i = 0; i < buttonObjs.Length; i++)
        //{
        //    Debug.Log(buttonObjs[i].name);
        //}
    }

}
