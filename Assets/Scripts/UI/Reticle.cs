using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [Header("Reticles")]
    [SerializeField] GameObject unarmedReticle;
    [SerializeField] RectTransform armedReticle;
    //[SerializeField] RectTransform shotgunReticle;

    [Header("Curves")]
    [SerializeField] AnimationCurve shotCurve;
    [SerializeField] AnimationCurve reloadCurve;
    [SerializeField] float reticleBloomAmount = 20;

    [Header("Events")]
    [SerializeField] PlayerItems player;

    float reticleHoleSize;
    //float defaultReticleHoleSize = -50;
    float defaultReticleHoleSize = 32;
    float mainReticleSize;
    void Start()
    {
        player.onGunSwitch += RefreshReticle;
        UpgradeManager.onUpgrade += RefreshReticle;
        Gun.onShot += Shot;
        Gun.onBeforeReload += Reload;


    }
    private void Update()
    {
        //float test1 = test;
        //shotgunReticle.sizeDelta = new Vector2(test1, test1);
    }
    IEnumerator ShotBloom()
    {
        float startTime = Time.time;
        while (Inventory.HasGun() && Time.time - startTime < (1 / Inventory.guns[0].attackSpeed))
        {
            reticleHoleSize = mainReticleSize + (shotCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].attackSpeed) * reticleBloomAmount);
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
                reticleHoleSize = mainReticleSize + (reloadCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].reloadSpeed) * reticleBloomAmount);
                armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
                if (!Inventory.HasGun()) yield break;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public void RefreshReticle()
    {
        if (Inventory.HasGun())
        {
            mainReticleSize = Inventory.guns[0].pelletLayers * Inventory.guns[0].pelletSpread * 600 + Inventory.guns[0].bulletSpread * 60 + defaultReticleHoleSize;
            unarmedReticle.SetActive(false);
            armedReticle.gameObject.SetActive(true);
            reticleHoleSize = mainReticleSize;
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
}
