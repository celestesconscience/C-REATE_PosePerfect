
using UnityEngine;
using UnityEngine.UI;

// Controls the pose icons displayed in the gameplay UI.
// Updates icons when the available pose selection changes.
public class PoseUIScript : MonoBehaviour
{
    // ==================== POSE SPRITES ====================

    // All available character pose sprites, indexed by pose ID.
    public Sprite[] poses;

    // Stores the five possible pose slots.
    // Only three slots are unlocked at the beginning.
    public Sprite[] currentPoses = new Sprite[5];


    // ==================== UI COMPONENTS ====================

    // Image component displaying this pose icon.
    private Image image;

    // Determines which pose slot this icon displays (0–4).
    // Assigned separately in the Inspector for each icon.
    public int poseNum;

    // Lock icon shown until this pose slot unlocks.
    public GameObject lockedIcon;

    // ==================== POSE PROGRESSION TRACKING ====================

    // Tracks the last pose set displayed by this UI element.
    private int lastPoseSetVersion = -1;

    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Get the Image component attached to this UI object.
        image = GetComponent<Image>();

        // Display the starting pose selection.
        RefreshPoseUI();
    }


    // ==================== UI UPDATE ====================

    void Update()
    {
        // Only refresh when GameManager changes the available poses.
        if (lastPoseSetVersion != GameManager.instance.poseSetVersion)
        {
            RefreshPoseUI();
        }
    }

    // ==================== REFRESH POSE UI ====================

    // Updates available sprites and shows or hides locked pose slots.
    private void RefreshPoseUI()
    {
        GameManager manager = GameManager.instance;

        // Refresh all five possible pose slots.
        for (int i = 0; i < currentPoses.Length; i++)
        {
            // Locked slots should not display a pose.
            if (i >= manager.activePoseCount)
            {
                currentPoses[i] = null;
                continue;
            }

            // Get the pose ID assigned to this slot.
            int poseIndex = manager.randomWallNums[i];

            // Make sure the pose ID exists in the sprite array.
            if (poseIndex > 0 && poseIndex < poses.Length)
            {
                currentPoses[i] = poses[poseIndex];
            }
            else
            {
                currentPoses[i] = null;
            }
        }

        // Check whether this UI icon's slot is unlocked.
        bool isUnlocked =
            poseNum >= 0 &&
            poseNum < manager.activePoseCount &&
            poseNum < currentPoses.Length;

        // Hide locked icons and reveal them when unlocked.
        image.enabled = isUnlocked;

        // Show the lock when the pose is unavailable.
        // Hide the lock when the pose unlocks.
        if (lockedIcon != null)
        {
            lockedIcon.SetActive(!isUnlocked);
        }

        // Display the assigned pose if this slot is unlocked.
        if (isUnlocked)
        {
            image.sprite = currentPoses[poseNum];
        }

        // Remember which pose set is currently displayed.
        lastPoseSetVersion = manager.poseSetVersion;
    }
}
