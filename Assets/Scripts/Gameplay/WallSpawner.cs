using System.Collections;
using UnityEngine;

// Spawns incoming walls in random lanes at intervals controlled by the GameManager.
public class WallSpawner : MonoBehaviour
{
    // ==================== WALL PREFAB ====================

    // Reference to the wall prefab that will be spawned during gameplay.
    public GameObject wallPrefab;


    // ==================== SPAWN SETTINGS ====================

    // Unused spawn frequency variable.
    // The actual spawn interval is controlled by GameManager.wallSpawnSpeed.
    public float spawnFreq;

    // X positions of the three lanes, configured in the Inspector.
    // These should match the lane positions used by PlayerScript.
    public float[] locations = new float[3];


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Begin the coroutine responsible for repeatedly spawning walls.
        StartCoroutine("SpawnWalls");
    }


    // ==================== WALL SPAWNING ====================

    // Spawns a wall in a randomly selected lane, waits, then repeats.
    IEnumerator SpawnWalls()
    {
        // Initialize the position where the next wall will spawn.
        Vector3 spawnPosition = Vector2.zero;

        // Randomly select one of the three lanes.
        // Random.Range(0, 3) returns 0, 1, or 2 for integers.
        int randomNum = Random.Range(0, 3);

        // Set the spawn position based on the randomly selected lane.
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

        // Create a new wall at the selected position with no rotation.
        GameObject wall = Instantiate(
            wallPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Wait before spawning the next wall.
        // GameManager adjusts this interval as the difficulty increases.
        yield return new WaitForSeconds(GameManager.instance.wallSpawnSpeed);

        // Repeat the spawning process.
        StartCoroutine("SpawnWalls");
    }
}