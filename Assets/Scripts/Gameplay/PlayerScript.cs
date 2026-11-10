using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// Handles player poses, lane movement, wall collisions, gameplay feedback,
// combo tracking, and sounds played by the player.
public class PlayerScript : MonoBehaviour
{
    // ==================== MENU MANAGER ====================

    public MenuManager menu;


    // ==================== PLAYER INPUT ====================

    private InputAction buttonPress;
    private InputAction buttonHold;

    // Prevents player input after losing the final heart.
    private bool isGameOver = false;


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

    [Header("Continuous Lane Movement Settings")]

    public float holdToMoveDelay = 0.3f;
    public float laneRepeatInterval = 0.4f;

    private float laneHoldTimer = 0f;
    private float nextLaneMoveTime = 0f;
    private bool isAutoMoving = false;


    // ==================== LANE DIRECTION ARROW ====================

    [Header("Lane Direction Arrow")]

    public RectTransform laneDirectionArrow;

    private Image laneArrowImage;

    [Header("Arrow Fade Settings")]

    public float arrowFadeDistance = 130f;
    public float arrowHideDistance = 12f;

    [Range(0f, 1f)]
    public float arrowFadedAlpha = 0.3f;

    public float arrowFadeSpeed = 5f;

    private float targetArrowAlpha = 1f;
    private float arrowCheckTimer = 0f;


    // ==================== GAMEPLAY SOUND EFFECTS ====================

    public AudioSource movementAudio;
    public AudioSource poseChangeAudio;
    public AudioSource successSparkleAudio;
    public AudioSource failSmokeAudio;
    public AudioSource heartLostAudio;


    // ==================== COLLISION EFFECTS ====================

    public GameObject winObject;
    public GameObject failObject;
    public GameObject armorObject;


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
        // Reset Game Over state.
        isGameOver = false;

        GameManager.instance.score = 0;
        GameManager.instance.ResetGame();

        buttonPress = InputSystem.actions.FindAction("TapButton");
        buttonHold = InputSystem.actions.FindAction("HoldButton");

        spriteRenderer = GetComponent<SpriteRenderer>();

        startingPose = 0;

        spriteRenderer.sprite =
            poses[GameManager.instance.randomWallNums[0]];

        // Restore player visibility.
        spriteRenderer.color = Color.white;

        gameObject.tag = "Pose_1";

        transform.position = new Vector2(
            locations[currentLocation],
            transform.position.y
        );

        // Begin with armor hidden.
        if (armorObject != null)
            armorObject.SetActive(false);

        // Initialize the lane direction arrow.
        if (laneDirectionArrow != null)
        {
            // Restore arrow visibility after restarting.
            laneDirectionArrow.gameObject.SetActive(true);

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
        // Stop player updates after Game Over.
        if (isGameOver)
            return;

        // Refresh pose sprites.
        for (int i = 0; i < currentPoses.Length; i++)
        {
            int poseIndex =
                GameManager.instance.randomWallNums[i];

            if (poseIndex > 0 && poseIndex < poses.Length)
                currentPoses[i] = poses[poseIndex];
            else
                currentPoses[i] = null;
        }

        spriteRenderer.sprite = currentPoses[startingPose];

        // Keep armor visuals synchronized with actual protection.
        if (armorObject != null)
        {
            bool armorActive =
                GameManager.instance.armorAlreadyOn &&
                GameManager.instance.armor &&
                GameManager.instance.armorTimeLeft > 0;

            if (armorObject.activeSelf != armorActive)
                armorObject.SetActive(armorActive);
        }

        // Ignore gameplay input while paused.
        if (Time.timeScale == 0)
            return;

        // Tap to change poses.
        if (buttonPress != null &&
            buttonPress.WasPerformedThisFrame())
        {
            poseChange();
        }

        // Hold Space to move repeatedly between lanes.
        if (buttonHold != null && buttonHold.IsPressed())
        {
            laneHoldTimer += Time.deltaTime;

            if (!isAutoMoving &&
                laneHoldTimer >= holdToMoveDelay)
            {
                isAutoMoving = true;

                laneMovement();

                nextLaneMoveTime =
                    Time.time +
                    Mathf.Max(0.05f, laneRepeatInterval);
            }
            else if (isAutoMoving &&
                     Time.time >= nextLaneMoveTime)
            {
                laneMovement();

                nextLaneMoveTime =
                    Time.time +
                    Mathf.Max(0.05f, laneRepeatInterval);
            }
        }
        else
        {
            laneHoldTimer = 0f;
            isAutoMoving = false;
        }

        // Preserve the existing combo-based score adjustment.
        if (comboCount % 10 == 0 && comboCount < 101)
        {
            GameManager.instance.scoreAddAmt =
                100f + comboCount;
        }
    }


    // ==================== ARROW UPDATE ====================

    void LateUpdate()
    {
        // Stop all arrow updates after Game Over.
        if (isGameOver)
            return;

        FollowPlayerWithArrow();

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
        if (laneDirectionArrow == null ||
            locations.Length < 2)
            return;

        int nextDirection = changeAMT;

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
        if (laneDirectionArrow == null ||
            Camera.main == null)
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

            laneDirectionArrow.localPosition =
                arrowPosition;
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

        Vector2 arrowScreenPosition =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                laneDirectionArrow.position
            );

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

            Bounds bounds = wallRenderer.bounds;

            Vector3 screenMin =
                Camera.main.WorldToScreenPoint(bounds.min);

            Vector3 screenMax =
                Camera.main.WorldToScreenPoint(bounds.max);

