using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Upgrades upgrades;
    [SerializeField] InputManager input;
    Animator shootAnimator;

    [SerializeField] Player player;
    Transform attackPoint;
    GameObject currentBullet;
    int bulletsShot;
    bool readyToShoot = true;
    bool allowInvoke = true;
    Inventory.Hand hand;

    new Camera camera;

    void Awake()
    {
        camera = Camera.main;
        shootAnimator = GetComponent<Animator>();
    }
    void Start()
    {
        RunAtStart();
    }
    void Update()
    {
        if (readyToShoot)
        {
            bulletsShot = 0;
            if (input.leftMouse && !input.throwItem && (hand == Inventory.Hand.Left || (Inventory.HasGun(Inventory.Hand.Right) && !Inventory.HasGun(Inventory.Hand.Left))))
            {
                Shoot();
            }
            if (input.rightMouse && !input.throwItem && (hand == Inventory.Hand.Right || (Inventory.HasGun(Inventory.Hand.Left) && !Inventory.HasGun(Inventory.Hand.Right))))
            {
                Shoot();
            }
        }
    }
    void Shoot()
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


        for (int i = 0; i < player.bulletsPerShot; i++)
        {

            Vector3 direction = targetPoint - attackPoint.position + RandomSpread();

            // creates bullet and sends it zoomin

            currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = direction.normalized;
            currentBullet.transform.localScale *= player.bulletSize;
            currentBullet.GetComponent<Rigidbody>().velocity = direction.normalized * player.bulletSpeed;

            // creates bullet with specified stats
            Damage damage = currentBullet.GetComponent<Damage>();
            damage.damage = player.bulletDamage;
            damage.knockback = player.bulletKnockback;


        }
        bulletsShot++;


        // attack speed

        if (allowInvoke)
        {
            Invoke("ResetShot", 1/player.attackSpeed);
            allowInvoke = false;
        }

        // time between all bullets from same tap

        if (bulletsShot < player.bulletsPerTap)
        {
            Invoke("Shoot", player.attackSpeed);
        }

        shootAnimator.Play("shoot", 0, 0f);

        shootAnimator.speed = player.attackSpeed;
    }


    Vector3 RandomSpread()
    {
        float x = Random.Range(-player.bulletSpread, player.bulletSpread);
        float y = Random.Range(-player.bulletSpread, player.bulletSpread);
        float z = Random.Range(-player.bulletSpread, player.bulletSpread);
        return new Vector3(x, y, z);
    }

    void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
    public void RunAtStart()
    {
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
        else
        {
            Debug.Log("ERRA");
        }
    }
}