using UnityEngine;
using System.Collections;
using TMPro;

// Controls random encouragement animations and pose progression
// announcements. Progression announcements take priority.
public class EncouragementTextScript : MonoBehaviour
{
    // ==================== ANIMATION COMPONENTS ====================

    private Animator anim;
    private SpriteRenderer spriteRenderer;


    // ==================== ANNOUNCEMENT UI ====================

    // Main Critpop text and its black outline.
    public TextMeshProUGUI stageAnnouncementText;
    public TextMeshProUGUI stageAnnouncementOutline;

    // How long an announcement remains visible.
    public float stageAnnouncementDuration = 2.5f;


    // ==================== TRACKING ====================

    private int lastPoseSetVersion = -1;
    private bool stageAnnouncementActive = false;

    private Coroutine encouragementCoroutine;
    private Coroutine stageCoroutine;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        HideStageText();

        if (GameManager.instance != null)
        {
            lastPoseSetVersion =
                GameManager.instance.poseSetVersion;
        }
    }


    // ==================== PROGRESSION DETECTION ====================

    void Update()
    {
        if (GameManager.instance == null)
            return;

        GameManager manager = GameManager.instance;

        // Only check announcements when the pose set changes.
        if (lastPoseSetVersion == manager.poseSetVersion)
            return;

        lastPoseSetVersion = manager.poseSetVersion;

        // Do not announce anything when restarting.
        if (manager.score <= 0f)
        {
            ResetAnnouncements();
            return;
        }

        string message = GetStageMessage(manager.score);

        if (!string.IsNullOrEmpty(message))
        {
            ShowStageAnnouncement(message);
        }
    }


    // ==================== PROGRESSION MESSAGES ====================

    private string GetStageMessage(float score)
    {
        // After the first shuffle, stop announcements.
        // The pose sparkles will continue normally.
        if (score >= 4500f)
            return null;

        if (score >= 3900f)
            return "POSE SHUFFLE!";

        if (score >= 3300f)
            return "MIX IT UP!\nALL POSES IN PLAY!";

        if (score >= 2700f)
            return "LEVEL UP!\nALL 20 POSES DISCOVERED!";

        if (score >= 2100f)
            return "LEVEL UP!\n5 NEW POSES!";

        if (score >= 1500f)
            return "LEVEL UP!\n5 NEW POSES!";

        if (score >= 600f)
            return "LEVEL UP!\n+2 NEW POSES UNLOCKED!";

        return null;
    }


    // ==================== STAGE ANNOUNCEMENTS ====================

    private void ShowStageAnnouncement(string message)
    {
        // Stop encouragement so the two effects never overlap.
        StopEncouragement();

        stageAnnouncementActive = true;

        if (stageCoroutine != null)
        {
            StopCoroutine(stageCoroutine);
        }

        // Update both Critpop text layers.
        if (stageAnnouncementText != null)
        {
            stageAnnouncementText.text = message;
            stageAnnouncementText.gameObject.SetActive(true);
        }

        if (stageAnnouncementOutline != null)
        {
            stageAnnouncementOutline.text = message;
            stageAnnouncementOutline.gameObject.SetActive(true);
        }

        stageCoroutine = StartCoroutine(HideStageAfterDelay());
    }


    private IEnumerator HideStageAfterDelay()
    {
        yield return new WaitForSeconds(stageAnnouncementDuration);

        HideStageText();

        stageAnnouncementActive = false;
        stageCoroutine = null;
    }


    private void HideStageText()
    {
        if (stageAnnouncementText != null)
            stageAnnouncementText.gameObject.SetActive(false);

        if (stageAnnouncementOutline != null)
            stageAnnouncementOutline.gameObject.SetActive(false);
    }


    private void ResetAnnouncements()
    {
        if (stageCoroutine != null)
        {
            StopCoroutine(stageCoroutine);
            stageCoroutine = null;
        }

        HideStageText();
        StopEncouragement();

        stageAnnouncementActive = false;
    }


    // ==================== RANDOM ENCOURAGEMENT ====================

    // Called by PlayerScript every five successful matches.
    public void PlayRandomEncouragement()
    {
        // Don't interrupt progression announcements.
        if (stageAnnouncementActive)
            return;

        if (encouragementCoroutine != null)
        {
            StopCoroutine(encouragementCoroutine);
        }

        int randomText = Random.Range(0, 8);

        switch (randomText)
        {
            case 0:
                anim.Play("AwesomeText", 0, 0f);
                break;

            case 1:
                anim.Play("CoolText", 0, 0f);
                break;

            case 2:
                anim.Play("GroovyText", 0, 0f);
                break;

            case 3:
                anim.Play("WowText", 0, 0f);
                break;

            case 4:
                anim.Play("GreatText", 0, 0f);
                break;

            case 5:
                anim.Play("NiceText", 0, 0f);
                break;

            case 6:
                anim.Play("RadicalText", 0, 0f);
                break;

            case 7:
                anim.Play("TubularText", 0, 0f);
                break;
        }

        encouragementCoroutine =
            StartCoroutine(HideEncouragement());
    }


    // ==================== ENCOURAGEMENT CLEANUP ====================

    private IEnumerator HideEncouragement()
    {
        yield return new WaitForSeconds(1f);

        anim.Play("Idle", 0, 0f);
        spriteRenderer.sprite = null;

        encouragementCoroutine = null;
    }


    private void StopEncouragement()
    {
        if (encouragementCoroutine != null)
        {
            StopCoroutine(encouragementCoroutine);
            encouragementCoroutine = null;
        }

        anim.Play("Idle", 0, 0f);
        spriteRenderer.sprite = null;
    }
}
