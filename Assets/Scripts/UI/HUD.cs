using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] Health health;
    [SerializeField] PlayerItems playerOther;


    [Header("Text Fields")]
    [SerializeField] TMP_Text fpsText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text bulletsText;

    void Start()
    {
        StartCoroutine(PrintFPS());
        StartCoroutine(PrintSpeed());
        PrintHealth();
        Gun.onShot += PrintBullets;
        Gun.onAfterReload += PrintBullets;
        playerOther.onGunSwitch += PrintBullets;
        UpgradeManager.onUpgrade += PrintBullets;
        UpgradeManager.onUpgrade += PrintHealth;
        PlayerHealth.onPlayerHurt += PrintHealth;
    }
    IEnumerator PrintFPS()
    {
        while (true)
        {
            fpsText.text = Mathf.Round(1f / Time.unscaledDeltaTime) + " FPS\n";
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator PrintSpeed()
    {
        Movement characterController = playerOther.GetComponent<Movement>();
        while (true)
        {
            speedText.text = Mathf.RoundToInt(characterController.Motor.BaseVelocity.SetY(0).magnitude) + " SPEEDS";
            yield return new WaitForSeconds(0.1f);
        }

    }
    public void PrintHealth()
    {
        healthText.text = "Health " + health.ToString();
    }
    public void PrintBullets()
    {
        StartCoroutine(PrintBulletsAfterFrame());
    }
    IEnumerator PrintBulletsAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        if (Inventory.HasGun())
        {
            bulletsText.text = Inventory.guns[0].gunSlot.bulletsLeft + "/" + Inventory.guns[0].gunSlot.bulletsPerMag;
        }
    }
}
