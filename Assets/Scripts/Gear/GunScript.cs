using UnityEngine;
using System.Collections;
using TMPro; // REQUIRED for using TextMeshPro UI components

/// <summary>
/// Handles all core combat logic: shooting (raycasting), damage application, 
/// fire rate control, ammunition tracking, reloading, and all associated visual/audio feedback 
/// including triggering weapon animations.
/// </summary>
public class GunScript : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.5f;

    [Header("Ammunition")]
    public int magSize = 30;
    public float reloadTime = 2.0f;
    private int currentAmmo;
    private bool isReloading = false;

    [Header("Component References")]
    // Drag your Player Camera here (the one this script is attached to)
    public Camera fpsCam;
    private AudioSource audioSource;
    // NEW: Drag the Animator component attached to your WeaponHolder here
    public Animator gunAnimator;

    [Header("HUD")]
    public TextMeshProUGUI ammoText;

    [Header("Visual Feedback Prefabs")]
    // The exact point on the gun where the flash should appear.
    public Transform muzzlePoint;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletDecalPrefab;

    [Header("Audio Feedback")]
    public AudioClip fireSound;
    public AudioClip hitSoundEnemy;
    public AudioClip hitSoundEnvironment;
    public AudioClip reloadSound;

    // Time variables for managing the fire rate
    private float nextTimeToFire = 0f;

    void Start()
    {
        // Initialize state and components
        currentAmmo = magSize;

        if (fpsCam == null)
        {
            fpsCam = Camera.main;
            if (fpsCam == null)
            {
                Debug.LogError("GunScript could not find a Camera tagged 'MainCamera'. Set the reference manually.");
                enabled = false;
                return;
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("GunScript requires an AudioSource component on the same GameObject (Camera).");
        }

        if (gunAnimator == null)
        {
            Debug.LogError("Gun Animator reference is missing. Drag the WeaponHolder's Animator here.");
        }

        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading)
            return;

        // 1. Check for Fire Input (Hold Left Mouse Button)
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                // Auto-reload if we try to shoot with no ammo
                StartCoroutine(Reload());
            }
        }

        // 2. Check for Manual Reload Input (Default: R key)
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magSize)
        {
            StartCoroutine(Reload());
        }
    }

    /// <summary>
    /// Executes the raycast, damage, effects, and animation logic.
    /// </summary>
    void Shoot()
    {
        // Consume one bullet
        currentAmmo--;
        UpdateAmmoUI();

        // Trigger the Fire animation (uses the 'Fire' trigger in the Animator)
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Fire");
        }

        // Play the main firing sound
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        // Trigger Muzzle Flash (Visual Feedback)
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            // NEW: Added muzzlePoint as the parent to ensure flash follows the moving gun
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);
            StartCoroutine(DestroyMuzzleFlash(flash, 0.5f));
        }

        // Raycast logic
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            Health targetHealth = hit.transform.GetComponent<Health>();

            // --- DAMAGE & HIT SOUND LOGIC ---
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                if (audioSource != null && hitSoundEnemy != null)
                {
                    audioSource.PlayOneShot(hitSoundEnemy);
                }
            }
            else
            {
                if (audioSource != null && hitSoundEnvironment != null)
                {
                    audioSource.PlayOneShot(hitSoundEnvironment);
                }
            }

            // --- BULLET DECAL LOGIC ---
            if (bulletDecalPrefab != null)
            {
                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                rotation *= Quaternion.Euler(0, 0, Random.Range(0, 360));
                GameObject decal = Instantiate(bulletDecalPrefab, hit.point + hit.normal * 0.01f, rotation);
                decal.transform.SetParent(hit.transform);
                Destroy(decal, 10f);
            }
        }
    }

    /// <summary>
    /// Coroutine to handle the reload delay. Prevents firing until complete.
    /// </summary>
    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        // Trigger the Reload animation (uses the 'Reload' trigger in the Animator)
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Reload");
        }

        // Play reload sound
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        // Wait for the specified reload time
        yield return new WaitForSeconds(reloadTime);

        // Refill the magazine
        currentAmmo = magSize;
        isReloading = false;
        UpdateAmmoUI();
        Debug.Log("Reload Complete.");
    }

    /// <summary>
    /// Updates the HUD display for the current ammo count.
    /// </summary>
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + magSize;
        }
    }

    /// <summary>
    /// Destroys a temporary visual effect.
    /// </summary>
    IEnumerator DestroyMuzzleFlash(GameObject flashObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(flashObject);
    }
}