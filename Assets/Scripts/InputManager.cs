using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    [HideInInspector] public Vector2 movement;
    [HideInInspector] public Vector2 look;
    [HideInInspector] public bool jump;
    [HideInInspector] public bool jumpDown;
    [HideInInspector] public bool leftMouse;
    [HideInInspector] public bool leftMouseDown;
    [HideInInspector] public bool rightMouse;
    [HideInInspector] public bool rightMouseDown;
    [HideInInspector] public bool upgradeMenu;
    [HideInInspector] public bool interactLeft;
    [HideInInspector] public bool interactRight;
    [HideInInspector] public bool switchHands;
    [HideInInspector] public bool throwItem;
    [HideInInspector] public bool escape;
    [HideInInspector] public bool punch;

    void Update()
    {
        upgradeMenu = Input.GetButtonDown("Upgrade");
        escape = Input.GetButtonDown("Pause");
        if (!UI.inMenu)
        {
            //movement = m_con
            //movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            leftMouse = Input.GetKey(KeyCode.Mouse0);
            leftMouseDown = Input.GetKeyDown(KeyCode.Mouse0);
            rightMouse = Input.GetKey(KeyCode.Mouse1);
            rightMouseDown = Input.GetKeyDown(KeyCode.Mouse1);
            interactLeft = Input.GetButtonDown("Interact Left");
            interactRight = Input.GetButtonDown("Interact Right");
            switchHands = Input.GetButtonDown("Switch Hands");
            throwItem = Input.GetButton("Throw Item");
            punch = Input.GetButtonDown("Punch");
        }

    }
    void OnLook(InputValue value)
    {
        if (!UI.inMenu)
        {
            look = value.Get<Vector2>();
        }
    }
    void OnJump(InputValue value)
    {
        if (!UI.inMenu)
        {
            jump = value.Get<bool>();
        }
    }
}
