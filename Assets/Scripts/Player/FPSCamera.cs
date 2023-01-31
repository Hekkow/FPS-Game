using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Transform playerBody;

    PlayerInputAction playerInput;
    InputAction lookInput;

    float xRotation;
    float yRotation;

    Vector2 mouse;
    float mouseX;
    float mouseY;

    void Awake()
    {
        // locks mouse to game

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput = new PlayerInputAction();
    }

    private void OnEnable()
    {
        lookInput = playerInput.Player.Look;
        playerInput.Enable();
    }

    void Update()
    {
            mouse = lookInput.ReadValue<Vector2>();

            mouseX = mouse.x * player.sensitivity * Time.fixedDeltaTime;
            mouseY = mouse.y * player.sensitivity * Time.fixedDeltaTime;

            // no idea how this works, i think it's cuz the xRotation is inversed by default
            yRotation += mouseX;
            xRotation -= mouseY;

            // doesn't allow player to look up and backwards
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // camera alignment
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
    void FixedUpdate()
    {
        // aligns body to camera
        playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
