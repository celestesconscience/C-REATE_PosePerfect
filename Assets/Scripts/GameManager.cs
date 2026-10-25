using UnityEngine;

// Everything related to controlling the Game
public class GameManager : MonoBehaviour // <-- MonoBehaviour allows Unity to attach script to a GameObject
{
    public static GameManager instance;//allows other scripts to access gameManager variables

    public float score;

    public float scoreAddAmt;//how many points to get each time

    private int wallCounter;

    public float[] highScore = new float [5];

    [Header("Wall Spawn Speed Settings")]
    public float wallSpawnSpeed;
    public float wallEndSpawnSpeed;
    public float wallSpawnScale;
    private float startWallSpawn;

    [Header("Wall Speed Settings")]
    public float wallSpeed;
    public float wallEndSpeed;
    public float wallScale;
    private float startWallSpeed;

    //happens before start, only for important stuff
    void Awake()
    {
        //Singleton Pattern, allows it to persist across scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //keeps starting speed for reset
        startWallSpawn = wallSpawnSpeed;
        startWallSpeed = wallSpeed; 
    }

    
    void Update()
    {
        //if Score is greater than highScore make highScore score
        if(score > highScore[0])
        {
            highScore[0] = score;
        }

        if(score == 0)//when reset go back to original speeds;
        {
            wallSpawnSpeed = startWallSpawn;
            wallSpeed = startWallSpeed;
        }
    }

    public void AddScore()
    {
        score += scoreAddAmt;

        //DECREASES SPAWN SPEED OF WALL GRADUALLY AS SCORE INCREASES
        if(wallSpawnSpeed > wallEndSpawnSpeed)
        {
            wallSpawnSpeed -= wallSpawnScale;
        }
        
        if(wallSpawnSpeed < wallEndSpawnSpeed)
        {
            wallSpawnSpeed = wallEndSpawnSpeed;
        }

        //INCREASES SPEED OF THE WALL GRADUALLY AS SCORE INCREASES
        if(wallSpeed < wallEndSpeed)
        {
            wallSpeed += wallScale;
        }
        if(wallSpeed > wallEndSpeed)
        {
            wallSpeed = wallEndSpeed;
        }
    }
    
}