using UnityEngine;
using UnityEngine.UI;

// Controls the pose icons displayed in the gameplay UI.
// Keeps each icon updated with the current poses selected by the GameManager.
public class PoseUIScript : MonoBehaviour
{
    // ==================== POSE SPRITES ====================

    // All available pose sprites used for the UI.
    public Sprite[] poses;

    // The five poses currently available during gameplay.
    public Sprite[] currentPoses = new Sprite[5];


    // ==================== UI COMPONENTS ====================

    // Reference to the Image component displaying this pose icon.
    private Image image;

    // Index (0–4) determining which of the five poses this UI element displays.
    // Assigned in the Inspector for each pose icon.
    public int poseNum;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Get the Image component attached to this UI GameObject.
        image = GetComponent<Image>();
    }


    // ==================== POSE UI UPDATE ====================

    void Update()
    {
        // Retrieve the five currently available poses from the GameManager.
        // These selections may change as the player progresses.
        currentPoses[0] = poses[GameManager.instance.randomWallNums[0]];
        currentPoses[1] = poses[GameManager.instance.randomWallNums[1]];
        currentPoses[2] = poses[GameManager.instance.randomWallNums[2]];
        currentPoses[3] = poses[GameManager.instance.randomWallNums[3]];
        currentPoses[4] = poses[GameManager.instance.randomWallNums[4]];

        // Display the pose assigned to this UI icon.
        image.sprite = currentPoses[poseNum];
    }
}
