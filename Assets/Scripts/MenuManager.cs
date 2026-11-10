using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

// Manages scene navigation, menu overlays, game pausing,
// menu audio, and scrolling credits.
public class MenuManager : MonoBehaviour
{
    // ==================== MENU OVERLAYS ====================

    // Reference to the Pause Menu overlay.
    public GameObject pauseOverlay;

    // Reference to the Controls overlay.
    public GameObject controlsOverlay;

    // Reference to the Game Over overlay.
    public GameObject gameOverOverlay;


    // ==================== MENU SOUND EFFECTS ====================

    public GameObject buttonClickAudio;
    public GameObject confirmAudio;

    private static GameObject persistentButtonClickAudio;
    private static GameObject persistentConfirmAudio;


    // ==================== GAME AUDIO ====================

    public AudioSource backgroundMusic;
    public AudioSource gameOverAudio;


    // ==================== SCROLLING CREDITS ====================

    [Header("Scrolling Credits")]

    // Visible area where the credits scroll.
    public RectTransform creditsViewport;

    // Parent containing all scrolling credit text objects.
    public RectTransform scrollingCreditsContent;

    // How fast the credits move upward.
    public float creditsScrollSpeed = 55f;

    // Delay before the credits restart.
    public float creditsRestartDelay = 2f;

    // Whether the credits should repeat.
    public bool loopCredits = true;


    // ==================== INITIALIZATION ====================

    void Awake()
    {
        // Store the first button click audio object.
        if (buttonClickAudio != null)
        {
            if (persistentButtonClickAudio == null)
            {
                persistentButtonClickAudio = buttonClickAudio;
            }
            else
            {
                Destroy(buttonClickAudio);
            }
        }

        // Store the first confirmation audio object.
        if (confirmAudio != null)
        {
            if (persistentConfirmAudio == null)
            {
                persistentConfirmAudio = confirmAudio;
            }
            else
            {
                Destroy(confirmAudio);
            }
        }
    }

    void Start()
    {
        // Only run scrolling credits when the references
        // have been assigned in the Credits scene.
        if (creditsViewport != null &&
            scrollingCreditsContent != null)
        {
            StartCoroutine(ScrollCredits());
        }
    }


    // ==================== SCROLLING CREDITS ====================

    private IEnumerator ScrollCredits()
    {
        // Allow the Canvas layout to initialize.
        yield return null;

        Canvas.ForceUpdateCanvases();

        // Update the layout to calculate the height
        // of all credit text objects together.
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            scrollingCreditsContent
        );

        Canvas.ForceUpdateCanvases();

        // Get the height of the visible credits area.
        float viewportHeight =
            creditsViewport.rect.height;

        // Get the total height of the credits content.
        float contentHeight =
            scrollingCreditsContent.rect.height;

        // Starting position: fully below the viewport.
        float startY = -(
            viewportHeight / 2f +
            contentHeight / 2f
        );

        // Ending position: fully above the viewport.
        float endY =
            viewportHeight / 2f +
            contentHeight / 2f;

        // Start the scrolling loop.
        while (true)
        {
            Vector2 position =
                scrollingCreditsContent.anchoredPosition;

            // Reset the credits below the screen.
            position.y = startY;

            scrollingCreditsContent.anchoredPosition =
                position;

            // ==================== SCROLL UPWARD ====================

            while (position.y < endY)
            {
                position.y = Mathf.MoveTowards(
                    position.y,
                    endY,
                    creditsScrollSpeed * Time.unscaledDeltaTime
                );

                scrollingCreditsContent.anchoredPosition =
                    position;

                yield return null;
            }

            // Stop once the credits finish if looping is off.
            if (!loopCredits)
            {
                yield break;
            }

            // Wait before restarting.
            yield return new WaitForSecondsRealtime(
                creditsRestartDelay
            );
        }
    }


    // ==================== MENU SOUND EFFECTS ====================

    public void Play_Button_Click_SFX()
    {
        if (persistentButtonClickAudio != null)
        {
            AudioSource audioSource =
                persistentButtonClickAudio.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    public void Play_Confirm_SFX()
    {
        if (persistentConfirmAudio != null)
        {
            AudioSource audioSource =
                persistentConfirmAudio.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }


    // ==================== SCENE NAVIGATION ====================

    // Returns to the Main Menu.
    public void Open_MainMenu_Scene()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("MainMenu");
    }

    // Opens the original Controls / Instructions scene.
    public void Open_Instructions_Scene()
    {
        SceneManager.LoadScene("Instructions");
    }

    // NEW: Opens the Power-Ups & Scoring guide.
    // Instructions -> GameGuide
    public void Open_GameGuide_Scene()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("GameGuide");
    }

    // Starts or restarts gameplay.
    // GameGuide -> Game
    public void Open_Game_Scene()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("Game");

        print("Game restarted");
    }

    // Opens High Scores.
    public void Open_HighScores_Scene()
    {
        SceneManager.LoadScene("HighScores");
    }

    // Opens Credits.
    public void Open_Credits_Scene()
    {
        SceneManager.LoadScene("Credits");
    }


    // ==================== PAUSE MENU ====================

    public void Pause_Open_UI()
    {
        pauseOverlay.SetActive(true);

        Time.timeScale = 0;

        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }

        print("Pause menu opened. Game frozen");
    }

    public void Pause_Close_UI()
    {
        pauseOverlay.SetActive(false);

        Time.timeScale = 1;

        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause();
        }

        print("Pause menu closed. Game resumed");
    }


    // ==================== CONTROLS MENU ====================

    public void Controls_Open_UI()
    {
        controlsOverlay.SetActive(true);

        print("Controls menu opened. Game frozen");
    }

    public void Controls_Close_UI()
    {
        controlsOverlay.SetActive(false);

        print("Controls menu closed. Returned to pause menu.");
    }


    // ==================== GAME OVER MENU ====================

    public void GameOver_Open_UI()
    {
        Time.timeScale = 0;

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        if (gameOverAudio != null)
        {
            gameOverAudio.Play();
        }

        gameOverOverlay.SetActive(true);
    }


    // ==================== APPLICATION CONTROLS ====================

    public void QuitGame()
    {
        Application.Quit();
    }
}
