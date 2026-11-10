using System.Collections;
using UnityEngine;

// Manages scoring, pose progression, wall colors, difficulty,
// and shared gameplay settings across scenes.
public class GameManager : MonoBehaviour
{
    // ==================== GAME MANAGER INSTANCE ====================

    public static GameManager instance;


    // ==================== SCORING SYSTEM ====================

    public float score;
    public float scoreAddAmt;

    private int wallCounter;

    public float[] highScore = new float[5];
    public float livesLeft;


    // ==================== POSE PROGRESSION ====================

    public int[] randomWallNums = new int[5];

    public int currentNumPoses = 20;
    public int activePoseCount = 3;

    public int poseSetVersion { get; private set; } = 0;

    private int totalSuccessfulMatches = 0;
    private int poseStage = 1;

    private bool[] introducedPoses = new bool[21];

    private float nextPoseRotationScore = 3300f;


    // ==================== WALL COLORS ====================

    [Header("Color Settings")]
    public int randomColor, prevColor, numColors;


    // ==================== WALL / POWER-UP SPAWN ====================

    [Header("Wall and PowerUp Settings")]

    public int wallLocation;
    public int powerUpLocation;
    public int numPowerUps;


    // ==================== POWER-UPS ====================

    [Header("Power Up Settings")]

    public int doublePointsTimeStart = 20;
    public int doublePointsTimeLeft;
    public bool doublePoints;
    public bool doublePointsAlreadyOn;

    public int armorTimeStart = 30;
    public int armorTimeLeft;
    public bool armor;
    public bool armorAlreadyOn;


    // ==================== WALL SPAWN SPEED ====================

    [Header("Wall Spawn Speed Settings")]

    public float wallSpawnSpeed;
    public float wallEndSpawnSpeed;
    public float wallSpawnScale;

    private float startWallSpawn;


    // ==================== WALL MOVEMENT SPEED ====================

    [Header("Wall Speed Settings")]

    public float wallSpeed;
    public float wallEndSpeed;
    public float wallScale;

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

        // Preserve GameManager between scene changes.
        DontDestroyOnLoad(gameObject);

        // Initialize the starting pose selection.
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

        // Restore starting difficulty when score is zero.
        if (score == 0)
        {
            wallSpawnSpeed = startWallSpawn;
            wallSpeed = startWallSpeed;
        }

        // Change wall color every 10 successful matches.
        if (wallCounter >= 10)
        {
            if (numColors > 1)
            {
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

    public void ResetGame()
    {
        // Stop old power-up timers before resetting values.
        StopCoroutine("activateArmor");
        StopCoroutine("activateDoublePoints");

        // Restore the player's starting heart after restarting.
        livesLeft = 1;

        // Clear all armor effects.
        armorAlreadyOn = false;
        armor = false;
        armorTimeLeft = 0;

        // Clear all double-points effects.
        doublePointsAlreadyOn = false;
        doublePoints = false;
        doublePointsTimeLeft = 0;

        // Reset wall color counter.
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

    public void AddScore()
    {
        // Apply double points when active.
        if (doublePoints)
            score += scoreAddAmt * 2;
        else
            score += scoreAddAmt;

        // Count successful matches.
        wallCounter++;
        totalSuccessfulMatches++;

        // Check for pose progression.
        UpdatePoseProgression();

        // Increase wall spawn frequency.
        if (wallSpawnSpeed > wallEndSpawnSpeed)
        {
            wallSpawnSpeed -= wallSpawnScale;
        }

        if (wallSpawnSpeed < wallEndSpawnSpeed)
        {
            wallSpawnSpeed = wallEndSpawnSpeed;
        }

        // Increase wall movement speed.
        if (wallSpeed < wallEndSpeed)
        {
            wallSpeed += wallScale;
        }

        if (wallSpeed > wallEndSpeed)
        {
            wallSpeed = wallEndSpeed;
        }
    }


    // ==================== STARTING POSES ====================

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

        // Select two unique random poses.
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

        // Keep final two slots locked.
        randomWallNums[3] = 0;
        randomWallNums[4] = 0;

        poseSetVersion++;
    }


    // ==================== FIRST POSE UNLOCK ====================

    private void UnlockAdditionalPoses()
    {
        if (poseStage != 1)
            return;

        // Unlock two previously unseen poses.
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

        activePoseCount = 5;
        poseStage = 2;

        poseSetVersion++;
    }


    // ==================== NEW POSE ROTATION ====================

    private void RotateToNewPoses()
    {
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

        poseStage++;

        poseSetVersion++;
    }


    // ==================== FULL-POOL RANDOMIZATION ====================

    private void RotateFromFullPool()
    {
        // Prevent duplicate poses within the same rotation.
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
        poseStage = 6;

        poseSetVersion++;
    }


    // ==================== POSE PROGRESSION CONTROLLER ====================

    private void UpdatePoseProgression()
    {
        // Stage 1: Unlock five poses after six matches.
        if (poseStage == 1 && totalSuccessfulMatches >= 6)
        {
            UnlockAdditionalPoses();
        }

        // Stage 2: Rotate at 1500 points.
        if (poseStage == 2 && score >= 1500f)
        {
            RotateToNewPoses();
        }

        // Stage 3: Rotate at 2100 points.
        if (poseStage == 3 && score >= 2100f)
        {
            RotateToNewPoses();
        }

        // Stage 4: Rotate at 2700 points.
        if (poseStage == 4 && score >= 2700f)
        {
            RotateToNewPoses();
        }

        // Stage 5+: Rotate every 600 points from 3300.
        if (poseStage >= 5 && score >= nextPoseRotationScore)
        {
            RotateFromFullPool();
            nextPoseRotationScore += 600f;
        }
    }


    // ==================== WALL LOCATION CONTROLLER ====================

    public void randomWallLocation()
    {
        wallLocation = Random.Range(0, 3);

        // Prevent wall and power-up occupying the same lane.
        while (wallLocation == powerUpLocation)
        {
            wallLocation = Random.Range(0, 3);
        }
    }


    // ==================== POWER-UP LOCATION CONTROLLER ====================

    public void randomPowerUpLocation()
    {
        powerUpLocation = Random.Range(0, 3);

        // Prevent power-up and wall occupying the same lane.
        while (powerUpLocation == wallLocation)
        {
            powerUpLocation = Random.Range(0, 3);
        }
    }


    // ==================== DOUBLE POINTS TIMER ====================

    public IEnumerator activateDoublePoints()
    {
        // Activate double points immediately.
        doublePointsAlreadyOn = true;

        // Count down while the effect is active.
        while (doublePointsTimeLeft > 0 && doublePoints)
        {
            yield return new WaitForSeconds(1f);

            // Stop if double points were cancelled.
            if (!doublePoints)
                yield break;

            doublePointsTimeLeft--;
        }

        // Disable the multiplier when time expires.
        doublePointsTimeLeft = 0;
        doublePointsAlreadyOn = false;
        doublePoints = false;
    }


    // ==================== ARMOR TIMER ====================

    public IEnumerator activateArmor()
    {
        // Activate protection immediately.
        armorAlreadyOn = true;

        // Count down while armor remains active.
        while (armorTimeLeft > 0 && armor)
        {
            yield return new WaitForSeconds(1f);

            // Stop if armor was consumed or cancelled.
            if (!armor)
                yield break;

            armorTimeLeft--;
        }

        // Remove protection when time expires.
        armorTimeLeft = 0;
        armorAlreadyOn = false;
        armor = false;
    }
}
