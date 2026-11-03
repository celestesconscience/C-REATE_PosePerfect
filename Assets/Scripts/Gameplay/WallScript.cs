
using UnityEngine;

// Controls each wall's pose, color, animation speed,
// and collision tag. Synchronizes walls when poses change.
public class WallScript : MonoBehaviour
{
    // ==================== WALL POSES ====================

    // All available wall sprites, indexed by pose ID.
    public Sprite[] walls;

    // Stores up to five currently available wall sprites.
    // Only three slots are active at the beginning.
    public Sprite[] currentWalls = new Sprite[5];

    // Displays the wall's selected sprite and color.
    private SpriteRenderer spriteRenderer;

    // Index of this wall's selected pose slot (0–4).
    public int wallType;

    // Tracks the pose set currently displayed by this wall.
    private int lastPoseSetVersion;


    // ==================== WALL ANIMATION ====================

    // Controls wall movement and animation playback speed.
    private Animator anim;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Get the required components.
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Select a pose from the currently unlocked slots.
        // Stage 1: 3 poses. Stage 2 onward: 5 poses.
        wallType = Random.Range(
            0,
            GameManager.instance.activePoseCount
        );

        // Load the current pose selection.
        RefreshWallPoses();

        // Display this wall's selected pose.
        spriteRenderer.sprite = currentWalls[wallType];

        // Remember the pose set used at spawn.
        lastPoseSetVersion = GameManager.instance.poseSetVersion;

        // Assign the collision tag for this pose slot.
        // Valid tags: Pose_1 through Pose_5.
        gameObject.tag = "Pose_" + (wallType + 1);

        // Apply the current wall color.
        ApplyWallColor();
    }


    // ==================== WALL UPDATE ====================

    void Update()
    {
        // Detect changes to the available pose set.
        if (lastPoseSetVersion != GameManager.instance.poseSetVersion)
        {
            // Refresh the available sprites.
            RefreshWallPoses();

            // Update this wall's sprite without changing its pose slot.
            if (wallType < GameManager.instance.activePoseCount)
            {
                spriteRenderer.sprite = currentWalls[wallType];
            }

            // Record the new pose set version.
            lastPoseSetVersion = GameManager.instance.poseSetVersion;
        }

        // Update wall animation speed as difficulty increases.
        anim.SetFloat("Speed", GameManager.instance.wallSpeed);
    }


    // ==================== WALL SPRITE SELECTION ====================

    // Refreshes the available wall sprites from GameManager.
    // Called when a wall spawns or the pose set changes.
    private void RefreshWallPoses()
    {
        for (int i = 0; i < currentWalls.Length; i++)
        {
            // Only assign sprites to unlocked pose slots.
            if (i < GameManager.instance.activePoseCount)
            {
                int poseIndex = GameManager.instance.randomWallNums[i];

                // Make sure the pose ID exists in the sprite array.
                if (poseIndex > 0 && poseIndex < walls.Length)
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
                // Keep locked slots empty until they unlock.
                currentWalls[i] = null;
            }
        }
    }


    // ==================== WALL COLOR SELECTION ====================

    // Applies the current color selected by GameManager.
    // Alpha stays at 0 so the existing fade-in animation works.
    private void ApplyWallColor()
    {
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
}