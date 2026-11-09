using System.Collections;
using UnityEngine;

// Spawns incoming walls in random lanes at intervals
// controlled by the GameManager.
public class WallSpawner : MonoBehaviour
{
    // ==================== WALL PREFAB ====================

    // Reference to the wall prefab.
    public GameObject wallPrefab;


    // ==================== SPAWN SETTINGS ====================

    // Unused variable, kept from the original script.
    // GameManager.wallSpawnSpeed controls the actual interval.
    public float spawnFreq;

    // X positions of the three lanes.
    public float[] locations = new float[3];


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Begin spawning walls.
        StartCoroutine("SpawnWalls");
    }


    // ==================== WALL SPAWNING ====================

    IEnumerator SpawnWalls()
    {
        // Initialize the spawn position.
        Vector2 spawnPosition;

        GameManager.instance.randomWallLocation();
        spawnPosition = new Vector2(locations[GameManager.instance.wallLocation], 0);

        // Spawn the wall.
        GameObject wall = Instantiate(
            wallPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Wait before spawning the next wall.
        yield return new WaitForSeconds(
            GameManager.instance.wallSpawnSpeed
        );

        // Repeat spawning.
        StartCoroutine("SpawnWalls");
    }
}
