using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Controls the pose icons displayed in the gameplay UI.
// Handles pose updates, locked slots, and unlock animations.
public class PoseUIScript : MonoBehaviour
{
    // ==================== POSE SPRITES ====================

    // All available pose sprites, indexed by pose ID.
    public Sprite[] poses;

    // The five possible pose slots.
    public Sprite[] currentPoses = new Sprite[5];


    // ==================== UI COMPONENTS ====================

    // Image component displaying this pose.
    private Image image;

    // Pose slot index (0–4), assigned in the Inspector.
    public int poseNum;


    // ==================== LOCK SETTINGS ====================

    // Lock icon for this pose slot, if it has one.
    public GameObject lockedIcon;

    // Time to wait for the burst animation to finish.
    public float lockBurstDuration = 0.6f;

    // Tracks whether this lock has finished unlocking.
    private bool unlockComplete = false;

    // Tracks an unlock animation in progress.
    private Coroutine unlockCoroutine;

    // Original scale of the lock, used when resetting.
    private Vector3 originalLockScale;


    // ==================== POSE TRACKING ====================

    // Tracks the last pose set displayed.
    private int lastPoseSetVersion = -1;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        image = GetComponent<Image>();

        if (lockedIcon != null)
        {
            originalLockScale = lockedIcon.transform.localScale;
        }

        RefreshPoseUI();
    }


    // ==================== UI UPDATE ====================

    void Update()
    {
        if (GameManager.instance == null)
            return;

        // Only refresh when the available poses change.
        if (lastPoseSetVersion != GameManager.instance.poseSetVersion)
        {
            RefreshPoseUI();
        }
    }


    // ==================== REFRESH POSE UI ====================

    private void RefreshPoseUI()
    {
        GameManager manager = GameManager.instance;

        // Update the available pose sprites.
        for (int i = 0; i < currentPoses.Length; i++)
        {
            if (i >= manager.activePoseCount)
            {
                currentPoses[i] = null;
                continue;
            }

            int poseIndex = manager.randomWallNums[i];

            if (poseIndex > 0 && poseIndex < poses.Length)
            {
                currentPoses[i] = poses[poseIndex];
            }
            else
            {
                currentPoses[i] = null;
            }
        }

        // Check whether this pose slot is unlocked.
        bool isUnlocked =
            poseNum >= 0 &&
            poseNum < manager.activePoseCount &&
            poseNum < currentPoses.Length;

        if (isUnlocked)
        {
            image.sprite = currentPoses[poseNum];
        }

        // Poses without locks display normally.
        if (lockedIcon == null)
        {
            image.enabled = isUnlocked;
        }

        // Locked poses remain hidden behind their lock icons.
        else if (!isUnlocked)
        {
            if (unlockCoroutine != null)
            {
                StopCoroutine(unlockCoroutine);
                unlockCoroutine = null;
            }

            unlockComplete = false;
            image.enabled = false;

            lockedIcon.SetActive(true);
            lockedIcon.transform.localScale = originalLockScale;

            // Restart the normal lock-shaking animation.
            Animator lockAnimator = lockedIcon.GetComponent<Animator>();

            if (lockAnimator != null)
            {
                lockAnimator.ResetTrigger("Burst");
                lockAnimator.Play("LockShake", 0, 0f);
            }
        }

        // The lock has already burst open.
        else if (unlockComplete)
        {
            image.enabled = true;
            lockedIcon.SetActive(false);
        }

        // A newly unlocked pose needs to play its burst animation.
        else if (unlockCoroutine == null)
        {
            image.enabled = false;
            unlockCoroutine = StartCoroutine(PlayLockBurst());
        }

        // Remember the current pose set.
        lastPoseSetVersion = manager.poseSetVersion;
    }


    // ==================== LOCK BURST ANIMATION ====================

    private IEnumerator PlayLockBurst()
    {
        // Play the burst animation on this lock.
        Animator lockAnimator = lockedIcon.GetComponent<Animator>();

        if (lockAnimator != null)
        {
            lockAnimator.SetTrigger("Burst");
        }

        // Wait for the burst animation to finish.
        yield return new WaitForSeconds(lockBurstDuration);

        // Hide the lock and reveal the new pose.
        lockedIcon.SetActive(false);
        image.enabled = true;

        unlockComplete = true;
        unlockCoroutine = null;
    }
}