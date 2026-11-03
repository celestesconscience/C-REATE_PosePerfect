using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

// Controls player poses, lane movement, wall collisions, and gameplay feedback.
public class PlayerScript : MonoBehaviour
{
    // ==================== MENU MANAGER ====================

    // Reference to the MenuManager for displaying the Game Over screen.
    public MenuManager menu;

    // ==================== PLAYER INPUT ====================

    // Input actions for changing poses and moving between lanes.
    InputAction buttonPress, buttonHold;

    // Tracks whether each input action was performed during the current frame.
    private bool pressed, held;

    // ==================== PLAYER POSES ====================

    // All available character pose sprites.
    public Sprite[] poses;

    // The five poses currently available during gameplay.
    public Sprite[] currentPoses = new Sprite[5];

    // Controls which sprite is displayed on the player.
    private SpriteRenderer spriteRenderer;

    // Index of the player's selected pose (0–4).
    private int startingPose = 0;

    // ==================== PLAYER MOVEMENT ====================

    // X positions of the three lanes, configured in the Inspector.
    public float[] locations = new float[3];

    // Current lane index: 0 = left, 1 = middle, 2 = right.
    public int currentLocation;

    // Direction of lane movement: 1 = right, -1 = left.
    private int changeAMT = 1;


    // ==================== SOUND EFFECTS ====================

    public AudioSource movementAudio;
    public AudioSource poseChangeAudio;
    public AudioSource successSparkleAudio;
    public AudioSource failSmokeAudio;


    // ==================== COLLISION EFFECTS ====================

    // Visual effects displayed after successful or failed wall matches.
    public GameObject winObject, failObject;


    // ==================== ENCOURAGEMENT SYSTEM ====================

    // Reference to the script responsible for playing encouragement animations.
    public EncouragementTextScript encouragementText;

    // Tracks total successful matches, regardless of consecutive streaks.
    private int successfulMatches = 0;


    // ==================== COMBO / STREAK SYSTEM ====================

    // Tracks consecutive successful matches. Resets when a wall match fails.
    private int comboCount = 0;


    // ==================== COMBO UI ====================

    // Reference to the TextMeshPro element displaying the current combo.
    public TextMeshProUGUI comboText;

    // Reference to the matching outline text.
    public TextMeshProUGUI comboOutline;

    // Animator responsible for the combo pop and idle animations.
    public Animator comboAnimator;


    // ==================== COMBO DISPLAY ====================

    // Updates the persistent combo counter and replays its pop animation.
    void ShowCombo()
    {
        // Only display combos of 2 or higher.
        if (comboCount < 2)
            return;

        // Update both text layers with the current combo.
        string comboMessage = comboCount + "X COMBO!";

        comboText.text = comboMessage;
        comboOutline.text = comboMessage;

        // Keep the combo visible while the streak continues.
        comboText.gameObject.SetActive(true);
        comboOutline.gameObject.SetActive(true);

        // Replay the pop animation whenever the combo increases.
        comboAnimator.Play("ComboPop", 0, 0f);
    }


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Reset the score at the beginning of a new game.
        GameManager.instance.score = 0;

        // Reset wall progression, colors, poses, and difficulty.
        GameManager.instance.ResetGame();
        
        // Find the input actions configured in Unity's Input System.
        buttonPress = InputSystem.actions.FindAction("TapButton");
        buttonHold = InputSystem.actions.FindAction("HoldButton");

        // Get the player's SpriteRenderer and assign the initial pose.
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = poses[GameManager.instance.randomWallNums[0]];

        // Set the player's starting pose tag.
        gameObject.tag = "Pose_1";

