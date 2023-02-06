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

    UpgradeManager upgradeManager;
    int bulletsPerShot = 1;
    int bulletsPerTap = 1;
    float bulletSize = 7;
    float bulletSpeed = 100;
    float bulletDamage = 34;
    float bulletKnockback = 2000;
    float attackSpeed = 1;
    float bulletSpread = 0;
    int bulletsShot;
    bool readyToShoot = true;
    bool allowInvoke = true;
    bool shooting = false;
    new Camera camera;

    void Awake()
    {
        camera = Camera.main;
        shootAnimator = GetComponent<Animator>();
        upgradeManager = GameObject.Find("GameManager").GetComponent<UpgradeManager>();

    }

    void OnEnable()
    {
        InputManager.playerInput.Player.Shoot.performed += (obj) => shooting = true;
        InputManager.playerInput.Player.Shoot.canceled += (obj) => shooting = false;
        InputManager.playerInput.Player.Shoot.Enable();
        
    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Shoot.Disable();
    }
    void Update()
    {
        if (shooting)
        {
            if (readyToShoot)
            {
                bulletsShot = 0;
                ShootGun();
            }
        }
        
    }

    void ShootGun()
    {
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
}