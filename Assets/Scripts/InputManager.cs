using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            leftMouse = Input.GetKey(KeyCode.Mouse0);
            leftMouseDown = Input.GetKeyDown(KeyCode.Mouse0);
            rightMouse = Input.GetKey(KeyCode.Mouse1);
            rightMouseDown = Input.GetKeyDown(KeyCode.Mouse1);
            interactLeft = Input.GetButtonDown("Interact Left");
            interactRight = Input.GetButtonDown("Interact Right");
            throwItem = Input.GetButton("Throw Item");
            punch = Input.GetButtonDown("Punch");
        }

    }

}
