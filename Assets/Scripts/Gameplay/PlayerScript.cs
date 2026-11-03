
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

// Controls player poses, lane movement, wall collisions,
// scoring feedback, combos, and gameplay effects.
public class PlayerScript : MonoBehaviour
{
    // ==================== MENU MANAGER ====================

    // Opens the Game Over screen after a failed wall match.
    public MenuManager menu;


    // ==================== PLAYER INPUT ====================

    // Input actions for changing poses and moving between lanes.
    private InputAction buttonPress;
    private InputAction buttonHold;


    // ==================== PLAYER POSES ====================

    // All character pose sprites, indexed by pose ID.
    public Sprite[] poses;

    // Stores the five possible pose slots.
    // Only three slots are unlocked at the beginning.
    public Sprite[] currentPoses = new Sprite[5];

    // Displays the currently selected player pose.
    private SpriteRenderer spriteRenderer;

    // Index of the selected pose slot (0–4).
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

    // Visual effects displayed after successful or failed matches.
    public GameObject winObject;
    public GameObject failObject;


    // ==================== ENCOURAGEMENT SYSTEM ====================

    // Plays encouragement animations every five successful matches.
    public EncouragementTextScript encouragementText;

    // Total successful matches during the current game.
    private int successfulMatches = 0;


    // ==================== COMBO SYSTEM ====================

    // Consecutive successful matches.
    // Resets after a failed wall match.
    private int comboCount = 0;

    // Combo text and its matching outline.
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI comboOutline;

    // Plays the combo pop and idle animations.
    public Animator comboAnimator;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Reset the score and gameplay progression.
        GameManager.instance.score = 0;
        GameManager.instance.ResetGame();

        // Find the configured Input System actions.
        buttonPress = InputSystem.actions.FindAction("TapButton");
        buttonHold = InputSystem.actions.FindAction("HoldButton");

        // Get the player SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start with the first available pose (Idle).
        startingPose = 0;
        spriteRenderer.sprite =
            poses[GameManager.instance.randomWallNums[0]];

        // Assign the matching collision tag.
        gameObject.tag = "Pose_1";

        // Position the player in the configured starting lane.
        transform.position = new Vector2(
            locations[currentLocation],
            transform.position.y
        );
    }


    // ==================== PLAYER UPDATE ====================

    void Update()
    {
        // Refresh the available poses using GameManager's selection.
        // Locked slots remain empty until they are unlocked.
        for (int i = 0; i < currentPoses.Length; i++)
        {
            int poseIndex = GameManager.instance.randomWallNums[i];

            if (poseIndex > 0 && poseIndex < poses.Length)
            {
                currentPoses[i] = poses[poseIndex];
            }
            else
            {
                currentPoses[i] = null;
            }
        }

        // Display the player's currently selected pose.
        spriteRenderer.sprite = currentPoses[startingPose];

        // Ignore gameplay input while paused.
        if (Time.timeScale == 0)
            return;

        // Tap to cycle through unlocked poses.
        if (buttonPress.WasPerformedThisFrame())
        {
            poseChange();
        }

        // Hold action to change lanes.
        if (buttonHold.WasPerformedThisFrame())
        {
            laneMovement();
        }
    }


    // ==================== WALL COLLISIONS ====================

    // Checks whether the player matches an incoming wall's
    // required pose and lane.
    void OnTriggerEnter2D(Collider2D other)
    {
        // A successful match requires the correct pose and X position.
        if (other.CompareTag(gameObject.tag) &&
            other.transform.position.x == transform.position.x)
        {
            // Show the success effect.
            winObject.SetActive(true);

            // Play the success sound.
            if (successSparkleAudio != null)
            {
                successSparkleAudio.Play();
            }

            // Award points and update progression and difficulty.
            GameManager.instance.AddScore();

            // Update successful match and combo counters.
            successfulMatches++;
            comboCount++;

            // Update the combo display.
            ShowCombo();

            // Play encouragement every five successful matches.
            if (successfulMatches % 5 == 0)
            {
                encouragementText.PlayRandomEncouragement();
            }
        }
        else
        {
            // Reset the combo after a failed match.
            comboCount = 0;

            // Hide both combo text layers.
            comboText.gameObject.SetActive(false);
            comboOutline.gameObject.SetActive(false);

            // Hide the player and display the failure effect.
            spriteRenderer.color = new Color32(255, 255, 255, 0);
            failObject.SetActive(true);

            // Play the failure sound.
            if (failSmokeAudio != null)
            {
                failSmokeAudio.Play();
            }

            // Delay Game Over until the failure effect finishes.
            StartCoroutine(DelayedGameOver());
        }
    }


    // ==================== COMBO DISPLAY ====================

    // Updates the combo text and replays its pop animation.
    private void ShowCombo()
    {
        // Only display combos of two or higher.
        if (comboCount < 2)
            return;

        string comboMessage = comboCount + "X COMBO!";

        // Update both text layers.
        comboText.text = comboMessage;
        comboOutline.text = comboMessage;

        // Keep the combo visible.
        comboText.gameObject.SetActive(true);
        comboOutline.gameObject.SetActive(true);

        // Replay the combo pop animation.
        comboAnimator.Play("ComboPop", 0, 0f);
    }


    // ==================== GAME OVER ====================

    // Waits for the failure effect before opening Game Over.
    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(1.5f);

        menu.GameOver_Open_UI();
    }


    // ==================== COLLISION CLEANUP ====================

    // Hides the success effect when the wall exits the collider.
    void OnTriggerExit2D(Collider2D other)
    {
        winObject.SetActive(false);
    }


    // ==================== POSE CHANGING ====================

    // Cycles through the currently unlocked poses.
    private void poseChange()
    {
        // Move to the next pose slot.
        startingPose++;

        // Get the number of unlocked poses (3 or 5).
        int availablePoses = Mathf.Clamp(
            GameManager.instance.activePoseCount,
            1,
            currentPoses.Length
        );

        // Wrap back to the first pose when reaching the end.
        if (startingPose >= availablePoses)
        {
            startingPose = 0;
        }

        // Play the pose change sound.
        if (poseChangeAudio != null)
        {
            poseChangeAudio.Play();
        }

        // Update the player's collision tag.
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

    // Moves between three lanes, reversing direction at the edges.
    private void laneMovement()
    {
        // Advance one lane in the current direction.
        currentLocation += changeAMT;

        // Reverse direction at the right edge.
        if (currentLocation == locations.Length)
        {
            changeAMT = -changeAMT;

            // Move back into the valid lane range.
            currentLocation += changeAMT;
            currentLocation += changeAMT;
        }

        // Reverse direction at the left edge.
        else if (currentLocation == -1)
        {
            changeAMT = -changeAMT;

            // Move back into the valid lane range.
            currentLocation += changeAMT;
            currentLocation += changeAMT;
        }

        // Move the player to the selected lane.
        transform.position = new Vector2(
            locations[currentLocation],
            transform.position.y
        );

        // Play the lane movement sound.
        if (movementAudio != null)
        {
            movementAudio.Play();
        }
    }
}
