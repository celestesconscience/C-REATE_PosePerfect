using UnityEngine;

// Manages scoring, pose progression, wall colors, difficulty,
// and shared gameplay settings across scenes.
public class GameManager : MonoBehaviour
{
    // ==================== GAME MANAGER INSTANCE ====================

    // Shared instance accessible by other gameplay scripts.
    public static GameManager instance;


    // ==================== SCORING SYSTEM ====================

    // Player's current score.
    public float score;

    // Points awarded for each successful wall match.
    public float scoreAddAmt;

    // Counts successful matches toward the next wall color change.
    private int wallCounter;

    // Stores high scores. Currently, index 0 tracks the highest score.
    public float[] highScore = new float[5];


    // ==================== POSE PROGRESSION ====================

    // Stores up to five pose IDs, used by the player, walls, and UI.
    // Only the first three slots are active at the start.
    public int[] randomWallNums = new int[5];

    // Total number of available poses (IDs 1–20).
    public int currentNumPoses = 20;

    // Number of currently unlocked pose slots (3 or 5).
    public int activePoseCount = 3;

    // Increases whenever the available pose selection changes.
    // Used by WallScript to synchronize existing walls.
    public int poseSetVersion { get; private set; } = 0;

    // Total successful matches during the current game.
    private int totalSuccessfulMatches = 0;

    // Current progression stage.
    private int poseStage = 1;

    // Tracks which pose IDs have already been introduced.
    private bool[] introducedPoses = new bool[21];

    // Next score milestone for full-pool randomization.
    private float nextPoseRotationScore = 3300f;


    // ==================== WALL COLORS ====================

    // Current color, previous color, and number of available colors.
    public int randomColor, prevColor, numColors;


    // ==================== WALL SPAWN SPEED ====================

    [Header("Wall Spawn Speed Settings")]

    // Current time between wall spawns.
    public float wallSpawnSpeed;

    // Minimum spawn interval.
    public float wallEndSpawnSpeed;

    // Amount the spawn interval decreases after each successful match.
    public float wallSpawnScale;

    // Original spawn interval, saved for resetting.
    private float startWallSpawn;


    // ==================== WALL MOVEMENT SPEED ====================

    [Header("Wall Speed Settings")]

    // Current wall movement speed.
    public float wallSpeed;

    // Maximum wall movement speed.
    public float wallEndSpeed;

    // Amount wall speed increases after each successful match.
    public float wallScale;

    // Original wall speed, saved for resetting.
    private float startWallSpeed;


    // ==================== INITIALIZATION ====================

    void Awake()
    {
        // Prevent duplicate GameManager instances.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Save starting difficulty before other scripts call ResetGame().
        startWallSpawn = wallSpawnSpeed;
        startWallSpeed = wallSpeed;

        // Preserve the GameManager between scene changes.
        DontDestroyOnLoad(gameObject);

        // Initialize Idle and two randomly selected poses.
        GenerateStartingPoses();
    }

    void Start()
    {
        // Select the initial wall color.
        randomColor = Random.Range(1, numColors + 1);
        prevColor = randomColor;
    }


    // ==================== GAME STATE UPDATE ====================

    void Update()
    {
        // Update the highest score.
        if (score > highScore[0])
        {
            highScore[0] = score;
        }

        // Restore starting difficulty when the score is zero.
        if (score == 0)
        {
            wallSpawnSpeed = startWallSpawn;
            wallSpeed = startWallSpeed;
        }

        // Change wall color every 10 successful matches.
        // Pose progression is handled separately.
        if (wallCounter >= 10)
        {
            if (numColors > 1)
            {
                // Prevent selecting the same color twice consecutively.
                do
                {
                    randomColor = Random.Range(1, numColors + 1);
                }
                while (randomColor == prevColor);

                prevColor = randomColor;
            }

            wallCounter = 0;
        }
    }


    // ==================== NEW GAME RESET ====================

    // Resets difficulty, wall colors, and pose progression.
    public void ResetGame()
    {
        // Reset the wall color counter.
        wallCounter = 0;

        // Restore starting difficulty.
        wallSpawnSpeed = startWallSpawn;
        wallSpeed = startWallSpeed;

        // Restart pose progression from Stage 1.
        GenerateStartingPoses();

        // Select a new starting wall color.
        randomColor = Random.Range(1, numColors + 1);
        prevColor = randomColor;
    }


    // ==================== SCORING & DIFFICULTY ====================

    // Called by PlayerScript after each successful wall match.
    public void AddScore()
    {
        // Award points.
        score += scoreAddAmt;

        // Count successful matches.
        wallCounter++;
        totalSuccessfulMatches++;

        // Check for pose unlocks and rotations.
        UpdatePoseProgression();

        // Gradually reduce the interval between wall spawns.
        if (wallSpawnSpeed > wallEndSpawnSpeed)
        {
            wallSpawnSpeed -= wallSpawnScale;
        }

        // Enforce the minimum spawn interval.
        if (wallSpawnSpeed < wallEndSpawnSpeed)
        {
            wallSpawnSpeed = wallEndSpawnSpeed;
        }

        // Gradually increase wall movement speed.
        if (wallSpeed < wallEndSpeed)
        {
            wallSpeed += wallScale;
        }

        // Enforce the maximum wall movement speed.
        if (wallSpeed > wallEndSpeed)
        {
            wallSpeed = wallEndSpeed;
        }
    }


