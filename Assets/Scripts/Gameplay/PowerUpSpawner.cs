using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;

    public float[] locations = new float [3];

    //minimum time required to wait before another powerup spawn
    public int minTime;

    //maximum time required to wait before a powerup is spawned
    public int maxTime;

    private float waitTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {     
        //deactivates it in the beginning
        GameManager.instance.powerUpLocation = 10;

        StartCoroutine("SpawnPowerUps");

        GameManager.instance.armorAlreadyOn = false;
        GameManager.instance.doublePointsAlreadyOn = false;
    }

    IEnumerator SpawnPowerUps()
    {   
        //Generates a random number to see how long until powerup spawns
        waitTime = Random.Range(minTime, maxTime);

        //How long until powerup spawns
        yield return new WaitForSeconds(waitTime);

        Vector2 spawnPosition;

        GameManager.instance.randomPowerUpLocation();
        spawnPosition = new Vector2(locations[GameManager.instance.powerUpLocation], 0);

        // Spawn the powerup.
        GameObject wall = Instantiate(
            powerUpPrefab,
            spawnPosition,
            Quaternion.identity
        );

        StartCoroutine("SpawnPowerUps");

    }
}
