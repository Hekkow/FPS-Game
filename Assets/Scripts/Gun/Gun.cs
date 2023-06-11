using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [HideInInspector] public Animator shootAnimator;

    [Header("Objects")]
    [SerializeField] GameObject hitImpact;

    [Header("Stats")]
    public Addon addon = null;
    public GunSlot upgradeSlot = new GunSlot();
    public int bulletsPerShot = 1;
    public int bulletsPerTap = 1;
    public float bulletSize = 0.1f;
    public float bulletSpeed = 200;
    public float bulletDamage = 34;
    public float bulletKnockback = 5000000;
    public float attackSpeed = 1;
    public float bulletSpread = 0;
    public float reloadSpeed = 1f;
    public int bulletsPerMag = 6;
    public int bulletsLeft = 6;
    public int shotsPerMag = 6;
    
    [Header("Pellets")]
    [HideInInspector] public int pelletLayers = 0;
    [HideInInspector] public float pelletSpread = 0;
    [HideInInspector] public List<int> pelletsPerLayer = new List<int>();

    [Header("Explosive Bullets")]
    [Rename("Radius")] public float explosionRadius = 5;
    [Rename("Force")] public float explosionForce = 30;
    [Rename("Up Force")] public float explosionUpForce = 1;
    [Rename("Damage")] public float explosionDamage = 15;

    [Header("Upgrades")]
    public bool bouncer = false;
    
    enum ForceDirection
    {
        towardPlayer,
        hitNormal
    }

    public bool readyToShoot = true;
    public bool shooting = false;
    public bool reloading = false;
    Coroutine reloadCoroutine;
    Transform cameraTransform;
    float particleLength = 0.3f;

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
        InputManager.playerInput.Player.Shoot.performed += _ => shooting = true;
        InputManager.playerInput.Player.Shoot.canceled += _ => shooting = false;
        InputManager.playerInput.Player.Attachment.performed += ActivateAddon;
        InputManager.playerInput.Player.Reload.performed += Reload;
        InputManager.playerInput.Player.Attachment.Enable();
        InputManager.playerInput.Player.Reload.Enable();
        InputManager.playerInput.Player.Shoot.Enable();
    }
    void OnDisable()
    {
        InputManager.playerInput.Player.Attachment.performed -= ActivateAddon;
        InputManager.playerInput.Player.Reload.performed -= Reload;
        InputManager.playerInput.Player.Attachment.Disable();
        InputManager.playerInput.Player.Reload.Disable();
        InputManager.playerInput.Player.Shoot.Disable();
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
        Lazer(originalX, originalY, 0f);
        for (int i = 0; i < pelletLayers; i++)
        {
            for (int j = 0; j < pelletsPerLayer[i]; j++)
            {
                float radius = (i + 1) * pelletSpread;
                float angle = 360 * Mathf.Deg2Rad / pelletsPerLayer[i];
                float x = radius * Mathf.Cos(j * angle) + originalX;
                float y = radius * Mathf.Sin(j * angle) + originalY;
                Lazer(x, y, 0f);
            }
        }
        
        StartCoroutine(ResetShot());

        shootAnimator.Play("shoot", 0, 0f);
        shootAnimator.speed = upgradeSlot.attackSpeed;

        if (bulletsLeft <= 0)
        {
            reloadCoroutine = StartCoroutine(WaitThenReload(1 / upgradeSlot.attackSpeed));
        }

        onShot?.Invoke();

        
    }
    void Lazer(float x, float y, float hitDelay)
    {
        bulletsLeft--;
        RaycastHit hit;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward + cameraTransform.right * x + cameraTransform.up * y);
        if (Physics.SphereCast(ray, bulletSize, out hit) && !hit.transform.GetComponent<Player>()) 
        { 
            StartCoroutine(Hit(ray, hit, hitDelay, ForceDirection.towardPlayer));
        }
    }
    void LazerFromPoint(float x, float y, float hitDelay, Ray originalRay, RaycastHit originalHit)
    {
        RaycastHit hit;
        Ray ray = new Ray(originalHit.point, Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right) * Vector3.Reflect(originalRay.direction, originalHit.normal));
        if (Physics.SphereCast(ray, bulletSize, out hit) && !hit.transform.GetComponent<Player>())
        {
            StartCoroutine(Hit(ray, hit, hitDelay, ForceDirection.hitNormal));
        }
    }
    IEnumerator Hit(Ray ray, RaycastHit hit, float hitDelay, ForceDirection mode)
    {
        yield return new WaitForSeconds(hitDelay);
        //yield return new WaitForSeconds(hit.distance/bulletSpeed);
        if (hit.transform.TryGetComponentInParent(out IDamageable damage))
        {
            damage.Damaged(bulletDamage, hit.collider, this);
        }
        ForceMode forceMode = ForceMode.Force;
        if (hit.rigidbody != null)
        {
            if (mode == ForceDirection.towardPlayer) hit.rigidbody.AddForce(Camera.main.transform.forward * bulletKnockback, forceMode);
            else if (mode == ForceDirection.hitNormal) hit.rigidbody.AddForce(-hit.normal * bulletKnockback, forceMode);
        }
        if (bouncer && hit.collider != null)
        {
            LazerFromPoint(0, 0, 0.1f, ray, hit);
        }
        HitImpact(hit);
    }
    IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(1 / (upgradeSlot.attackSpeed));
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
    public void ActivateAddon(InputAction.CallbackContext obj)
    {
        addon?.Activate();
    }
}