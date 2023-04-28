using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class Platforms : MonoBehaviour
{
    [SerializeField] RenderPipelineAsset[] qualityLevels;
    [SerializeField] FPSCamera fpsCamera;
    [SerializeField] Player player;
    void Awake()
    {
        InputSystem.onActionChange += InputActionChangeCallback;
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            Application.targetFrameRate = 120;
            QualitySettings.SetQualityLevel(0);
            QualitySettings.renderPipeline = qualityLevels[0];
        }
    }
    void InputActionChangeCallback(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction receivedInputAction = (InputAction)obj;
            InputDevice lastDevice = receivedInputAction.activeControl.device;
            if (lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse"))
            {
                fpsCamera.sensitivity = player.sensitivity;
            }
            else if (lastDevice.name.Contains("Gamepad") || lastDevice.name.Contains("Controller"))
            {
                fpsCamera.sensitivity = player.controllerSensitivity;
            }
        }
    }
}
