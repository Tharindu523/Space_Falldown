using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages spawning aliens at random locations from a list of spawn points.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject alienPrefab;      // Drag your Alien Drone prefab here
    public Transform[] spawnPoints;    // List of locations where aliens can spawn
    public float timeBetweenSpawns = 5f;
    public int maxAliensInLevel = 10;

    [Header("Player Safety")]
    public Transform player;           // Reference to player to avoid spawning on their head
    public float minimumSpawnDistance = 10f; // Don't spawn closer than this

    private float spawnTimer;
    private List<GameObject> activeAliens = new List<GameObject>();

    void Start()
    {
        spawnTimer = timeBetweenSpawns;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner: No spawn points assigned! Create empty objects and drag them here.");
        }
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            spawnTimer = timeBetweenSpawns;

            // Cleanup list: Remove aliens that have been destroyed (died)
            activeAliens.RemoveAll(item => item == null);

            if (activeAliens.Count < maxAliensInLevel)
            {
                TrySpawnAlien();
            }
        }
    }

    void TrySpawnAlien()
    {
        // 1. Pick a random spawn point from our list
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // 2. Check if the point is far enough from the player
        float distanceToPlayer = Vector3.Distance(selectedPoint.position, player.position);

        if (distanceToPlayer >= minimumSpawnDistance)
        {
            // 3. Spawn the Alien!
            GameObject newAlien = Instantiate(alienPrefab, selectedPoint.position, selectedPoint.rotation);
            activeAliens.Add(newAlien);

            // Optional: Add a spawn effect (like a teleport sound or flash) here
            Debug.Log("Alien spawned at: " + selectedPoint.name);
        }
        else
        {
            // If the player was too close, wait a shorter time and try again
            spawnTimer = 1f;
        }
    }
}