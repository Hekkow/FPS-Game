using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Transform playerBody;
    [SerializeField] MyCharacterController characterController;
    InputAction lookInput;

    float xRotation;
    float yRotation;

    Vector2 mouse;
    float mouseX;
    float mouseY;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnEnable()
    {
        lookInput = InputManager.playerInput.Player.Look;
        lookInput.Enable();

    }
    void LateUpdate()
    {
        mouse = lookInput.ReadValue<Vector2>();
        mouseX = mouse.x * player.sensitivity * Time.fixedDeltaTime;
        mouseY = mouse.y * player.sensitivity * Time.fixedDeltaTime;
        characterController.mouseInput = new Vector2(mouseX, mouseY); // maybe change to x/yrotation?
        yRotation += mouseX;
        characterController.yRotation = yRotation;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
    void FixedUpdate()
    {
        playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
