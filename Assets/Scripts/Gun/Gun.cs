using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [HideInInspector] public Animator shootAnimator;

    [Header("Objects")]
    [SerializeField] Player player;
    [SerializeField] Transform attackPoint;

    public GameEvent onGunShot;
    public GameEvent beforeReload;
    public GameEvent afterReload;

    public int slot = -1;
    public int bulletsPerShot = 1;
    public int bulletsPerTap = 1;
    public float bulletSize = 0.1f;
    public float bulletSpeed = 200;
    public float bulletDamage = 34;
    public float bulletKnockback = 2000;
    public float attackSpeed = 1;
    public float bulletSpread = 0;
    public float reloadSpeed = 1f;
    public int bulletsPerMag = 6;
    public int bulletsLeft = 6;
    public int shotsPerMag = 6;

    bool readyToShoot = true;
    bool shooting = false;
    bool reloading = false;
    Coroutine reloadCoroutine;
    GameObject bulletPrefab;

    void Awake()
    {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
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
        InputManager.playerInput.Player.Shoot.Enable(); 
        InputManager.playerInput.Player.Reload.performed += Reload;
        InputManager.playerInput.Player.Reload.Enable();

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
                Shoot();
            }
        }
    }

    void Shoot()
    {
        reloading = false;
        StopCoroutine(reloadCoroutine);
        readyToShoot = false;
        for (int i = 0; i < bulletsPerShot; i++)
        {
            bulletsLeft--;
            RaycastHit hit;
            if (Physics.SphereCast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), bulletSize, out hit))
            {
                if (hit.transform.TryGetComponent(out IDamageable damage))
                {
                    damage.Damaged(bulletDamage, hit.collider);
                }
                if (hit.rigidbody != null)
                {
                    //hit.rigidbody.AddForce(Camera.main.transform.forward * 200);
                    hit.rigidbody.AddForce(-hit.normal * 300);
                }
            }
        }

        StartCoroutine(ResetShot());

        shootAnimator.Play("shoot", 0, 0f);
        shootAnimator.speed = attackSpeed;

        if (bulletsLeft <= 0)
        {
            reloadCoroutine = StartCoroutine(WaitThenReload(1 / attackSpeed));
        }

        onGunShot.Raise(null, null);

        
    }

    //void ShootGun()
    //{
    //    reloading = false;
    //    StopCoroutine(reloadCoroutine);
    //    readyToShoot = false;
    //    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //    RaycastHit hit;
    //    Vector3 targetPoint;
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        targetPoint = hit.point;
    //    }
    //    else
    //    {
    //        targetPoint = ray.GetPoint(100);
    //    }

    //    for (int i = 0; i < bulletsPerShot; i++)
    //    {
    //        bulletsLeft--;
    //        Vector3 direction = targetPoint - attackPoint.position;

    //        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
    //        currentBullet.transform.forward = direction.normalized;
    //        Vector3 randomSpread = RandomSpread();
    //        currentBullet.transform.Rotate(randomSpread);
    //        currentBullet.transform.localScale *= bulletSize;
    //        currentBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * currentBullet.transform.forward;
    //        Helper.AddDamage(currentBullet, bulletDamage, bulletKnockback, false, false);
    //        Bullet bullet = currentBullet.GetComponent<Bullet>();
    //    }

    //    StartCoroutine(ResetShot());

    //    shootAnimator.Play("shoot", 0, 0f);
    //    shootAnimator.speed = attackSpeed;

    //    if (bulletsLeft <= 0)
    //    {
    //        reloadCoroutine = StartCoroutine(WaitThenReload(1/attackSpeed));
    //    }

    //    onGunShot.Raise(null, null);

    //} 
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
        beforeReload.Raise(null, null);
        reloading = true;
        shootAnimator.Play("reload", 0, 0f);
        shootAnimator.speed = reloadSpeed;
        yield return new WaitForSeconds(1/reloadSpeed);
        reloading = false;
        ResetBullets();
        afterReload.Raise(null, null);
    }
    public void ResetBullets()
    {
        bulletsLeft = bulletsPerMag;
        readyToShoot = true;
    }
    public void ResetBulletsAfterUpgrade()
    {
        bulletsLeft = bulletsPerMag;
        readyToShoot = false;
    }
}