
using UnityEngine;
using UnityEngine.InputSystem;

// Controls the pose selection indicator in the gameplay UI.
// Keeps the indicator aligned with the player's selected pose.
public class IndicatorScript : MonoBehaviour
{
    // ==================== PLAYER INPUT ====================

    // Input action used to cycle through available poses.
    private InputAction buttonPress;


    // ==================== INDICATOR SETTINGS ====================

    // Vertical offset from the selected pose icon.
    // Positive values position the indicator above the icon.
    public int arrowAbove;


    // ==================== INDICATOR POSITIONS ====================

    // References to the five pose positions in the UI.
    // Assigned in the Inspector.
    public GameObject[] Location;

    // Index of the currently selected pose slot.
    private int startingPose = 0;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Find the configured tap input action.
        buttonPress = InputSystem.actions.FindAction("TapButton");

        // Begin at the first available pose.
        startingPose = 0;

        // Position the indicator above the starting pose.
        UpdateIndicatorPosition();
    }


    // ==================== INDICATOR UPDATE ====================

    void Update()
    {
        // Ignore input while the game is paused.
        if (Time.timeScale == 0)
            return;

        // Only update when the player taps to change poses.
        if (buttonPress.WasPerformedThisFrame())
        {
            CycleIndicator();
        }
    }


    // ==================== INDICATOR CYCLING ====================

    // Advances the indicator through currently unlocked pose slots.
    private void CycleIndicator()
    {
        // Determine how many poses are currently available.
        // Stage 1 has 3 poses; later stages have 5.
        int availablePoses = Mathf.Clamp(
            GameManager.instance.activePoseCount,
            1,
            Location.Length
        );

        // Advance to the next pose, wrapping back to the first.
        startingPose = (startingPose + 1) % availablePoses;

        // Move the indicator to the selected pose.
        UpdateIndicatorPosition();
    }


    // ==================== INDICATOR POSITIONING ====================

    // Aligns the indicator with the currently selected pose icon.
    private void UpdateIndicatorPosition()
    {
        // Get the selected pose icon's local position.
        Vector2 posePosition =
            Location[startingPose].transform.localPosition;

        // Apply the configured vertical offset.
        transform.localPosition = new Vector2(
            posePosition.x,
            posePosition.y + arrowAbove
        );
    }
}
