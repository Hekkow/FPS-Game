using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject unarmedReticle;
    [SerializeField] RectTransform armedReticle;
    [SerializeField] AnimationCurve curve;

    Health health;
    TMP_Text fpsText;
    TMP_Text healthText;
    TMP_Text speedText;
    TMP_Text bulletsText;
    float fps;
    float minfps = 10000;
    float maxfps = 0;
    float averagefps;
    List<float> fpsList;
    float reticleHoleSize;

    void Start()
    {
        fpsList = new List<float>();
        health = player.GetComponent<Health>();
        fpsText = gameObject.transform.Find("FPS").GetComponent<TMP_Text>();
        healthText = gameObject.transform.Find("Health").GetComponent<TMP_Text>();
        speedText = gameObject.transform.Find("Speed").GetComponent<TMP_Text>();
        bulletsText = gameObject.transform.Find("Bullets").GetComponent<TMP_Text>(); 
        InvokeRepeating("PrintFPS", 1, 0.1f);
        InvokeRepeating("PrintSpeed", 0, 0.1f);
    }
    void Update()
    {
        PrintHealth();
    }
    IEnumerator ReticleBloom()
    {
        float startTime = Time.time;
        while (Time.time - startTime < (1 / Inventory.guns[0].attackSpeed))
        {
            yield return new WaitForEndOfFrame();
        }
        //while (Time.time - startTime < (1 / Inventory.guns[0].attackSpeed) * (1f / 8f))
        //{
        //    reticleHoleSize *= 1.02f;
        //    armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
        //    yield return new WaitForEndOfFrame();
        //}
        //startTime = Time.time;
        //while (reticleHoleSize > Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * 10 + 32 && Time.time - startTime < (1 / Inventory.guns[0].attackSpeed) * (7f / 8f))
        //{
        //    reticleHoleSize /= 1.004f;
        //    armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
        //    yield return new WaitForEndOfFrame();
        //}
        //reticleHoleSize = Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * 10 + 32;
        //armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
    }
    public void RefreshReticle()
    {
        if (Inventory.HasGun() > 0)
        {
            unarmedReticle.SetActive(false);
            armedReticle.gameObject.SetActive(true);
            reticleHoleSize = Inventory.guns[0].bulletSize + Inventory.guns[0].bulletSpread * 8 + 32;
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
        StartCoroutine(ReticleBloom());
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
    void PrintHealth()
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
