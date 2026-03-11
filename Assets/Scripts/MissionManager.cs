using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the display of current mission objectives and inventory icons.
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance; // Simple singleton for easy access

    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public GameObject bombIcon; // The UI Image showing the bomb in inventory

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateObjective("Find a way to clear the path.");
        if (bombIcon != null) bombIcon.SetActive(false);
    }

    public void UpdateObjective(string newText)
    {
        if (objectiveText != null)
        {
            objectiveText.text = "OBJECTIVE: " + newText;
        }
    }

    public void SetBombIcon(bool isVisible)
    {
        if (bombIcon != null)
        {
            bombIcon.SetActive(isVisible);
        }
    }
}