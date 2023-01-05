using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Transform playerBody;

    [SerializeField] InputManager input;

    float xRotation;
    float yRotation;

    Vector2 mouse;
    public bool cameraEnabled;
    float mouseX;
    float mouseY;

    void Awake()
    {
        cameraEnabled = true;
        // locks mouse to game

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (cameraEnabled)
        {
            mouse = input.look;

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
    }
    void FixedUpdate()
    {
        // aligns body to camera
        playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
