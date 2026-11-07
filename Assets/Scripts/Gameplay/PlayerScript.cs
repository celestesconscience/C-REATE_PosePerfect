using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// Controls player poses, lane movement, wall collisions,
// scoring feedback, combos, and gameplay effects.
public class PlayerScript : MonoBehaviour
{
    // ==================== MENU MANAGER ====================

    public MenuManager menu;

    // ==================== PLAYER INPUT ====================

    private InputAction buttonPress;
    private InputAction buttonHold;

    // ==================== PLAYER POSES ====================

    public Sprite[] poses;
    public Sprite[] currentPoses = new Sprite[5];

    private SpriteRenderer spriteRenderer;
    private int startingPose = 0;

    // ==================== PLAYER MOVEMENT ====================

    public float[] locations = new float[3];

    public int currentLocation;

    // 1 = right, -1 = left.
    private int changeAMT = 1;
// ==================== CONTINUOUS LANE MOVEMENT ====================

    [Header("Continuous Lane Movement Settings")]

    // How long Space must be held before movement begins.
    public float holdToMoveDelay = 0.3f;

    // Time between automatic lane changes.
    public float laneRepeatInterval = 0.4f;

    // Tracks how long Space has been held.
    private float laneHoldTimer = 0f;

    // Tracks when the next lane movement should happen.
    private float nextLaneMoveTime = 0f;

    // Whether automatic movement has started.
    private bool isAutoMoving = false;

    // ==================== LANE DIRECTION ARROW ====================

    [Header("Lane Direction Arrow")]

    public RectTransform laneDirectionArrow;

    private Image laneArrowImage;

    // ==================== ARROW FADE SETTINGS ====================

    [Header("Arrow Fade Settings")]

    // Distance in screen pixels at which fading begins.
    public float arrowFadeDistance = 130f;

    // Distance at which the arrow becomes invisible.
    public float arrowHideDistance = 12f;

    // Opacity when the arrow is faded.
    [Range(0f, 1f)]
    public float arrowFadedAlpha = 0.3f;

    // How smoothly the arrow fades.
    public float arrowFadeSpeed = 5f;

    private float targetArrowAlpha = 1f;
    private float arrowCheckTimer = 0f;

    // ==================== SOUND EFFECTS ====================

    public AudioSource movementAudio;
    public AudioSource poseChangeAudio;
    public AudioSource successSparkleAudio;
    public AudioSource failSmokeAudio;

    // ==================== COLLISION EFFECTS ====================

    public GameObject winObject;
    public GameObject failObject;

    // ==================== ENCOURAGEMENT SYSTEM ====================

    public EncouragementTextScript encouragementText;

    private int successfulMatches = 0;

    // ==================== COMBO SYSTEM ====================

    private int comboCount = 0;

    public TextMeshProUGUI comboText;
    public TextMeshProUGUI comboOutline;

    public Animator comboAnimator;

    // ==================== INITIALIZATION ====================

    void Start()
    {
        GameManager.instance.score = 0;
        GameManager.instance.ResetGame();

        buttonPress =
            InputSystem.actions.FindAction("TapButton");

        buttonHold =
            InputSystem.actions.FindAction("HoldButton");

        spriteRenderer = GetComponent<SpriteRenderer>();

        startingPose = 0;

        spriteRenderer.sprite =
            poses[GameManager.instance.randomWallNums[0]];

        gameObject.tag = "Pose_1";

        transform.position = new Vector2(
            locations[currentLocation],
            transform.position.y
        );

        // Initialize the direction arrow.
        if (laneDirectionArrow != null)
        {
            laneArrowImage =
                laneDirectionArrow.GetComponent<Image>();

            if (laneArrowImage != null)
            {
                Color color = laneArrowImage.color;
                color.a = 1f;
                laneArrowImage.color = color;
            }

            UpdateLaneDirectionArrow();
            FollowPlayerWithArrow();
        }
    }

    // ==================== PLAYER UPDATE ====================

