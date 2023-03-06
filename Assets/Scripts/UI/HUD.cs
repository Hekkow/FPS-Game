using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject player;
    [SerializeField] PlayerOther playerOther;


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
        Rigidbody rb = player.GetComponent<Rigidbody>();
        while (true)
        {
            speedText.text = Mathf.RoundToInt(new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude) + " SPEEDS";
            yield return new WaitForSeconds(0.1f);
        }

    }
    public void PrintHealth()
    {
        healthText.text = "Health " + Helper.HealthToHashtags(GameObject.Find("Player").GetComponent<Health>()); 
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
            bulletsText.text = Inventory.guns[0].bulletsLeft + "/" + Inventory.guns[0].bulletsPerMag;
        }
    }
}
