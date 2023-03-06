using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [Header("Reticles")]
    [SerializeField] GameObject unarmedReticle;
    [SerializeField] RectTransform armedReticle;

    [Header("Curves")]
    [SerializeField] AnimationCurve shotCurve;
    [SerializeField] AnimationCurve reloadCurve;
    [SerializeField] float reticleBloomAmount = 20;

    [Header("Events")]
    [SerializeField] PlayerOther playerOther;

    float reticleHoleSize;
    float defaultReticleHoleSize = 32;
    void Start()
    {
        playerOther.onGunSwitch += RefreshReticle;
        UpgradeManager.onUpgrade += RefreshReticle;
        Gun.onShot += Shot;
        Gun.onBeforeReload += Reload;


    }
    IEnumerator ShotBloom()
    {
        float startTime = Time.time;
        while (Inventory.HasGun() && Time.time - startTime < (1 / Inventory.guns[0].attackSpeed))
        {
            reticleHoleSize = Inventory.guns[0].bulletSize * 10 + defaultReticleHoleSize + (shotCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].attackSpeed) * reticleBloomAmount);
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
                reticleHoleSize = Inventory.guns[0].bulletSize * 10 + defaultReticleHoleSize + (reloadCurve.Evaluate((Time.time - startTime) * Inventory.guns[0].reloadSpeed) * reticleBloomAmount);
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
            reticleHoleSize = Inventory.guns[0].bulletSize * 10 + defaultReticleHoleSize;
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
