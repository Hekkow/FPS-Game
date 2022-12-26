using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [HideInInspector] public Vector2 movement;
    [HideInInspector] public Vector2 look;
    [HideInInspector] public bool jump;
    [HideInInspector] public bool jumpDown;
    [HideInInspector] public bool mouse;
    [HideInInspector] public bool mouseDown;
    [HideInInspector] public bool upgradeMenu;
    [HideInInspector] public bool interact;
    [HideInInspector] public bool throwItem;
    [HideInInspector] public bool escape;
    [HideInInspector] public bool punch;

    void Update()
    {
        upgradeMenu = Input.GetButtonDown("Upgrade");
        escape = Input.GetButtonDown("Pause");
        if (!UI.inMenu)
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            look = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            jump = Input.GetButton("Jump");
            jumpDown = Input.GetButtonDown("Jump");
            mouse = Input.GetKey(KeyCode.Mouse0);
            mouseDown = Input.GetKeyDown(KeyCode.Mouse0);
            interact = Input.GetButtonDown("Interact");
            throwItem = Input.GetButtonDown("Throw Item");
            punch = Input.GetButtonDown("Punch");
        }

    }

}
