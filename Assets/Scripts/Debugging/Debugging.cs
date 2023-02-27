using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Debugging : MonoBehaviour
{
    [SerializeField] Animator enemyAnimator;
    void OnEnable()
    {
        //InputManager.playerInput.Player.RightClick.performed += EnemyPunch;
        //InputManager.playerInput.Player.RightClick.Enable();
    }

}