            float left =
                Mathf.Min(screenMin.x, screenMax.x);

            float right =
                Mathf.Max(screenMin.x, screenMax.x);

            float bottom =
                Mathf.Min(screenMin.y, screenMax.y);

            float top =
                Mathf.Max(screenMin.y, screenMax.y);

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
                closestDistance = distance;
        }

        if (closestDistance <= arrowHideDistance)
            targetArrowAlpha = 0f;
        else if (closestDistance <= arrowFadeDistance)
            targetArrowAlpha = arrowFadedAlpha;
        else
            targetArrowAlpha = 1f;
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
        // Ignore collisions once Game Over has started.
        if (isGameOver ||
            GameManager.instance.livesLeft <= 0)
        {
            return;
        }

        // Power-ups handle their own collection.
        if (other.GetComponentInParent<PowerUpScript>() != null ||
            other.CompareTag("PowerUp"))
        {
            return;
        }

        // Ignore unrelated trigger objects.
        WallScript wall =
            other.GetComponentInParent<WallScript>();

        if (wall == null)
            return;

        // ==================== SUCCESSFUL MATCH ====================

        if (other.CompareTag(gameObject.tag) &&
            Mathf.Abs(
                other.transform.position.x -
                transform.position.x
            ) < 0.01f)
        {
            StartCoroutine(ActivateSuccessAnim());

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
                if (encouragementText != null)
                {
                    encouragementText
                        .PlayRandomEncouragement();
                }
            }
        }

        // ==================== FAILED MATCH ====================

        else
        {
            comboCount = 0;

            if (comboText != null)
                comboText.gameObject.SetActive(false);

            if (comboOutline != null)
                comboOutline.gameObject.SetActive(false);

            // Check whether armor can absorb the hit.
            if (GameManager.instance.armorAlreadyOn &&
                GameManager.instance.armor &&
                GameManager.instance.armorTimeLeft > 0)
            {
                // Stop the active armor timer.
                GameManager.instance.StopCoroutine(
                    "activateArmor"
                );

                // Consume armor without losing a heart.
                GameManager.instance.armorAlreadyOn = false;
                GameManager.instance.armor = false;
                GameManager.instance.armorTimeLeft = 0;

                if (armorObject != null)
                    armorObject.SetActive(false);
            }
            else
            {
                // Remove one heart.
                if (GameManager.instance.livesLeft > 0)
                {
                    GameManager.instance.livesLeft--;

                    if (heartLostAudio != null)
                        heartLostAudio.Play();
                }
            }

            StartCoroutine(ActivateFailAnim());

            // ==================== GAME OVER ====================

            if (GameManager.instance.livesLeft <= 0)
            {
                // Immediately disable player input.
                isGameOver = true;

                // Reset continuous movement.
                laneHoldTimer = 0f;
                nextLaneMoveTime = 0f;
                isAutoMoving = false;

                // Hide the player.
                spriteRenderer.color =
                    new Color32(255, 255, 255, 0);

                // Hide the lane direction arrow.
                if (laneDirectionArrow != null)
                {
                    laneDirectionArrow.gameObject
                        .SetActive(false);
                }

                // Wait before displaying the Game Over menu.
                StartCoroutine(DelayedGameOver());
            }
        }
    }


    // ==================== SUCCESS ANIMATION ====================

    private IEnumerator ActivateSuccessAnim()
    {
        if (winObject != null)
        {
            winObject.SetActive(false);
            winObject.SetActive(true);
        }

        if (successSparkleAudio != null)
            successSparkleAudio.Play();

        yield return new WaitForSeconds(2f);

        if (winObject != null)
            winObject.SetActive(false);
    }


    // ==================== FAILURE ANIMATION ====================

    private IEnumerator ActivateFailAnim()
    {
        if (failObject != null)
        {
            failObject.SetActive(false);
            failObject.SetActive(true);
        }

        if (failSmokeAudio != null)
            failSmokeAudio.Play();

        yield return new WaitForSeconds(2f);

        if (failObject != null)
            failObject.SetActive(false);
    }


    // ==================== COMBO DISPLAY ====================

    private void ShowCombo()
    {
        if (comboCount < 2)
            return;

        string comboMessage =
            comboCount + "X COMBO!";

        if (comboText != null)
        {
            comboText.text = comboMessage;
            comboText.gameObject.SetActive(true);
        }

        if (comboOutline != null)
        {
            comboOutline.text = comboMessage;
            comboOutline.gameObject.SetActive(true);
        }

        if (comboAnimator != null)
        {
            comboAnimator.Play(
                "ComboPop",
                0,
                0f
            );
        }
    }


    // ==================== GAME OVER DELAY ====================

    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(1.5f);

        if (menu != null)
            menu.GameOver_Open_UI();
    }


    // ==================== POSE CHANGING ====================

    private void poseChange()
    {
        // Prevent pose changes after Game Over.
        if (isGameOver)
            return;

        startingPose++;

        int availablePoses = Mathf.Clamp(
            GameManager.instance.activePoseCount,
            1,
            currentPoses.Length
        );

        if (startingPose >= availablePoses)
            startingPose = 0;

        if (poseChangeAudio != null)
            poseChangeAudio.Play();

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
        // Prevent lane movement after Game Over.
        if (isGameOver)
            return;

        currentLocation += changeAMT;

        // Reverse direction at the rightmost lane.
        if (currentLocation == locations.Length)
        {
            changeAMT = -changeAMT;

            currentLocation += changeAMT;
            currentLocation += changeAMT;
        }

        // Reverse direction at the leftmost lane.
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
            movementAudio.Play();
    }
}
