using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{
    [HideInInspector] public Animator shootAnimator;

    [Header("Objects")]
    [SerializeField] GameObject hitImpact;
    

    [Header("Stats")]
    public Addon addon = null;
    public int slot = -1;
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
    public int pelletLayers = 0;
    public float pelletSpread = 0;
    public List<int> pelletsPerLayer = new List<int>();

    [Header("Splitter")]
    public float splitterDistance = 10;
    public float splitterSpread = 5;

    [Header("Upgrades")]
    public bool splitter = false;
    public bool bouncer = false;
    public bool gravityFlip = false;
    
    enum ForceDirection
    {
        towardPlayer,
        hitNormal
    }

    bool readyToShoot = true;
    bool shooting = false;
    bool reloading = false;
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
        InputManager.playerInput.Player.Shoot.performed += (obj) => shooting = true;
        InputManager.playerInput.Player.Shoot.canceled += (obj) => shooting = false;
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
        if (Physics.SphereCast(ray, bulletSize, out hit) && !hit.transform.GetComponent<Player>()) 
        { 
            StartCoroutine(Hit(ray, hit, hitDelay, ForceDirection.towardPlayer));
        }
        if (splitter)
        {
            if (hit.collider == null || hit.distance > splitterDistance)
            {
                LazerFromRay(splitterDistance, splitterSpread, 0, 0.01f, ray);
                LazerFromRay(splitterDistance, -splitterSpread, 0, 0.01f, ray);
            }
            else LazerFromRay(hit.distance - 1, 0, 0, 0.01f, ray);
        }
        
    }
    void LazerFromRay(float startDistance, float x, float y, float hitDelay, Ray originalRay)
    {
        RaycastHit hit;
        Ray ray = new Ray(originalRay.origin + originalRay.direction * startDistance, Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right) * originalRay.direction);
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
            if (hit.collider.TryGetComponent(out WeakPoint wp))
                damage.Damaged(bulletDamage * wp.multiplier, hit.collider, this);
            else damage.Damaged(bulletDamage, hit.collider, this);
        }
        ForceMode forceMode = ForceMode.Force;
        if (hit.transform.GetComponentInParent<DeadEnemy>()) forceMode = ForceMode.VelocityChange;
        if (hit.rigidbody != null)
        {
            if (mode == ForceDirection.towardPlayer) hit.rigidbody.AddForce(Camera.main.transform.forward * bulletKnockback, forceMode);
            else if (mode == ForceDirection.hitNormal) hit.rigidbody.AddForce(-hit.normal * bulletKnockback, forceMode);
            if (gravityFlip) hit.rigidbody.gameObject.GetOrAdd<BulletEffects>().FlipGravity();
        }
        if (hit.transform.TryGetComponent(out MeshDestroy md))
        {
            md.DestroyMesh();
        }
        if (bouncer && hit.collider != null)
        {
            LazerFromPoint(0, 0, 0.1f, ray, hit);
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
    public void ActivateAddon(InputAction.CallbackContext obj)
    {
        addon?.Activate();
    }
}