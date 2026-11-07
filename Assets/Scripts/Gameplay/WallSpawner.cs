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
        Vector3 spawnPosition = Vector2.zero;

        // Randomly select one of the three lanes.
        int randomNum = Random.Range(0, 3);

        // Set the spawn position.
        switch (randomNum)
        {
            case 0:
                spawnPosition = new Vector2(locations[randomNum], 0);
                break;

            case 1:
                spawnPosition = new Vector2(locations[randomNum], 0);
                break;

            case 2:
                spawnPosition = new Vector2(locations[randomNum], 0);
                break;
        }

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
