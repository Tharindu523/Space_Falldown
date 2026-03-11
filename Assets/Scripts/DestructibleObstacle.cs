using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attached to the Fallen Column. 
/// Fixed: Now handles multiple child parts and hides them before destruction to play SFX.
/// </summary>
public class DestructibleObstacle : MonoBehaviour
{
    [Header("Settings")]
    public float fuseTime = 3.0f;
    public GameObject explosionPrefab;
    public AudioClip plantSound;
    public AudioClip explosionSound;

    [Header("Visuals")]
    public GameObject bombVisualOnObstacle;
    [Tooltip("Drag the 3 child column parts here. If left empty, the script will find them automatically.")]
    public GameObject[] columnParts;

    private bool isPlanted = false;

    void Start()
    {
        if (bombVisualOnObstacle != null) bombVisualOnObstacle.SetActive(false);

        // AUTOMATIC SETUP: If no parts are assigned, find all children with MeshRenderers
        if (columnParts == null || columnParts.Length == 0)
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            List<GameObject> parts = new List<GameObject>();

            foreach (MeshRenderer r in renderers)
            {
                // Don't include the bomb visual in the list of column parts
                if (r.gameObject != bombVisualOnObstacle)
                {
                    parts.Add(r.gameObject);
                }
            }
            columnParts = parts.ToArray();
        }
    }

    public void PlantBomb()
    {
        if (isPlanted) return;

        isPlanted = true;
        if (bombVisualOnObstacle != null) bombVisualOnObstacle.SetActive(true);

        // Play planting sound
        if (plantSound != null) AudioSource.PlayClipAtPoint(plantSound, transform.position);

        Debug.Log("Bomb planted! Get back!");
        StartCoroutine(DetonationSequence());
    }

    IEnumerator DetonationSequence()
    {
        yield return new WaitForSeconds(fuseTime);

        // 1. Spawn Explosion Particles
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 2. Play Explosion Sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1.0f);
        }

        // 3. VISUAL DESTRUCTION (Hide everything)
        // Loop through all assigned parts and hide them
        if (columnParts != null)
        {
            foreach (GameObject part in columnParts)
            {
                if (part != null) part.SetActive(false);
            }
        }

        if (bombVisualOnObstacle != null) bombVisualOnObstacle.SetActive(false);

        // Disable all colliders on this object and its children so the player can pass
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        // 4. LOGICAL DESTRUCTION (Wait for sound to finish)
        // We wait 2 seconds so the AudioSource created by PlayClipAtPoint isn't interrupted 
        // and any trailing particles can finish.
        yield return new WaitForSeconds(2.0f);

        Destroy(gameObject);
    }
}