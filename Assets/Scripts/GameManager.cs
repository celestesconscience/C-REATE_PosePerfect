using UnityEngine;

// Everything related to controlling the Game
public class GameManager : MonoBehaviour // <-- MonoBehaviour allows Unity to attach script to a GameObject
{
    public static GameManager instance;//allows other scripts to access gameManager variables

    public float score;

    public float scoreAddAmt;//how many points to get each time

    private int wallCounter;

    public float[] highScore = new float [5];

    public int[] randomWallNums = new int [5];

    public int randomColor, prevColor, numColors;

    public int currentNumPoses = 10;

    [Header("Wall Spawn Speed Settings")]
    public float wallSpawnSpeed;//currently 8
    public float wallEndSpawnSpeed;//currently 4
    public float wallSpawnScale;//currently .2
    private float startWallSpawn;

    [Header("Wall Speed Settings")]
    public float wallSpeed;//currently .6
    public float wallEndSpeed;//currently 1.2
    public float wallScale;//currently .3
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

        randomWallNumGenerator();
    }

    void Start()
    {
        //keeps starting speed for reset
        startWallSpawn = wallSpawnSpeed;
        startWallSpeed = wallSpeed; 

        randomColor = Random.Range(1 , numColors + 1);
        prevColor = randomColor;
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

        //changes the poses and color every that num walls
        if(wallCounter == 10)
        {   
            while(randomColor == prevColor){
                randomColor = Random.Range(0 , numColors);
            }
            prevColor = randomColor;

            randomWallNumGenerator();
            wallCounter = 0;
        }
    }

    public void AddScore()
    {
        score += scoreAddAmt;
        wallCounter ++;

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

    public void randomWallNumGenerator()
    {
        int count = 0;

        while(count < 5)//5 different numbers
        {
            int randomNum = Random.Range(1, currentNumPoses+1);//generates random number 1-10
            bool isDuplicate = false;

            for(int x = 0; x < count; x++)
            {
                if (randomWallNums[x] == randomNum)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                randomWallNums[count] = randomNum;
                count++;
            }
        }
    }
}   