using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject player;

    [Header("Reticles")]
    [SerializeField] GameObject unarmedReticle;
    [SerializeField] RectTransform armedReticle;

    [Header("Curves")]
    [SerializeField] AnimationCurve shotCurve;
    [SerializeField] AnimationCurve reloadCurve;
    
    [Header("Text Fields")]
    [SerializeField] TMP_Text fpsText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text bulletsText;

    float reticleHoleSize;
    float reticleBloomAmount = 20;
    float defaultReticleHoleSize = 32;
    float sizePerBulletSpread = 10;

    void Start()
    {
        StartCoroutine(PrintFPS());
        StartCoroutine(PrintSpeed());
        PrintHealth();
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
    IEnumerator ShotBloom()
    {
        float startTime = Time.time;
        while (Inventory.HasGun() && Time.time - startTime < (1 / Inventory.guns[0].attackSpeed))
        {
            reticleHoleSize = Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * sizePerBulletSpread + defaultReticleHoleSize + (shotCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].attackSpeed) * reticleBloomAmount);
            armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
            yield return new WaitForEndOfFrame();
        } 
    }
    IEnumerator ReloadBloom()
    {
        if (Inventory.HasGun())
        {
            float startTime = Time.time;
            while (Time.time - startTime < (1 / Inventory.guns[0].reloadSpeed))
            {
                reticleHoleSize = Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * sizePerBulletSpread + defaultReticleHoleSize + (reloadCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].reloadSpeed) * reticleBloomAmount);
                armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public void RefreshReticle()
    {
        if (Inventory.HasGun())
        {
            unarmedReticle.SetActive(false);
            armedReticle.gameObject.SetActive(true);
            reticleHoleSize = Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * sizePerBulletSpread + defaultReticleHoleSize;
            armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
        }
        else
        {
            unarmedReticle.SetActive(true);
            armedReticle.gameObject.SetActive(false);
        }
    }
    public void Shot()
    {
        StartCoroutine(ShotBloom());
    }
    public void Reload()
    {
        StartCoroutine(ReloadBloom());
    }
    public void PrintHealth()
    {
        healthText.text = "Health " + Helper.HealthToHashtags(GameObject.Find("Player").GetComponent<Health>()); 
    }
    public void PrintBullets()
    {
        if (Inventory.HasGun())
        {
            bulletsText.text = Inventory.guns[0].bulletsLeft + "/" + Inventory.guns[0].bulletsPerMag;
        }
    }
}
