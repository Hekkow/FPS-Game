using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    [Header("Objects")]
    Animator shootAnimator;

    [SerializeField] Player player;

    PlayerInputAction playerInput;
    public Gun gun;

    Transform attackPoint;

    UpgradeManager upgradeManager;

    int bulletsShot;
    bool readyToShoot = true;
    bool allowInvoke = true;
    Inventory.Hand hand;
    bool throwing;
    new Camera camera;

    void Awake()
    {
        camera = Camera.main;
        shootAnimator = GetComponent<Animator>();
        playerInput = new PlayerInputAction();
        upgradeManager = GameObject.Find("GameManager").GetComponent<UpgradeManager>();
    }
    void OnEnable()
    {
        playerInput.Player.ShootLeft.performed += ShootLeft;
        playerInput.Player.ShootRight.performed += ShootRight;
        playerInput.Player.Throw.performed += (obj) => throwing = true;
        playerInput.Player.Throw.canceled += (obj) => throwing = false;
        playerInput.Player.ShootRight.Enable();
        playerInput.Player.ShootLeft.Enable();

        if (transform.parent.gameObject.name == "Right Hand Location")
        {
            attackPoint = transform.parent.parent.Find("Weapon Camera").Find("Right Gun Barrel");
            hand = Inventory.Hand.Right;
        }
        else if (transform.parent.gameObject.name == "Left Hand Location")
        {
            attackPoint = transform.parent.parent.Find("Weapon Camera").Find("Left Gun Barrel");
            hand = Inventory.Hand.Left;
        }
    }
    void OnDisable()
    {
        playerInput.Player.ShootRight.Disable();
        playerInput.Player.ShootLeft.Disable();
    }
    void ShootLeft(InputAction.CallbackContext obj)
    {
        upgradeManager.ApplyUpgrade(upgradeManager.allUpgrades[0]);
        if (readyToShoot)
        {
            bulletsShot = 0;
            if (!throwing && (hand == Inventory.Hand.Left || (Inventory.HasGun(Inventory.Hand.Right) && !Inventory.HasGun(Inventory.Hand.Left)))) {
                ShootGun();
            }
        }
    }
    void ShootRight(InputAction.CallbackContext obj)
    {
        if (readyToShoot)
        {
            bulletsShot = 0;
            if (!throwing && (hand == Inventory.Hand.Right || (Inventory.HasGun(Inventory.Hand.Left) && !Inventory.HasGun(Inventory.Hand.Right))))
            {
                ShootGun();
            }
        }
    }
    //void Update()
    //{
    //    if (readyToShoot)
    //    {
    //        bulletsShot = 0;
    //        if (input.leftMouse && !input.throwItem && (hand == Inventory.Hand.Left || (Inventory.HasGun(Inventory.Hand.Right) && !Inventory.HasGun(Inventory.Hand.Left))))
    //        {
    //            ShootGun();
    //        }
    //        if (input.rightMouse && !input.throwItem && (hand == Inventory.Hand.Right || (Inventory.HasGun(Inventory.Hand.Left) && !Inventory.HasGun(Inventory.Hand.Right))))
    //        {
    //            ShootGun();
    //        }
    //    }
    //}
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


        for (int i = 0; i < gun.bulletsPerShot * player.bulletsPerShotMultiplier; i++)
        {

            Vector3 direction = targetPoint - attackPoint.position + RandomSpread();

            // creates bullet and sends it zoomin

            GameObject currentBullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = direction.normalized;
            currentBullet.transform.localScale *= gun.bulletSize * player.bulletSizeMultiplier;
            currentBullet.GetComponent<Rigidbody>().velocity = direction.normalized * gun.bulletSpeed * player.bulletSpeedMultiplier;

            // creates bullet with specified stats
            Helper.AddDamage(currentBullet, gun.bulletDamage * player.bulletDamageMultiplier, gun.bulletKnockback * player.bulletKnockbackMultiplier, false, false);


        }
        bulletsShot++;


        // attack speed

        if (allowInvoke)
        {
            Invoke("ResetShot", 1/(gun.attackSpeed * player.attackSpeedMultiplier));
            allowInvoke = false;
        }

        // time between all bullets from same tap

        if (bulletsShot < gun.bulletsPerTap * player.bulletsPerTapMultiplier)
        {
            Invoke("ShootGun", gun.attackSpeed * player.attackSpeedMultiplier);
        }

        shootAnimator.Play("shoot", 0, 0f);

        shootAnimator.speed = gun.attackSpeed * player.attackSpeedMultiplier;
    }


    Vector3 RandomSpread()
    {
        float x = Random.Range(-gun.bulletSpread * player.bulletSpreadMultiplier, gun.bulletSpread * player.bulletSpreadMultiplier);
        float y = Random.Range(-gun.bulletSpread * player.bulletSpreadMultiplier, gun.bulletSpread * player.bulletSpreadMultiplier);
        float z = 0;
        //float z = Random.Range(-gun.bulletSpread, gun.bulletSpread);
        return new Vector3(x, y, z);
    }

    void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
}