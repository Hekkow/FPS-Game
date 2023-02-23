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

    Health health;

    float fps;
    float minfps = 10000;
    float maxfps = 0;
    float averagefps;
    List<float> fpsList;

    float reticleHoleSize;
    float reticleBloomAmount = 20;
    float defaultReticleHoleSize = 32;
    float sizePerBulletSpread = 10;

    void Start()
    {
        fpsList = new List<float>();
        health = player.GetComponent<Health>();
        InvokeRepeating("PrintFPS", 1, 0.1f);
        InvokeRepeating("PrintSpeed", 0, 0.1f);
        PrintHealth();
    }
    IEnumerator ShotBloom()
    {
        float startTime = Time.time;
        while (Time.time - startTime < (1 / Inventory.guns[0].attackSpeed))
        {
            reticleHoleSize = Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * sizePerBulletSpread + defaultReticleHoleSize + (shotCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].attackSpeed) * reticleBloomAmount);
            armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator ReloadBloom()
    {
        float startTime = Time.time;
        while (Time.time - startTime < (1 / Inventory.guns[0].reloadSpeed))
        {
            reticleHoleSize = Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * sizePerBulletSpread + defaultReticleHoleSize + (reloadCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].reloadSpeed) * reticleBloomAmount);
            armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
            yield return new WaitForEndOfFrame();
        }
    }
    public void RefreshReticle()
    {
        if (Inventory.HasGun() > 0)
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
    void PrintFPS()
    {
        fps = Mathf.Round(1 / Time.deltaTime);
        if (fps < minfps) { minfps = fps; }
        if (fps > maxfps) { maxfps = fps; }
        fpsList.Add(fps);
        if (fpsList.Count > 100)
        {
            fpsList.RemoveAt(0);

        }
        averagefps = Mathf.Round(fpsList.Average());

        fpsText.text = fps + " FPS\n" + maxfps + " FPS MAX\n" + minfps + " FPS MIN\n " + averagefps + " FPS AVERAGE";


    }
    void PrintSpeed()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        int speed = Mathf.RoundToInt(new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude);
        speedText.SetText(speed + " SPEEDS");
    }
    public void PrintHealth()
    {
        healthText.text = "Health " + Helper.HealthToHashtags(health);
    }
    public void PrintBullets()
    {
        if (Inventory.HasGun() > 0)
        {
            bulletsText.text = Inventory.guns[0].bulletsLeft + "/" + Inventory.guns[0].bulletsPerMag;
        }
    }
}