        // Move the player to their starting lane.
        transform.position = new Vector2(
            locations[currentLocation],
            transform.position.y
        );
    }


    // ==================== PLAYER UPDATE ====================

    void Update()
    {
        // Read player input only while the game is not paused.
        if (Time.timeScale != 0)
        {
            pressed = buttonPress.WasPerformedThisFrame();
            held = buttonHold.WasPerformedThisFrame();
        }

        // Update the five available poses using the GameManager's selection.
        currentPoses[0] = poses[GameManager.instance.randomWallNums[0]];
        currentPoses[1] = poses[GameManager.instance.randomWallNums[1]];
        currentPoses[2] = poses[GameManager.instance.randomWallNums[2]];
        currentPoses[3] = poses[GameManager.instance.randomWallNums[3]];
        currentPoses[4] = poses[GameManager.instance.randomWallNums[4]];

        // Display the sprite corresponding to the currently selected pose.
        spriteRenderer.sprite = currentPoses[startingPose];

        // Cycle to the next pose when the tap action is performed.
        if (pressed == true)
        {
            poseChange();
        }

        // Move to the next lane when the hold action is performed.
        if (held == true)
        {
            laneMovement();
        }
    }


    // ==================== WALL COLLISIONS ====================

    // Checks whether the player matches the approaching wall's pose and lane.
    void OnTriggerEnter2D(Collider2D other)
    {
        // A successful match requires the correct pose tag and X position.
        if (other.CompareTag(gameObject.tag) &&
            (other.transform.position.x == gameObject.transform.position.x))
        {
            // Display the successful match effect.
            winObject.SetActive(true);

            // Play the success sound effect if assigned.
            if (successSparkleAudio != null)
            {
                successSparkleAudio.Play();
            }

            // Award points and update difficulty through the GameManager.
            GameManager.instance.AddScore();

            // Count this match toward total successes and the current streak.
            successfulMatches++;
            comboCount++;

            // Temporarily display the current combo in the Unity Console.
            Debug.Log("Current Combo: " + comboCount);

            ShowCombo();

            // Play a random encouragement animation every five successes.
            if (successfulMatches % 5 == 0)
            {
                encouragementText.PlayRandomEncouragement();
            }
        }
        else
        {
            // A failed match breaks the current combo streak.
            comboCount = 0;

            // Hide both combo text layers.
            comboText.gameObject.SetActive(false);
            comboOutline.gameObject.SetActive(false);

            // Hide the player sprite and display the failure effect.
            spriteRenderer.color = new Color32(255, 255, 255, 0);
            failObject.SetActive(true);

            // Play the failure sound effect if assigned.
            if (failSmokeAudio != null)
            {
                failSmokeAudio.Play();
            }

            // Delay Game Over so the failure animation can finish.
            StartCoroutine(DelayedGameOver());
        }
    }


    // ==================== GAME OVER ====================

    // Waits before opening Game Over to allow the failure effect to play.
    IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(1.5f);

        menu.GameOver_Open_UI();
    }


    // ==================== COLLISION CLEANUP ====================

    // Hide the success effect after the wall leaves the player's collider.
    void OnTriggerExit2D(Collider2D other)
    {
        winObject.SetActive(false);
    }


    // ==================== POSE CHANGING ====================

    // Cycles through the five available poses and updates the player's tag.
    private void poseChange()
    {
        // Advance to the next pose index.
        startingPose++;

        // Get the number of poses currently unlocked.
        int availablePoses = Mathf.Clamp(
            GameManager.instance.activePoseCount,
            1,
            currentPoses.Length
        );

        // Return to the first pose after the last unlocked pose.
        if (startingPose >= availablePoses)
        {
            startingPose = 0;
        }


        // Play the pose-changing sound effect if assigned.
        if (poseChangeAudio != null)
        {
            poseChangeAudio.Play();
        }

        // Update the player's tag for matching against incoming wall tags.
        switch (startingPose)
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


    // ==================== LANE MOVEMENT ====================

    // Moves the player between three lanes, reversing direction at the edges.
    private void laneMovement()
    {
        // Move one lane in the current direction.
        currentLocation += changeAMT;

        // Reverse direction when moving beyond the rightmost lane.
        if (currentLocation == locations.Length)
        {
            changeAMT = -changeAMT;

            // Move back into the valid lane range.
            currentLocation += changeAMT;
            currentLocation += changeAMT;
        }

        // Reverse direction when moving beyond the leftmost lane.
        else if (currentLocation == -1)
        {
            changeAMT = -changeAMT;

            // Move back into the valid lane range.
            currentLocation += changeAMT;
            currentLocation += changeAMT;
        }

        // Update the player's X position to match the selected lane.
        transform.position = new Vector2(
            locations[currentLocation],
            transform.position.y
        );

        // Play the movement sound effect if assigned.
        if (movementAudio != null)
        {
            movementAudio.Play();
        }
    }
}