    void Update()
    {
        // Refresh available poses.
        for (int i = 0; i < currentPoses.Length; i++)
        {
            int poseIndex =
                GameManager.instance.randomWallNums[i];

            if (poseIndex > 0 && poseIndex < poses.Length)
            {
                currentPoses[i] = poses[poseIndex];
            }
            else
            {
                currentPoses[i] = null;
            }
        }

        spriteRenderer.sprite = currentPoses[startingPose];

        if (Time.timeScale == 0)
            return;

        // Tap to change poses.
        if (buttonPress.WasPerformedThisFrame())
        {
            poseChange();
        }

        // ==================== CONTINUOUS LANE MOVEMENT ====================

        // Detect whether the player is holding Space.
        if (buttonHold != null && buttonHold.IsPressed())
        {
            // Track how long the button has been held.
            laneHoldTimer += Time.deltaTime;

            // Begin moving once the hold delay is reached.
            if (!isAutoMoving &&
                laneHoldTimer >= holdToMoveDelay)
            {
                isAutoMoving = true;

                // Move into the next lane immediately.
                laneMovement();

                // Schedule the next automatic movement.
                nextLaneMoveTime =
                    Time.time + Mathf.Max(0.05f, laneRepeatInterval);
            }

            // Continue moving while Space remains held.
            else if (isAutoMoving &&
                     Time.time >= nextLaneMoveTime)
            {
                laneMovement();

                // Schedule another lane change.
                nextLaneMoveTime =
                    Time.time + Mathf.Max(0.05f, laneRepeatInterval);
            }
        }
        else
        {
            // Player released Space.
            // Stop movement and reset the hold timer.
            laneHoldTimer = 0f;
            isAutoMoving = false;
        }
    }

    // ==================== ARROW UPDATE ====================

    void LateUpdate()
    {
        FollowPlayerWithArrow();

        // Check nearby walls every 0.1 seconds.
        arrowCheckTimer -= Time.deltaTime;

        if (arrowCheckTimer <= 0f)
        {
            UpdateArrowVisibility();
            arrowCheckTimer = 0.1f;
        }

        SmoothArrowFade();
    }

    // ==================== ARROW DIRECTION ====================

    private void UpdateLaneDirectionArrow()
    {
        if (laneDirectionArrow == null || locations.Length < 2)
            return;

        int nextDirection = changeAMT;

        // Preview direction reversal at lane boundaries.
        if (currentLocation + nextDirection >= locations.Length ||
            currentLocation + nextDirection < 0)
        {
            nextDirection = -nextDirection;
        }

        Vector3 arrowScale =
            laneDirectionArrow.localScale;

        arrowScale.x =
            Mathf.Abs(arrowScale.x) * nextDirection;

        laneDirectionArrow.localScale = arrowScale;
    }

    // ==================== ARROW FOLLOW PLAYER ====================

    private void FollowPlayerWithArrow()
    {
        if (laneDirectionArrow == null || Camera.main == null)
            return;

        RectTransform uiParent =
            laneDirectionArrow.parent as RectTransform;

        if (uiParent == null)
            return;

        Canvas canvas =
            laneDirectionArrow.GetComponentInParent<Canvas>();

        Camera uiCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera != null
                ? canvas.worldCamera
                : Camera.main;
        }

