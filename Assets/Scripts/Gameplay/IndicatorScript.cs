using UnityEngine;
using UnityEngine.InputSystem;

// Controls the pose selection indicator displayed above the player's available poses.
public class IndicatorScript : MonoBehaviour
{
    // ==================== PLAYER INPUT ====================

    // Input action used to cycle between poses.
    InputAction buttonPress;

    // Tracks whether the tap action was performed during the current frame.
    private bool pressed;


    // ==================== INDICATOR SETTINGS ====================

    // Vertical offset for positioning the indicator above each pose.
    // Use a positive value for an arrow above the pose, or 0 for a circle centered on it.
    public int arrowAbove;


    // ==================== INDICATOR POSITIONS ====================

    // References to the five pose positions, assigned in the Inspector.
    public GameObject[] Location;

    // Index of the currently selected pose indicator (starts at 0).
    private int startingPose = 0;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Find the tap input action configured in Unity's Input System.
        buttonPress = InputSystem.actions.FindAction("TapButton");

        // Position the indicator at the first pose when the game starts.
        transform.localPosition = new Vector2(
            Location[startingPose].transform.localPosition.x,
            Location[startingPose].transform.localPosition.y + arrowAbove
        );
    }


    // ==================== INDICATOR UPDATE ====================

    void Update()
    {
        // Read player input only while the game is not paused.
        if (Time.timeScale != 0)
        {
            pressed = buttonPress.WasPerformedThisFrame();
        }

        // Move the indicator when the player cycles to the next pose.
        if (pressed == true)
        {
            // Advance to the next pose index.
            startingPose++;
            
            // Get the number of poses currently unlocked.
            int availablePoses = Mathf.Clamp(
                GameManager.instance.activePoseCount,
                1,
                Location.Length
            );

            // Return to the first pose after the last unlocked pose.
            if (startingPose >= availablePoses)
            {
                startingPose = 0;
            }

            // Update the indicator's position to match the selected pose.
            // Apply the vertical offset to keep it above or centered on the pose.
            transform.localPosition = new Vector2(
                Location[startingPose].transform.localPosition.x,
                Location[startingPose].transform.localPosition.y + arrowAbove
            );
        }
    }
}