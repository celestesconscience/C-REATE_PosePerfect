using UnityEngine;

// Manages the game's scoring, difficulty progression, available poses,
// wall colors, and shared gameplay settings across scenes.
public class GameManager : MonoBehaviour
{
    // ==================== GAME MANAGER INSTANCE ====================

    // Shared instance that allows other scripts to access the GameManager.
    public static GameManager instance;


    // ==================== SCORING SYSTEM ====================

    // Player's current score.
    public float score;

    // Number of points awarded for each successful wall match.
    public float scoreAddAmt;

    // Tracks successful wall matches toward the next pose and color update.
    private int wallCounter;

    // Stores five high score values.
    // Currently, highScore[0] is used to track the highest score.
    public float[] highScore = new float[5];


    // ==================== WALL POSES ====================

    // Stores the five randomly selected pose sprite indexes.
    // Used by PlayerScript, WallScript, and PoseUIScript.
    public int[] randomWallNums = new int[5];

    // Total number of available poses to randomly select from.
    public int currentNumPoses = 10;


    // ==================== WALL COLORS ====================

    // Current wall color, previous wall color, and total color options.
    public int randomColor, prevColor, numColors;


    // ==================== WALL SPAWN SPEED ====================

    [Header("Wall Spawn Speed Settings")]

    // Current time interval between spawning walls, in seconds.
    public float wallSpawnSpeed;

    // Minimum spawn interval allowed as difficulty increases.
    public float wallEndSpawnSpeed;

    // Amount subtracted from the spawn interval after each successful match.
    public float wallSpawnScale;

    // Stores the initial spawn interval so it can be reset for a new game.
    private float startWallSpawn;


    // ==================== WALL MOVEMENT SPEED ====================

    [Header("Wall Speed Settings")]

    // Current wall animation speed multiplier.
    public float wallSpeed;

    // Maximum wall animation speed multiplier.
    public float wallEndSpeed;

    // Amount added to the wall speed after each successful match.
    public float wallScale;

    // Stores the initial wall speed so it can be reset for a new game.
    private float startWallSpeed;


    // ==================== INITIALIZATION ====================

    // Awake runs before Start and initializes the shared GameManager instance.
    void Awake()
    {
        // Singleton pattern: prevent multiple GameManagers from existing.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Assign this GameManager as the shared instance.
        instance = this;

        // Keep the GameManager alive when switching between scenes.
        DontDestroyOnLoad(gameObject);

        // Generate the first set of five available poses.
        randomWallNumGenerator();
    }

    void Start()
    {
        // Save the starting difficulty settings for future resets.
        startWallSpawn = wallSpawnSpeed;
        startWallSpeed = wallSpeed;

        // Randomly select the initial wall color.
        randomColor = Random.Range(1, numColors + 1);

        // Remember the initial color to avoid repeating it during the next change.
        prevColor = randomColor;
    }


    // ==================== GAME STATE UPDATE ====================

    void Update()
    {
        // Update the highest score whenever the player exceeds it.
        if (score > highScore[0])
        {
            highScore[0] = score;
        }

        // Restore the starting wall speeds when the score is zero.
        // PlayerScript resets the score when a new game begins.
        if (score == 0)
        {
            wallSpawnSpeed = startWallSpawn;
            wallSpeed = startWallSpeed;
        }

        // Change the available poses and wall color every 10 successful matches.
        if (wallCounter == 10)
        {
            // Select a different wall color from the previous one.
            while (randomColor == prevColor)
            {
                randomColor = Random.Range(0, numColors);
            }

            // Save the new color for the next comparison.
            prevColor = randomColor;

            // Generate five new available poses.
            randomWallNumGenerator();

            // Reset the counter for the next group of 10 successful matches.
            wallCounter = 0;
        }
    }


    // ==================== SCORING & DIFFICULTY ====================

    // Awards points for a successful wall match and increases difficulty.
    // Called by PlayerScript after a successful collision.
    public void AddScore()
    {
        // Add the configured amount of points to the player's score.
        score += scoreAddAmt;

        // Count this wall toward the next pose and color update.
        wallCounter++;


        // ==================== SPAWN DIFFICULTY ====================

        // Gradually decrease the time between wall spawns.
        if (wallSpawnSpeed > wallEndSpawnSpeed)
        {
            wallSpawnSpeed -= wallSpawnScale;
        }

        // Prevent the spawn interval from dropping below its minimum.
        if (wallSpawnSpeed < wallEndSpawnSpeed)
        {
            wallSpawnSpeed = wallEndSpawnSpeed;
        }


        // ==================== MOVEMENT DIFFICULTY ====================

        // Gradually increase the speed of incoming walls.
        if (wallSpeed < wallEndSpeed)
        {
            wallSpeed += wallScale;
        }

        // Prevent the wall speed from exceeding its maximum.
        if (wallSpeed > wallEndSpeed)
        {
            wallSpeed = wallEndSpeed;
        }
    }


    // ==================== RANDOM POSE GENERATION ====================

    // Generates five unique random pose indexes from the available pose pool.
    // Called at initialization and after every 10 successful wall matches.
    public void randomWallNumGenerator()
    {
        // Tracks how many unique pose indexes have been selected.
        int count = 0;

        // Continue until all five pose slots have been filled.
        while (count < 5)
        {
            // Select a random pose index from 1 to currentNumPoses.
            // The upper bound is exclusive for integer Random.Range().
            int randomNum = Random.Range(1, currentNumPoses + 1);

            // Tracks whether the selected pose already exists in the array.
            bool isDuplicate = false;

            // Compare the new pose against all previously selected poses.
            for (int x = 0; x < count; x++)
            {
                if (randomWallNums[x] == randomNum)
                {
                    isDuplicate = true;
                    break;
                }
            }

            // Only add the pose if it has not already been selected.
            if (!isDuplicate)
            {
                randomWallNums[count] = randomNum;
                count++;
            }
        }
    }
}