        Vector3 screenPosition =
            Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.z <= 0f)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiParent,
            screenPosition,
            uiCamera,
            out Vector2 localPosition))
        {
            Vector3 arrowPosition =
                laneDirectionArrow.localPosition;

            arrowPosition.x = localPosition.x;

            laneDirectionArrow.localPosition = arrowPosition;
        }
    }

    // ==================== ARROW VISIBILITY ====================

    private void UpdateArrowVisibility()
    {
        if (laneArrowImage == null ||
            laneDirectionArrow == null ||
            Camera.main == null)
        {
            return;
        }

        targetArrowAlpha = 1f;

        Canvas canvas =
            laneDirectionArrow.GetComponentInParent<Canvas>();

        Camera uiCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera != null
                ? canvas.worldCamera
                : Camera.main;
        }

        // Find the arrow's actual screen position.
        Vector2 arrowScreenPosition =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                laneDirectionArrow.position
            );

        // Find the nearest visible wall.
        // Walls use Pose_1 through Pose_5 tags,
        // so search for WallScript instead of a "Wall" tag.
        WallScript[] walls =
            FindObjectsByType<WallScript>();

        float closestDistance = Mathf.Infinity;

        foreach (WallScript wall in walls)
        {
            if (wall == null)
                continue;

            SpriteRenderer wallRenderer =
                wall.GetComponent<SpriteRenderer>();

            if (wallRenderer == null ||
                !wallRenderer.enabled ||
                wallRenderer.color.a <= 0.05f)
            {
                continue;
            }

            // Use the actual visible sprite bounds.
            // This works even if the wall grows as it approaches.
            Bounds bounds = wallRenderer.bounds;

            Vector3 screenMin =
                Camera.main.WorldToScreenPoint(bounds.min);

            Vector3 screenMax =
                Camera.main.WorldToScreenPoint(bounds.max);

            float left = Mathf.Min(screenMin.x, screenMax.x);
            float right = Mathf.Max(screenMin.x, screenMax.x);

            float bottom = Mathf.Min(screenMin.y, screenMax.y);
            float top = Mathf.Max(screenMin.y, screenMax.y);

            // Distance from the arrow to the wall's visible area.
            float distanceX = Mathf.Max(
                left - arrowScreenPosition.x,
                0f,
                arrowScreenPosition.x - right
            );

            float distanceY = Mathf.Max(
                bottom - arrowScreenPosition.y,
                0f,
                arrowScreenPosition.y - top
            );

            float distance = Mathf.Sqrt(
                distanceX * distanceX +
                distanceY * distanceY
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        // Wall overlaps or nearly touches the arrow:
        // hide the arrow completely.
        if (closestDistance <= arrowHideDistance)
        {
            targetArrowAlpha = 0f;
        }

        // Wall is approaching the arrow:
        // fade to 30% opacity.
        else if (closestDistance <= arrowFadeDistance)
        {
            targetArrowAlpha = arrowFadedAlpha;
        }

        // Wall is far away:
        // display the arrow normally.
        else
        {
            targetArrowAlpha = 1f;
        }
    }

    // ==================== SMOOTH ARROW FADE ====================

    private void SmoothArrowFade()
    {
        if (laneArrowImage == null)
            return;

        Color color = laneArrowImage.color;

        color.a = Mathf.MoveTowards(
            color.a,
            targetArrowAlpha,
            arrowFadeSpeed * Time.deltaTime
        );

        laneArrowImage.color = color;
    }

    // ==================== WALL COLLISIONS ====================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(gameObject.tag) &&
            other.transform.position.x == transform.position.x)
        {
            winObject.SetActive(true);

            if (successSparkleAudio != null)
            {
                successSparkleAudio.Play();
            }

            int poseVersionBeforeScore =
                GameManager.instance.poseSetVersion;

            GameManager.instance.AddScore();

            successfulMatches++;
            comboCount++;

            ShowCombo();

            if (successfulMatches % 5 == 0 &&
                GameManager.instance.poseSetVersion ==
                poseVersionBeforeScore)
            {
                encouragementText.PlayRandomEncouragement();
            }
        }
        else
        {
            comboCount = 0;

            comboText.gameObject.SetActive(false);
            comboOutline.gameObject.SetActive(false);

            spriteRenderer.color =
                new Color32(255, 255, 255, 0);

            failObject.SetActive(true);

            if (failSmokeAudio != null)
            {
                failSmokeAudio.Play();
            }

            StartCoroutine(DelayedGameOver());
        }
    }

    // ==================== COMBO DISPLAY ====================

    private void ShowCombo()
    {
        if (comboCount < 2)
            return;

        string comboMessage = comboCount + "X COMBO!";

        comboText.text = comboMessage;
        comboOutline.text = comboMessage;

        comboText.gameObject.SetActive(true);
        comboOutline.gameObject.SetActive(true);

        comboAnimator.Play("ComboPop", 0, 0f);
    }

    // ==================== GAME OVER ====================

    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(1.5f);

        menu.GameOver_Open_UI();
    }

    // ==================== COLLISION CLEANUP ====================

    void OnTriggerExit2D(Collider2D other)
    {
        winObject.SetActive(false);
    }

    // ==================== POSE CHANGING ====================

    private void poseChange()
    {
        startingPose++;

        int availablePoses = Mathf.Clamp(
            GameManager.instance.activePoseCount,
            1,
            currentPoses.Length
        );

        if (startingPose >= availablePoses)
        {
            startingPose = 0;
        }

        if (poseChangeAudio != null)
        {
            poseChangeAudio.Play();
        }

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

    private void laneMovement()
    {
        currentLocation += changeAMT;

        // Reverse at right boundary.
        if (currentLocation == locations.Length)
        {
            changeAMT = -changeAMT;
            currentLocation += changeAMT;
            currentLocation += changeAMT;
        }

        // Reverse at left boundary.
        else if (currentLocation == -1)
        {
            changeAMT = -changeAMT;
            currentLocation += changeAMT;
            currentLocation += changeAMT;
        }

        transform.position = new Vector2(
            locations[currentLocation],
            transform.position.y
        );

        UpdateLaneDirectionArrow();

        if (movementAudio != null)
        {
            movementAudio.Play();
        }
    }
}