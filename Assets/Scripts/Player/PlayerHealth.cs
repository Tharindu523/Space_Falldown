using UnityEngine;
using UnityEngine.UI; // For the Health Bar Slider
using TMPro;         // For the Health Text
using System.Collections;

/// <summary>
/// Manages player health, UI updates, and the death state.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("UI References")]
    public Slider healthBar;          // Drag your UI Slider here
    public TextMeshProUGUI healthText; // Drag your TMP Text here
    public GameObject gameOverPanel;   // Drag your Game Over screen here
    public Image damageFlashImage;     // Optional: A red image that flashes when hit
    public Color flashColor = new Color(1, 0, 0, 0.3f);
    public float flashSpeed = 5f;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (damageFlashImage != null) damageFlashImage.color = Color.clear;
    }

    void Update()
    {
        // Smoothly fade away the damage flash
        if (damageFlashImage != null && damageFlashImage.color != Color.clear)
        {
            damageFlashImage.color = Color.Lerp(damageFlashImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Called by AlienAI to damage the player.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Trigger visual feedback
        if (damageFlashImage != null) damageFlashImage.color = flashColor;

        // Ensure health doesn't drop below zero
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBar != null) healthBar.value = currentHealth / maxHealth;
        if (healthText != null) healthText.text = "HEALTH: " + Mathf.RoundToInt(currentHealth) + "%";
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");

        // 1. Show Game Over Screen
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // 2. Disable Player movement and shooting
        GetComponent<PlayerMovement>().enabled = false;
        // Find the camera and disable the gun script
        GetComponentInChildren<GunScript>().enabled = false;

        // 3. Unlock cursor so user can click 'Restart'
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Allows other scripts (like health packs) to heal the player.
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }
}