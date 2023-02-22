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

    public GameEvent onGunShot;
    public GameEvent onReload;


    public int slot = -1;
    public int bulletsPerShot = 1;
    public int bulletsPerTap = 1;
    public float bulletSize = 7;
    public float bulletSpeed = 200;
    public float bulletDamage = 34;
    public float bulletKnockback = 2000;
    public float attackSpeed = 1;
    public float bulletSpread = 0;
    public float reloadTime = 1f;
    public int bulletsPerMag = 6;
    public int bulletsLeft = 6;
    public int shotsPerMag = 6;


    public bool gravityFlip = false;


    bool readyToShoot = true;
    bool shooting = false;
    bool reloading = false;
    new Camera camera;
    Coroutine reloadCoroutine;
    GameObject bulletPrefab;

    void Awake()
    {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        camera = Camera.main;
        shootAnimator = GetComponent<Animator>();
        reloadCoroutine = StartCoroutine(WaitThenReload(0));
        StopCoroutine(reloadCoroutine);
    }

    void OnEnable()
    {
        if (bulletsLeft <= 0)
        {
            reloadCoroutine = StartCoroutine(WaitThenReload(0));
        }
        InputManager.playerInput.Player.Shoot.performed += (obj) => shooting = true;
        InputManager.playerInput.Player.Shoot.canceled += (obj) => shooting = false;
        InputManager.playerInput.Player.Reload.performed += Reload;
        InputManager.playerInput.Player.Reload.Enable();
        InputManager.playerInput.Player.Shoot.Enable();
        
    }
    void OnDisable()
    {
        StopCoroutine(reloadCoroutine);
        readyToShoot = true; 
        InputManager.playerInput.Player.Reload.Disable();
        InputManager.playerInput.Player.Shoot.Disable();
    }
    void Update()
    {
        if (shooting)
        {
            if (readyToShoot && bulletsLeft > 0)
            {
                ShootGun();
            }
        }
        
    }

    void ShootGun()
    {
        reloading = false;
        StopCoroutine(reloadCoroutine);
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


        for (int i = 0; i < bulletsPerShot; i++)
        {
            bulletsLeft--;
            Vector3 direction = targetPoint - attackPoint.position;// + RandomSpread();

            // creates bullet and sends it zoomin

            GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
            currentBullet.layer = 20;
            currentBullet.transform.forward = direction.normalized;
            Vector3 randomSpread = RandomSpread();
            currentBullet.transform.Rotate(randomSpread);
            currentBullet.transform.localScale *= bulletSize;
            currentBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * currentBullet.transform.forward;
            
            // creates bullet with specified stats
            Helper.AddDamage(currentBullet, bulletDamage, bulletKnockback, false, false);
            Bullet bullet = currentBullet.GetComponent<Bullet>();
            bullet.gravityFlip = gravityFlip;

        }

        StartCoroutine(ResetShot());

        shootAnimator.Play("shoot", 0, 0f);

        shootAnimator.speed = attackSpeed;
        if (bulletsLeft <= 0)
        {
            reloadCoroutine = StartCoroutine(WaitThenReload(1/attackSpeed));
        }
        onGunShot.Raise();

    }

    IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(1 / (attackSpeed));
        readyToShoot = true;
    }
    Vector3 RandomSpread()
    {

        float x = Random.Range(-bulletSpread, bulletSpread);
        float y = Random.Range(-bulletSpread, bulletSpread);
        float z = 0;
        return new Vector3(x, y, z);
    }
    void Reload(InputAction.CallbackContext obj)
    {

        if (!reloading && bulletsPerMag > bulletsLeft)
        {
            reloadCoroutine = StartCoroutine(WaitThenReload(0));
        }
    }
    IEnumerator WaitThenReload(float time)
    {
        yield return new WaitForSeconds(time);
        reloading = true;
        shootAnimator.Play("reload", 0, 0f);
        shootAnimator.speed = 1/reloadTime;
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        ResetBullets();
        onReload.Raise();
    }
    public void ResetBullets()
    {
        bulletsLeft = bulletsPerMag;
        readyToShoot = true;
    }
}