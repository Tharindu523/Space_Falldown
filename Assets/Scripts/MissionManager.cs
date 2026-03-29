using UnityEngine;
using TMPro;

/// <summary>
/// The central hub for all HUD elements (Objectives, Bomb Icon, and Ammo UI).
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("Objective UI")]
    public TextMeshProUGUI objectiveText;

    [Header("Inventory Icons")]
    public GameObject bombIcon;

    [Header("Weapon UI")]
    public GameObject ammoHUD; // Drag your Ammo Counter UI group here

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Initial State
        UpdateObjective("Locate a weapon and find a way out.");
        if (bombIcon != null) bombIcon.SetActive(false);
        if (ammoHUD != null) ammoHUD.SetActive(false);
    }

    public void ShowLockedMessage(string message)
    {
        StopAllCoroutines(); // Stop previous messages if they are still showing
        StartCoroutine(FlashMessage(message));
    }

    private System.Collections.IEnumerator FlashMessage(string msg)
    {
        string originalText = objectiveText.text;
        objectiveText.text = "<color=red>" + msg + "</color>";
        yield return new WaitForSeconds(3.0f);
        objectiveText.text = originalText;
    }


    public void UpdateObjective(string newText)
    {
        if (objectiveText != null) objectiveText.text = "OBJECTIVE: " + newText;
    }

    public void SetBombIcon(bool isVisible)
    {
        if (bombIcon != null) bombIcon.SetActive(isVisible);
    }

    public void SetAmmoHUD(bool isVisible)
    {
        if (ammoHUD != null) ammoHUD.SetActive(isVisible);
    }
}