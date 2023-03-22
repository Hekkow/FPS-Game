using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{
    [HideInInspector] public Animator shootAnimator;

    [Header("Objects")]
    [SerializeField] Player player;
    [SerializeField] Transform attackPoint;
    [SerializeField] GameObject hitImpact;
    

    [Header("Stats")]
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
    
    [Header("Pellets")]
    public int pelletLayers = 0;
    public float pelletSpread = 0;
    public List<int> pelletsPerLayer = new List<int>();

    [Header("Splitter")]
    public bool splitter = false;
    public float splitterDistance = 10;
    public float splitterSpread = 5;

    [Header("Upgrades")]
    public bool bouncer = false;
    public bool gravityFlip = true;
    

    bool readyToShoot = true;
    bool shooting = false;
    bool reloading = false;
    Coroutine reloadCoroutine;
    Transform cameraTransform;
    float particleLength;

    public static event Action onShot;
    public static event Action onBeforeReload;
    public static event Action onAfterReload;

    void Awake()
    {
        shootAnimator = GetComponent<Animator>();
        reloadCoroutine = StartCoroutine(WaitThenReload(0));
        StopCoroutine(reloadCoroutine);
        cameraTransform = Camera.main.transform;
        particleLength = 0.3f;
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
        if (shooting && readyToShoot && bulletsLeft > 0) Shoot();
    }

    void Shoot()
    {
        reloading = false;
        StopCoroutine(reloadCoroutine);
        readyToShoot = false;
        float multiplier = 0.1f;

        float originalX = UnityEngine.Random.Range(-bulletSpread * multiplier, bulletSpread * multiplier);
        float originalY = UnityEngine.Random.Range(-bulletSpread * multiplier, bulletSpread * multiplier);
        Lazer(1, originalX, originalY, 0f);
        for (int i = 0; i < pelletLayers; i++)
        {
            for (int j = 0; j < pelletsPerLayer[i]; j++)
            {
                float radius = (i + 1) * pelletSpread;
                float angle = 360 * Mathf.Deg2Rad / pelletsPerLayer[i];
                float x = radius * Mathf.Cos(j * angle) + originalX;
                float y = radius * Mathf.Sin(j * angle) + originalY;
                Lazer(1, x, y, 0f);
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
    void Lazer(float startDistance, float x, float y, float hitDelay)
    {
        bulletsLeft--;
        RaycastHit hit;
        Ray ray = new Ray(cameraTransform.position + cameraTransform.forward * startDistance, cameraTransform.forward + cameraTransform.right * x + cameraTransform.up * y);
        if (Physics.SphereCast(ray, bulletSize, out hit) && hit.transform.gameObject.GetComponent<Player>() == null) 
        { 
            StartCoroutine(Hit(hit, hitDelay));
        }
        if (splitter)
        {
            if (hit.collider == null || hit.distance > splitterDistance)
            {
                SplitterLazer(splitterDistance, splitterSpread, 0, 0.01f, ray);
                SplitterLazer(splitterDistance, -splitterSpread, 0, 0.01f, ray);
            }
        }
    }
    void SplitterLazer(float startDistance, float x, float y, float hitDelay, Ray originalRay)
    {
        RaycastHit hit;
        Ray ray = new Ray(originalRay.origin + originalRay.direction * startDistance, Quaternion.AngleAxis(x, Vector3.up) * originalRay.direction);
        if (Physics.SphereCast(ray, bulletSize, out hit) && hit.transform.gameObject.GetComponent<Player>() == null)
        {
            StartCoroutine(Hit(hit, hitDelay));
        }
    }
    IEnumerator Hit(RaycastHit hit, float hitDelay)
    {
        yield return new WaitForSeconds(hitDelay);
        //yield return new WaitForSeconds(hit.distance/bulletSpeed);

        if (hit.transform.TryGetComponent(out IDamageable damage))
        {
            damage.Damaged(bulletDamage, hit.collider, this);
        }
        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(Camera.main.transform.forward * bulletKnockback);
            Helper.GetOrAdd<BulletEffects>(hit.rigidbody.gameObject).FlipGravity(); 
            //hit.rigidbody.AddForce(-hit.normal * bulletKnockback);
        }
        HitImpact(hit);
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
    void HitImpact(RaycastHit hit)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/BulletHole"), hit.point + hit.normal * 0.01f, Quaternion.identity, hit.transform);
        GameObject particle = Instantiate(hitImpact, hit.point, Quaternion.identity);
        Destroy(particle, particleLength);
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