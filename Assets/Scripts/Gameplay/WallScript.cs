using UnityEngine;

// Controls each wall's pose, color, movement speed, and collision tag.
public class WallScript : MonoBehaviour
{
    // ==================== WALL POSES ====================

    // All available wall sprites.
    public Sprite[] walls;

    // The five wall sprites currently available during gameplay.
    public Sprite[] currentWalls = new Sprite[5];

    // Displays the selected wall sprite and its color.
    private SpriteRenderer spriteRenderer;

    // Index of the randomly selected wall pose (0–4).
    public int wallType;

    // Remembers which pose set this wall is currently displaying.
    private int lastPoseSetVersion;


    // ==================== WALL ANIMATION ====================

    // Controls the wall's movement animation and playback speed.
    private Animator anim;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Get the SpriteRenderer and Animator attached to this wall.
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Randomly select a wall pose from the currently unlocked poses.
        // Stage 1 uses 3 poses; Stage 2 onward uses 5.
        wallType = Random.Range(
            0,
            GameManager.instance.activePoseCount
        );

        // ==================== WALL SPRITE SELECTION ====================

        // Get the five available wall sprites selected by the GameManager.
        currentWalls[0] = walls[GameManager.instance.randomWallNums[0]];
        currentWalls[1] = walls[GameManager.instance.randomWallNums[1]];
        currentWalls[2] = walls[GameManager.instance.randomWallNums[2]];
        currentWalls[3] = walls[GameManager.instance.randomWallNums[3]];
        currentWalls[4] = walls[GameManager.instance.randomWallNums[4]];

        // Display the sprite corresponding to this wall's randomly selected pose.
        spriteRenderer.sprite = currentWalls[wallType];

        // Remember the pose set used when this wall spawned.
        lastPoseSetVersion = GameManager.instance.poseSetVersion;

        // ==================== WALL COLOR SELECTION ====================

        // Apply the current wall color selected by the GameManager.
        // All walls use the same color until the GameManager changes it.
        switch (GameManager.instance.randomColor)
        {
            case 1:
                // Punchy Pink
                spriteRenderer.color = new Color32(255, 92, 138, 0);
                break;

            case 2:
                // Game Show Yellow
                spriteRenderer.color = new Color32(255, 212, 59, 0);
                break;

            case 3:
                // Orange
                spriteRenderer.color = new Color32(255, 122, 48, 0);
                break;

            case 4:
                // Playful Green
                spriteRenderer.color = new Color32(82, 201, 122, 0);
                break;

            case 5:
                // Purple
                spriteRenderer.color = new Color32(155, 109, 227, 0);
                break;
        }
    }


    // ==================== WALL UPDATE ====================

    void Update()
    {
        // Check whether the available pose set has changed.
        if (lastPoseSetVersion != GameManager.instance.poseSetVersion)
        {
            // Refresh the wall sprites using the newly available poses.
            for (int i = 0; i < GameManager.instance.activePoseCount; i++)
            {
                currentWalls[i] = walls[GameManager.instance.randomWallNums[i]];
            }

            // Update this wall's appearance while preserving its pose slot.
            if (wallType < GameManager.instance.activePoseCount)
            {
                spriteRenderer.sprite = currentWalls[wallType];
            }

            // Remember the updated pose set.
            lastPoseSetVersion = GameManager.instance.poseSetVersion;
        }

        // Update the wall's animation speed using the GameManager's
        // current difficulty settings.
        anim.SetFloat("Speed", GameManager.instance.wallSpeed);


        // ==================== WALL COLLISION TAGS ====================

        // Assign the wall's collision tag based on its selected pose.
        // PlayerScript compares this tag against the player's current pose.
        switch (wallType)
        {
            case 0:
                gameObject.tag = "Pose_1";
                break;

            case 1:
                gameObject.tag = "Pose_2";
                break;

            case 2:
                gameObject.tag = "Pose_3";
                break;

            case 3:
                gameObject.tag = "Pose_4";
                break;

            case 4:
                gameObject.tag = "Pose_5";
                break;
        }
    }
}