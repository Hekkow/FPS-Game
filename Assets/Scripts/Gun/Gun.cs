using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Objects")]
    Animator shootAnimator;

    [SerializeField] Player player;

    [SerializeField] Transform attackPoint;
    int bulletsPerShot = 1;
    int bulletsPerTap = 1;
    float bulletSize = 7;
    float bulletSpeed = 100;
    float bulletDamage = 34;
    float bulletKnockback = 2000;
    float attackSpeed = 1;
    float bulletSpread = 0;
    float reloadTime = 3f;
    int bulletsShot;
    public int bulletsPerMag = 3;
    public int bulletsLeft = 3;
    bool readyToShoot = true;
    bool allowInvoke = true;
    bool shooting = false;
    bool gunShot = false;
    bool reloading = false;
    new Camera camera;
    Coroutine reloadCoroutine;

    void Awake()
    {
        camera = Camera.main;
        shootAnimator = GetComponent<Animator>();
        reloadCoroutine = StartCoroutine(WaitThenReload());
        StopCoroutine(reloadCoroutine);
    }

    void OnEnable()
    {
        InputManager.playerInput.Player.Shoot.performed += (obj) => shooting = true;
        InputManager.playerInput.Player.Shoot.canceled += (obj) => shooting = false;
        InputManager.playerInput.Player.Reload.performed += Reload;
        InputManager.playerInput.Player.Reload.Enable();
        InputManager.playerInput.Player.Shoot.Enable();
        
    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Reload.Disable();
        InputManager.playerInput.Player.Shoot.Disable();
    }
    void Update()
    {
        if (shooting)
        {
            if (readyToShoot && bulletsLeft > 0)
            {
                bulletsLeft--;
                bulletsShot = 0;
                ShootGun();

                if (bulletsLeft <= 0)
                {
                    reloadCoroutine = StartCoroutine(WaitThenReload());
                }
            }
        }
        
    }

    void ShootGun()
    {
        StopCoroutine(reloadCoroutine);
        gunShot = true;
        readyToShoot = false;

        // calculates direction

        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }


        for (int i = 0; i < bulletsPerShot + UpgradeManager.bulletsPerShotAddition; i++)
        {

            Vector3 direction = targetPoint - attackPoint.position; // + RandomSpread();

            // creates bullet and sends it zoomin

            GameObject currentBullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = direction.normalized;
            Vector3 randomSpread = RandomSpread();
            currentBullet.transform.Rotate(randomSpread);
            currentBullet.transform.localScale *= bulletSize * UpgradeManager.bulletSizeMultiplier;
            currentBullet.GetComponent<Rigidbody>().velocity = direction.normalized * bulletSpeed * UpgradeManager.bulletSpeedMultiplier;

            // creates bullet with specified stats
            Helper.AddDamage(currentBullet, bulletDamage * UpgradeManager.bulletDamageMultiplier, bulletKnockback, false, false);


        }
        bulletsShot++;


        // attack speed

        if (allowInvoke)
        {
            Invoke("ResetShot", 1/(attackSpeed*UpgradeManager.attackSpeedMultiplier));
            allowInvoke = false;
        }

        // time between all bullets from same tap

        if (bulletsShot < bulletsPerTap + UpgradeManager.bulletsPerTapAddition)
        {
            Invoke("ShootGun", 1 / (attackSpeed * UpgradeManager.attackSpeedMultiplier * (UpgradeManager.bulletsPerTapAddition+1)));
        }

        shootAnimator.Play("shoot", 0, 0f);

        shootAnimator.speed = attackSpeed * UpgradeManager.attackSpeedMultiplier;
        gunShot = false;
    }


    Vector3 RandomSpread()
    {

        float x = Random.Range(-bulletSpread - UpgradeManager.bulletSpreadAddition, bulletSpread + UpgradeManager.bulletSpreadAddition);
        float y = Random.Range(-bulletSpread - UpgradeManager.bulletSpreadAddition, bulletSpread + UpgradeManager.bulletSpreadAddition);
        float z = 0;
        return new Vector3(x, y, z);
    }

    void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
    void Reload(InputAction.CallbackContext obj)
    {
        if (!reloading) reloadCoroutine = StartCoroutine(WaitThenReload());
    }
    IEnumerator WaitThenReload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime / UpgradeManager.reloadTimeMultiplier);
        reloading = false;
        bulletsLeft = bulletsPerMag;

    }
}