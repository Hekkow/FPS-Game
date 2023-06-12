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
    IEnumerator ShotBloom()
    {
        float startTime = Time.time;
        while (Inventory.HasGun() && Time.time - startTime < (1 / Inventory.guns[0].gunSlot.attackSpeed))
        {
            reticleHoleSize = mainReticleSize + (shotCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].gunSlot.attackSpeed) * reticleBloomAmount);
            armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator ReloadBloom()
    {
        float startTime = Time.time;
        while (Inventory.HasGun() && Time.time - startTime < (1 / Inventory.guns[0].gunSlot.reloadSpeed))
        {
            reticleHoleSize = mainReticleSize + (reloadCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].gunSlot.reloadSpeed) * reticleBloomAmount);
            armedReticle.sizeDelta = new Vector2(reticleHoleSize, reticleHoleSize);
            yield return new WaitForEndOfFrame(); 
        }
    }
    public void RefreshReticle()
    {
        if (Inventory.HasGun())
        {
            mainReticleSize = Inventory.guns[0].gunSlot.pelletLayers * Inventory.guns[0].gunSlot.pelletSpread * 600 + Inventory.guns[0].gunSlot.bulletSpread * 60 + defaultReticleHoleSize;
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
