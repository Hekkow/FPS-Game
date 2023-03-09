using System;
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
    public Addon addon;
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
    public int pelletLayers = 0;
    public float pelletSpread = 0;
    public List<int> pelletsPerLayer = new List<int>();

    bool readyToShoot = true;
    bool shooting = false;
    bool reloading = false;
    Coroutine reloadCoroutine;
    Transform cameraTransform;


    public static event Action onShot;
    public static event Action onBeforeReload;
    public static event Action onAfterReload;




    void Awake()
    {
        shootAnimator = GetComponent<Animator>();
        reloadCoroutine = StartCoroutine(WaitThenReload(0));
        StopCoroutine(reloadCoroutine);
        cameraTransform = Camera.main.transform;
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
        InputManager.playerInput.Player.Attachment.performed += ((obj) => addon.Activate());
        InputManager.playerInput.Player.Attachment.Enable();
        InputManager.playerInput.Player.Reload.performed += Reload;
        InputManager.playerInput.Player.Reload.Enable();

    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Reload.Disable();
        InputManager.playerInput.Player.Shoot.Disable();
        InputManager.playerInput.Player.Attachment.Disable();
        StopCoroutine(reloadCoroutine);
        readyToShoot = true; 
        
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
        float multiplier = 0.1f;

        float originalX = UnityEngine.Random.Range(-bulletSpread * multiplier, bulletSpread * multiplier);
        float originalY = UnityEngine.Random.Range(-bulletSpread * multiplier, bulletSpread * multiplier);
        Lazer(originalX, originalY);
        for (int i = 0; i < pelletLayers; i++)
        {
            for (int j = 0; j < pelletsPerLayer[i]; j++)
            {
                float radius = (i + 1) * pelletSpread;
                float angle = 360 * Mathf.Deg2Rad / pelletsPerLayer[i];
                float x = radius * Mathf.Cos(j * angle) + originalX;
                float y = radius * Mathf.Sin(j * angle) + originalY;
                Lazer(x, y);
            }
        }
        

        StartCoroutine(ResetShot());

        shootAnimator.Play("shoot", 0, 0f);
        shootAnimator.speed = attackSpeed;

        if (bulletsLeft <= 0)
        {
            reloadCoroutine = StartCoroutine(WaitThenReload(1 / attackSpeed));
        }

        onShot?.Invoke();

        
    }
    void Lazer(float x, float y)
    {
        bulletsLeft--;
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, cameraTransform.forward + cameraTransform.right * x + cameraTransform.up * y);
        if (Physics.SphereCast(ray, bulletSize, out hit) && hit.transform.gameObject.GetComponent<Player>() == null) 
        {
            if (hit.transform.TryGetComponent(out IDamageable damage))
            {
                damage.Damaged(bulletDamage, hit.collider, this);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody?.AddForce(Camera.main.transform.forward * bulletKnockback);
                //hit.rigidbody.AddForce(-hit.normal * bulletKnockback);
            }
            Instantiate(Resources.Load<GameObject>("Prefabs/BulletHole"), hit.point + hit.normal * 0.01f, Quaternion.identity, hit.transform);
        }
    }
    IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(1 / (attackSpeed));
        readyToShoot = true;
    }
    void Reload(InputAction.CallbackContext obj)
    {
        if (!reloading && bulletsPerMag > bulletsLeft && gameObject.activeSelf)
        {
            reloadCoroutine = StartCoroutine(WaitThenReload(0));
        }
    }
    IEnumerator WaitThenReload(float time)
    {
        yield return new WaitForSeconds(time);
        onBeforeReload?.Invoke();
        reloading = true;
        shootAnimator.Play("reload", 0, 0f);
        shootAnimator.speed = reloadSpeed;
        yield return new WaitForSeconds(1/reloadSpeed);
        reloading = false;
        ResetBullets();
        onAfterReload?.Invoke();
    }
    public void ResetBullets()
    {
        bulletsLeft = bulletsPerMag;
        readyToShoot = true;
    }
    public void ResetBulletsAfterUpgrade()
    {
        if (gameObject.activeSelf) StartCoroutine(ResetBulletsAfterUpgradeCoroutine());
        else
        {
            bulletsLeft = bulletsPerMag;
            readyToShoot = true; 
        }
    }
    IEnumerator ResetBulletsAfterUpgradeCoroutine()
    {
        yield return new WaitForEndOfFrame();

        bulletsLeft = bulletsPerMag;
        readyToShoot = true;
    }
}