using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInputAction playerInput = new PlayerInputAction();
    public static void SwitchActionMap(InputActionMap actionMap)
    {
        playerInput.Disable();
        actionMap.Enable();
    }
}
