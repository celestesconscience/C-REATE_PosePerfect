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

    // Total number of poses available in the game.
    public int currentNumPoses = 20;

    // Number of poses the player can currently cycle through.
    // Starts at 3 and increases to a maximum of 5.
    public int activePoseCount = 3;

    // Increases whenever the available pose selection changes.
    // Other scripts can use this to detect a new pose set.
    public int poseSetVersion { get; private set; } = 0;

    // Tracks total successful matches during the current game.
    private int totalSuccessfulMatches = 0;

    // Tracks the current stage of pose progression.
    private int poseStage = 1;

    // Tracks which of the 20 poses have already been introduced.
    private bool[] introducedPoses = new bool[21];

    // Score required for the next random rotation after all 20 poses are introduced.
    private float nextPoseRotationScore = 3300f;


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

        // Save the starting difficulty settings for future resets.
        startWallSpawn = wallSpawnSpeed;
        startWallSpeed = wallSpeed;

        // Keep the GameManager alive when switching between scenes.
        DontDestroyOnLoad(gameObject);

        // Generate the starting three poses: Idle and two random poses.
        GenerateStartingPoses();
    }

    void Start()
    {
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
        
        // Change the wall color every 10 successful matches.
        if (wallCounter >= 10)
        {
            // Select a different wall color from the previous one.
            if (numColors > 1)
            {
                do
                {
                    randomColor = Random.Range(1, numColors + 1);
                }
                while (randomColor == prevColor);

                // Save the new color for the next comparison.
                prevColor = randomColor;
            }

            // Reset the counter for the next 10 successful matches.
            wallCounter = 0;
        }
    }

    // ==================== NEW GAME RESET ====================

    // Resets progression when starting a new game.
    public void ResetGame()
    {
        // Start counting successful matches from zero.
        wallCounter = 0;

        // Restore the starting difficulty settings.
        wallSpawnSpeed = startWallSpawn;
        wallSpeed = startWallSpeed;

        // Reset pose progression and generate three starting poses.
        GenerateStartingPoses();

        // Choose a new starting wall color.
        randomColor = Random.Range(1, numColors + 1);
        prevColor = randomColor;
    }

    // ==================== SCORING & DIFFICULTY ====================

    // Awards points for a successful wall match and increases difficulty.
    // Called by PlayerScript after a successful collision.
    public void AddScore()
    {
        // Add the configured amount of points to the player's score.
        score += scoreAddAmt;

  
        // Count this match toward the next wall color change.
        wallCounter++;

        // Track total successful matches for pose progression.
        totalSuccessfulMatches++;

        // Check whether the player has reached a pose unlock or rotation.
        UpdatePoseProgression();

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
    // ==================== STARTING POSES ====================

    // Generates three starting poses: Idle and two unique random poses.
    private void GenerateStartingPoses()
    {
        // Reset the list of introduced poses for a new game.
        introducedPoses = new bool[currentNumPoses + 1];

        // Start with three available poses.
        activePoseCount = 3;
        poseStage = 1;
        totalSuccessfulMatches = 0;

        // Reset the full-pool rotation milestone for each new game.
        nextPoseRotationScore = 3300f;

        // Always place Idle in the first slot.
        randomWallNums[0] = 1;
        introducedPoses[1] = true;

        // Select two additional unique poses from the remaining pool.
        for (int i = 1; i < 3; i++)
        {
            int randomNum;

            // Keep selecting until we find a pose not yet introduced.
            do
            {
                randomNum = Random.Range(2, currentNumPoses + 1);
            }
            while (introducedPoses[randomNum]);

            // Store the selected pose and mark it as introduced.
            randomWallNums[i] = randomNum;
            introducedPoses[randomNum] = true;
        }

        // Clear the two slots that unlock later.
        randomWallNums[3] = 0;
        randomWallNums[4] = 0;

        // Notify other scripts that the available poses have changed.
        poseSetVersion++;
    }

    // ==================== FIRST POSE UNLOCK ====================

    // Adds two new poses after six successful wall matches.
    private void UnlockAdditionalPoses()
    {
        // Only unlock poses during Stage 1.
        if (poseStage != 1)
            return;

        // Fill the two previously locked pose slots.
        for (int i = 3; i < 5; i++)
        {
            int randomNum;

            // Select a pose that has not been introduced yet.
            do
            {
                randomNum = Random.Range(1, currentNumPoses + 1);
            }
            while (introducedPoses[randomNum]);

            // Add the new pose without replacing the original three.
            randomWallNums[i] = randomNum;
            introducedPoses[randomNum] = true;
        }

        // The player can now cycle through five poses.
        activePoseCount = 5;

        // Advance to Stage 2.
        poseStage = 2;

        // Notify other scripts that the available poses have changed.
        poseSetVersion++;
    }


    // ==================== NEW POSE ROTATION ====================

    // Replaces the current five poses with five previously unseen poses.
    private void RotateToNewPoses()
    {
        // Only rotate after the first five poses have been unlocked.
        if (poseStage < 2 || poseStage >= 5)
            return;

        // Fill all five slots with previously unseen poses.
        for (int i = 0; i < 5; i++)
        {
            int randomNum;

            // Select a pose that has not been introduced yet.
            do
            {
                randomNum = Random.Range(1, currentNumPoses + 1);
            }
            while (introducedPoses[randomNum]);

            // Replace the previous pose and mark the new one as introduced.
            randomWallNums[i] = randomNum;
            introducedPoses[randomNum] = true;
        }

        // Continue using five active poses.
        activePoseCount = 5;

        // Advance to the next progression stage.
        poseStage++;

        // Notify other scripts that the available poses have changed.
        poseSetVersion++;
    }

    // ==================== FULL POSE RANDOMIZATION ====================

    // Selects five unique poses from the entire pool of 20.
    // Used after all 20 poses have been introduced.
    private void RotateFromFullPool()
    {
        // Track which poses are selected for this rotation.
        bool[] selectedPoses = new bool[currentNumPoses + 1];

        // Randomly fill all five available pose slots.
        for (int i = 0; i < 5; i++)
        {
            int randomNum;

            // Select a pose that hasn't been chosen in this rotation.
            do
            {
                randomNum = Random.Range(1, currentNumPoses + 1);
            }
            while (selectedPoses[randomNum]);

            // Assign the pose and prevent duplicates in this set.
            randomWallNums[i] = randomNum;
            selectedPoses[randomNum] = true;
        }

        // Keep five poses available to the player.
        activePoseCount = 5;

        // Stage 6 represents full-pool randomization.
        poseStage = 6;

        // Notify other scripts that the available poses have changed.
        poseSetVersion++;
    }

    // ==================== POSE PROGRESSION CONTROLLER ====================

    // Checks whether the player has reached a new pose stage.
    private void UpdatePoseProgression()
    {
        // Stage 1 → 2: Unlock two additional poses after six matches.
        if (poseStage == 1 && totalSuccessfulMatches >= 6)
        {
            UnlockAdditionalPoses();
        }

        // Stage 2 → 3: Introduce five new poses at 1,500 points.
        if (poseStage == 2 && score >= 1500f)
        {
            RotateToNewPoses();
        }

        // Stage 3 → 4: Introduce five more poses at 2,100 points.
        if (poseStage == 3 && score >= 2100f)
        {
            RotateToNewPoses();
        }

        // Stage 4 → 5: Introduce the final five poses at 2,700 points.
        if (poseStage == 4 && score >= 2700f)
        {
            RotateToNewPoses();
        }

        // Stage 6+: Randomize from all 20 poses every 600 points.
        if (poseStage >= 5 && score >= nextPoseRotationScore)
        {
            RotateFromFullPool();

            // Schedule the next rotation.
            nextPoseRotationScore += 600f;
        }
    }

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