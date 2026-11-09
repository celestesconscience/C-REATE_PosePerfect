using UnityEngine;

// Controls wall poses, colors, movement speed,
// collision tags, and the Pose Magnet mechanic.
public class WallScript : MonoBehaviour
{
    // ==================== WALL POSES ====================

    public Sprite[] walls;
    public Sprite[] currentWalls = new Sprite[5];

    private SpriteRenderer spriteRenderer;

    public int wallType;
    private int lastPoseSetVersion;


    // ==================== WALL ANIMATION ====================

    private Animator anim;


    // ==================== POSE MAGNET SETTINGS ====================

    [Header("Pose Magnet Settings")]

    // Enable or disable the magnet.
    public bool magnetEnabled = true;

    // Maximum magnet multiplier at the start of the game.
    public float magnetMultiplier = 1.5f;

    // Time the correct pose AND lane must be maintained.
    public float magnetHoldTime = 0.6f;

    // How quickly the magnet speeds up and slows down.
    public float magnetAcceleration = 8f;


    // ==================== MAGNET SAFETY SETTINGS ====================

    [Header("Magnet Safety Settings")]

    // Stop magnet acceleration during the final
    // 35% of the wall's approach.
    [Range(0f, 1f)]
    public float magnetCutoffFraction = 0.35f;

    // Starting wall speed from GameManager's Inspector.
    // Magnet is strongest at this movement speed.
    public float magnetStartWallSpeed = 0.6f;

    // Magnet reaches 1x when GameManager.wallSpeed
    // reaches GameManager.wallEndSpeed.


    // ==================== MAGNET TRACKING ====================

    private PlayerScript player;

    private float matchTimer = 0f;
    private float currentMagnetMultiplier = 1f;

    // Distance between the wall and player at spawn.
    private float initialApproachDistance;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        player = FindAnyObjectByType<PlayerScript>();

        // Record the wall's starting distance.
        if (player != null)
        {
            initialApproachDistance = Mathf.Abs(
                transform.position.y -
                player.transform.position.y
            );
        }

        // Select a pose from the currently unlocked slots.
        wallType = Random.Range(
            0,
            GameManager.instance.activePoseCount
        );

        RefreshWallPoses();

        spriteRenderer.sprite = currentWalls[wallType];

        lastPoseSetVersion =
            GameManager.instance.poseSetVersion;

        gameObject.tag = "Pose_" + (wallType + 1);

        ApplyWallColor();
    }


    // ==================== WALL UPDATE ====================

    void Update()
    {
        if (GameManager.instance == null)
            return;

        // Refresh the wall when the available poses change.
        if (lastPoseSetVersion !=
            GameManager.instance.poseSetVersion)
        {
            RefreshWallPoses();

            if (wallType <
                GameManager.instance.activePoseCount)
            {
                spriteRenderer.sprite =
                    currentWalls[wallType];
            }

            lastPoseSetVersion =
                GameManager.instance.poseSetVersion;
        }

        UpdateMagnetSpeed();
    }


    // ==================== POSE MAGNET ====================

    private void UpdateMagnetSpeed()
    {
        // Check if the player's pose matches this wall.
        bool correctPose =
            player != null &&
            player.gameObject.tag == gameObject.tag;

        // Check if the player is in the correct lane.
        bool correctLane =
            player != null &&
            Mathf.Abs(
                player.transform.position.x -
                transform.position.x
            ) < 0.1f;


        // ==================== DISTANCE CUTOFF ====================

        bool wallIsFarEnough = true;

        if (player != null &&
            initialApproachDistance > 0.01f)
        {
            float remainingDistance = Mathf.Abs(
                transform.position.y -
                player.transform.position.y
            );

            float cutoffDistance =
                initialApproachDistance *
                magnetCutoffFraction;

            wallIsFarEnough =
                remainingDistance > cutoffDistance;
        }


        // ==================== DIFFICULTY SCALING ====================

        // Read current wall movement speed.
        float currentWallSpeed =
            GameManager.instance.wallSpeed;

        // Read maximum wall movement speed.
        float maximumWallSpeed =
            GameManager.instance.wallEndSpeed;

        // Calculate difficulty using actual movement speed.
        // 0 = starting speed.
        // 1 = maximum speed.
        float difficultyProgress = Mathf.InverseLerp(
            magnetStartWallSpeed,
            maximumWallSpeed,
            currentWallSpeed
        );

        // Gradually reduce the magnet from its maximum
        // multiplier down to normal movement speed (1x).
        float adjustedMagnetMultiplier = Mathf.Lerp(
            Mathf.Max(1f, magnetMultiplier),
            1f,
            difficultyProgress
        );


        // ==================== MAGNET ACTIVATION ====================

        bool isMatching =
            magnetEnabled &&
            correctPose &&
            correctLane &&
            wallIsFarEnough &&
            adjustedMagnetMultiplier > 1.001f;

        if (isMatching)
        {
            matchTimer += Time.deltaTime;
        }
        else
        {
            matchTimer = 0f;
        }

        // Normal speed unless the match has been held.
        float targetMultiplier = 1f;

        if (isMatching &&
            matchTimer >= magnetHoldTime)
        {
            targetMultiplier = adjustedMagnetMultiplier;
        }


        // ==================== SMOOTH SPEED CHANGE ====================

        currentMagnetMultiplier = Mathf.MoveTowards(
            currentMagnetMultiplier,
            targetMultiplier,
            magnetAcceleration * Time.deltaTime
        );

        float finalWallSpeed =
            currentWallSpeed * currentMagnetMultiplier;

        anim.SetFloat("Speed", finalWallSpeed);
    }


    // ==================== WALL SPRITE SELECTION ====================

    private void RefreshWallPoses()
    {
        for (int i = 0; i < currentWalls.Length; i++)
        {
            if (i < GameManager.instance.activePoseCount)
            {
                int poseIndex =
                    GameManager.instance.randomWallNums[i];

                if (poseIndex > 0 &&
                    poseIndex < walls.Length)
                {
                    currentWalls[i] = walls[poseIndex];
                }
                else
                {
                    currentWalls[i] = null;
                }
            }
            else
            {
                currentWalls[i] = null;
            }
        }
    }


    // ==================== WALL COLOR SELECTION ====================

    private void ApplyWallColor()
    {
        switch (GameManager.instance.randomColor)
        {
            case 1:
                spriteRenderer.color =
                    new Color32(255, 92, 138, 0);
                break;

            case 2:
                spriteRenderer.color =
                    new Color32(255, 212, 59, 0);
                break;

            case 3:
                spriteRenderer.color =
                    new Color32(255, 122, 48, 0);
                break;

            case 4:
                spriteRenderer.color =
                    new Color32(82, 201, 122, 0);
                break;

            case 5:
                spriteRenderer.color =
                    new Color32(155, 109, 227, 0);
                break;
        }
    }
}
