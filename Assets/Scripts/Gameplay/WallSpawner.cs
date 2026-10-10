using System.Collections;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{

    public GameObject wallPrefab;

    public float spawnFreq;//how long until next wall

    public float[] locations = new float[3];//just copy the location of the player and put here

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("SpawnWalls");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnWalls()//RANDOM WALLS
    {
        
        Vector3 spawnPosition = Vector2.zero;

        int randomNum = Random.Range(0,3);//returns a number from 0-2
        
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

        GameObject wall = Instantiate(wallPrefab, spawnPosition, Quaternion.identity);//spawns wall

        yield return new WaitForSeconds(spawnFreq);//waits spawnFreq amount of seconds before starting again

        StartCoroutine("SpawnWalls");
    }
}
