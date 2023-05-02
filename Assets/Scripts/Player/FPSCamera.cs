using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Rendering;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Movement characterController;
    InputAction lookInput;

    float xRotation;
    float yRotation;

    Vector2 mouse;
    float mouseX;
    float mouseY;

    public float sensitivity;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sensitivity = player.sensitivity;
    }
    
    private void OnEnable()
    {
        lookInput = InputManager.playerInput.Player.Look;
        lookInput.Enable();
    }

    void LateUpdate()
    {
        mouse = lookInput.ReadValue<Vector2>();
        mouseX = mouse.x * sensitivity * Time.fixedDeltaTime;
        mouseY = mouse.y * sensitivity * Time.fixedDeltaTime;
        characterController.mouseInput = new Vector2(mouseX, mouseY); // maybe change to x/yrotation?
        yRotation += mouseX;
        characterController.yRotation = yRotation;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
