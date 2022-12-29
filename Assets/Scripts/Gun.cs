using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform attackPoint;
    [SerializeField] Upgrades upgrades;
    [SerializeField] InputManager input;
    [SerializeField] Animations animation;

    Player player;

    GameObject currentBullet;
    int bulletsShot;
    bool readyToShoot = true;
    bool allowInvoke = true;

    new Camera camera;

    void Awake()
    {
        camera = Camera.main;
        player = GetComponent<Player>();
    }
    void Update()
    {
        if (input.mouse && readyToShoot && Inventory.HasGun())
        {
            bulletsShot = 0;
            Shoot();
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

        animation.ReloadGun();
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
}