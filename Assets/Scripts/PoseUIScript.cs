
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Controls the pose icons displayed in the gameplay UI.
// Handles pose changes, lock animations, and sparkle effects.
public class PoseUIScript : MonoBehaviour
{
    // ==================== POSE SPRITES ====================

    // All available pose sprites, indexed by pose ID.
    public Sprite[] poses;

    // The five possible pose slots.
    public Sprite[] currentPoses = new Sprite[5];


    // ==================== UI COMPONENTS ====================

    private Image image;

    // Pose slot index (0–4), assigned in the Inspector.
    public int poseNum;


    // ==================== LOCK SETTINGS ====================

    // Lock icon assigned to Pose4 and Pose5.
    public GameObject lockedIcon;

    // Duration of the lock burst animation.
    public float lockBurstDuration = 1f;

    private bool unlockComplete = false;
    private Coroutine unlockCoroutine;
    private Vector3 originalLockScale;


    // ==================== SPARKLE SETTINGS ====================

    // Sparkle animation object assigned to each pose.
    public GameObject sparkleEffect;

    // How long the sparkles remain visible.
    public float sparkleDuration = 2.1f;

    private Coroutine sparkleCoroutine;


    // ==================== POSE TRACKING ====================

    // Detects when the pose selection changes.
    private int lastPoseSetVersion = -1;

    // Tracks the previous number of active poses.
    private int lastActivePoseCount = -1;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        image = GetComponent<Image>();

        if (lockedIcon != null)
        {
            originalLockScale = lockedIcon.transform.localScale;
        }

        // Sparkles should not appear when the game starts.
        if (sparkleEffect != null)
        {
            sparkleEffect.SetActive(false);
        }

        RefreshPoseUI();
    }


    // ==================== UI UPDATE ====================

    void Update()
    {
        if (GameManager.instance == null)
            return;

        // Refresh only when the GameManager changes poses.
        if (lastPoseSetVersion != GameManager.instance.poseSetVersion)
        {
            RefreshPoseUI();
        }
    }


    // ==================== REFRESH POSE UI ====================

    private void RefreshPoseUI()
    {
        GameManager manager = GameManager.instance;

        if (manager == null)
            return;

        bool isFirstUpdate = lastPoseSetVersion == -1;

        // Remember how many poses were previously unlocked.
        int previousPoseCount = lastActivePoseCount;

        // Update all five available sprite slots.
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

        // Determine whether this pose slot is unlocked.
        bool isUnlocked =
            poseNum >= 0 &&
            poseNum < manager.activePoseCount &&
            poseNum < currentPoses.Length;

        if (isUnlocked)
        {
            image.sprite = currentPoses[poseNum];
        }

        // A full pose rotation has occurred.
        // Do not sparkle when the game first initializes.
        bool newPoseSet =
            !isFirstUpdate &&
            previousPoseCount == 5 &&
            manager.activePoseCount == 5 &&
            manager.score > 0f;


        // ==================== NORMAL POSES ====================

        if (lockedIcon == null)
        {
            image.enabled = isUnlocked;

            // Sparkle when all five poses rotate.
            if (isUnlocked && newPoseSet)
            {
                PlaySparkle();
            }
        }


        // ==================== LOCKED POSES ====================

        else if (!isUnlocked)
        {
            // Reset the lock if the game restarts.
            if (unlockCoroutine != null)
            {
                StopCoroutine(unlockCoroutine);
                unlockCoroutine = null;
            }

            unlockComplete = false;
            image.enabled = false;

            lockedIcon.SetActive(true);
            lockedIcon.transform.localScale = originalLockScale;

            Animator lockAnimator =
                lockedIcon.GetComponent<Animator>();

            if (lockAnimator != null)
            {
                lockAnimator.ResetTrigger("Burst");
                lockAnimator.Play("LockShake", 0, 0f);
            }

            HideSparkle();
        }

        else if (unlockComplete)
        {
            // Already unlocked: display the pose normally.
            image.enabled = true;
            lockedIcon.SetActive(false);

            // Sparkle whenever the five-pose set rotates.
            if (newPoseSet)
            {
                PlaySparkle();
            }
        }

        else if (unlockCoroutine == null)
        {
            // Newly unlocked: burst the lock first.
            image.enabled = false;

            unlockCoroutine = StartCoroutine(PlayLockBurst());
        }

        // Remember the current progression state.
        lastPoseSetVersion = manager.poseSetVersion;
        lastActivePoseCount = manager.activePoseCount;
    }


    // ==================== LOCK BURST ANIMATION ====================

    private IEnumerator PlayLockBurst()
    {
        Animator lockAnimator =
            lockedIcon.GetComponent<Animator>();

        // Trigger the existing LockBurst animation.
        if (lockAnimator != null)
        {
            lockAnimator.SetTrigger("Burst");
        }

        // Wait until the lock finishes bursting.
        yield return new WaitForSeconds(lockBurstDuration);

        // Reveal the unlocked pose.
        lockedIcon.SetActive(false);
        image.enabled = true;

        unlockComplete = true;
        unlockCoroutine = null;

        // Celebrate the new pose with sparkles!
        PlaySparkle();
    }


    // ==================== SPARKLE ANIMATION ====================

    private void PlaySparkle()
    {
        if (sparkleEffect == null)
            return;

        // Restart the sparkle if another rotation happens.
        if (sparkleCoroutine != null)
        {
            StopCoroutine(sparkleCoroutine);
        }

        sparkleCoroutine = StartCoroutine(ShowSparkle());
    }


    private IEnumerator ShowSparkle()
    {
        // Show the sparkle object.
        sparkleEffect.SetActive(true);

        // Restart its default animation from the beginning.
        Animator sparkleAnimator =
            sparkleEffect.GetComponent<Animator>();

        if (sparkleAnimator != null)
        {
            sparkleAnimator.Rebind();
            sparkleAnimator.Update(0f);
        }

        // Keep sparkles visible briefly.
        yield return new WaitForSeconds(sparkleDuration);

        // Hide the sparkle after its animation finishes.
        sparkleEffect.SetActive(false);

        sparkleCoroutine = null;
    }


    private void HideSparkle()
    {
        if (sparkleCoroutine != null)
        {
            StopCoroutine(sparkleCoroutine);
            sparkleCoroutine = null;
        }

        if (sparkleEffect != null)
        {
            sparkleEffect.SetActive(false);
        }
    }
}