    // ==================== STARTING POSES ====================

    // Starts a new game with Idle and two unique random poses.
    private void GenerateStartingPoses()
    {
        // Clear previously introduced poses.
        introducedPoses = new bool[currentNumPoses + 1];

        // Reset progression.
        activePoseCount = 3;
        poseStage = 1;
        totalSuccessfulMatches = 0;
        nextPoseRotationScore = 3300f;

        // Always place Idle in the first slot.
        randomWallNums[0] = 1;
        introducedPoses[1] = true;

        // Select two unique random poses from IDs 2–20.
        for (int i = 1; i < 3; i++)
        {
            int randomNum;

            do
            {
                randomNum = Random.Range(2, currentNumPoses + 1);
            }
            while (introducedPoses[randomNum]);

            randomWallNums[i] = randomNum;
            introducedPoses[randomNum] = true;
        }

        // Keep the last two pose slots locked.
        randomWallNums[3] = 0;
        randomWallNums[4] = 0;

        // Notify other scripts that the pose set changed.
        poseSetVersion++;
    }


    // ==================== FIRST POSE UNLOCK ====================

    // Unlocks two additional poses after six successful matches.
    private void UnlockAdditionalPoses()
    {
        // Only unlock during Stage 1.
        if (poseStage != 1)
            return;

        // Fill slots 4 and 5 with previously unseen poses.
        for (int i = 3; i < 5; i++)
        {
            int randomNum;

            do
            {
                randomNum = Random.Range(1, currentNumPoses + 1);
            }
            while (introducedPoses[randomNum]);

            randomWallNums[i] = randomNum;
            introducedPoses[randomNum] = true;
        }

        // Activate all five slots.
        activePoseCount = 5;
        poseStage = 2;

        // Notify other scripts.
        poseSetVersion++;
    }


    // ==================== NEW POSE ROTATION ====================

    // Replaces all five slots with previously unseen poses.
    // Used at 1,500, 2,100, and 2,700 points.
    private void RotateToNewPoses()
    {
        // Only introduce unseen poses during Stages 2–4.
        if (poseStage < 2 || poseStage >= 5)
            return;

        // Select five previously unseen poses.
        for (int i = 0; i < 5; i++)
        {
            int randomNum;

            do
            {
                randomNum = Random.Range(1, currentNumPoses + 1);
            }
            while (introducedPoses[randomNum]);

            randomWallNums[i] = randomNum;
            introducedPoses[randomNum] = true;
        }

        activePoseCount = 5;

        // Advance to the next stage.
        poseStage++;

        // Notify other scripts.
        poseSetVersion++;
    }


    // ==================== FULL-POOL RANDOMIZATION ====================

    // Selects five unique poses from all 20 available poses.
    // Used at 3,300 points and every 600 points afterward.
    private void RotateFromFullPool()
    {
        // Track selections within this rotation to prevent duplicates.
        bool[] selectedPoses = new bool[currentNumPoses + 1];

        for (int i = 0; i < 5; i++)
        {
            int randomNum;

            do
            {
                randomNum = Random.Range(1, currentNumPoses + 1);
            }
            while (selectedPoses[randomNum]);

            randomWallNums[i] = randomNum;
            selectedPoses[randomNum] = true;
        }

        activePoseCount = 5;

        // Stage 6 represents unrestricted randomization.
        poseStage = 6;

        // Notify other scripts.
        poseSetVersion++;
    }


    // ==================== POSE PROGRESSION CONTROLLER ====================

    // Activates pose changes when progression milestones are reached.
    private void UpdatePoseProgression()
    {
        // Stage 1 → 2: Unlock two poses after six successful matches.
        if (poseStage == 1 && totalSuccessfulMatches >= 6)
        {
            UnlockAdditionalPoses();
        }

        // Stage 2 → 3: Introduce five unseen poses.
        if (poseStage == 2 && score >= 1500f)
        {
            RotateToNewPoses();
        }

        // Stage 3 → 4: Introduce five more unseen poses.
        if (poseStage == 3 && score >= 2100f)
        {
            RotateToNewPoses();
        }

        // Stage 4 → 5: Introduce the final five unseen poses.
        if (poseStage == 4 && score >= 2700f)
        {
            RotateToNewPoses();
        }

        // Stage 6+: Randomize from all 20 poses every 600 points.
        if (poseStage >= 5 && score >= nextPoseRotationScore)
        {
            RotateFromFullPool();

            // Schedule the next full-pool rotation.
            nextPoseRotationScore += 600f;
        }
    }
}
