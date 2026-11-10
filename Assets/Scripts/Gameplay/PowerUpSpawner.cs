using UnityEngine;
using System.Collections;

// Controls when and where power-ups spawn during gameplay.
// Uses randomized spawn intervals and the GameManager's
// power-up lane selection system.
public class PowerUpSpawner : MonoBehaviour
{
    // ==================== POWER-UP PREFAB ====================

    // Prefab instantiated whenever a power-up spawns.
    public GameObject powerUpPrefab;


    // ==================== SPAWN LOCATIONS ====================

    // X positions of the three gameplay lanes.
    // These should match the player's lane positions.
    public float[] locations = new float[3];


    // ==================== SPAWN TIMING ====================

    [Header("Power-Up Spawn Settings")]

    // Minimum number of seconds between power-up spawns.
    public int minTime;

    // Exclusive upper limit for the random spawn delay.
    public int maxTime;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        if (GameManager.instance == null)
            return;

        // Reset the power-up location at the start.
        // 10 represents no active power-up lane.
        GameManager.instance.powerUpLocation = 10;

        // Reset active power-up tracking flags.
        GameManager.instance.armorAlreadyOn = false;
        GameManager.instance.doublePointsAlreadyOn = false;

        // Begin continuously spawning power-ups.
        StartCoroutine(SpawnPowerUps());
    }


    // ==================== POWER-UP SPAWNING ====================

    private IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            // Choose a random delay before the next spawn.
            float waitTime = Random.Range(minTime, maxTime);

            // Wait for the selected amount of time.
            yield return new WaitForSeconds(waitTime);

            // Stop if the GameManager is unavailable.
            if (GameManager.instance == null)
                yield break;

            // Ask GameManager to select a power-up lane.
            GameManager.instance.randomPowerUpLocation();

            int selectedLane =
                GameManager.instance.powerUpLocation;

            // Ensure the selected lane is valid.
            if (selectedLane < 0 ||
                selectedLane >= locations.Length)
            {
                Debug.LogWarning(
                    "Power-up spawn skipped: Invalid lane."
                );

                continue;
            }

            // Determine the spawn position.
            Vector2 spawnPosition = new Vector2(
                locations[selectedLane],
                0f
            );

            // Spawn the power-up in the selected lane.
            Instantiate(
                powerUpPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }
}