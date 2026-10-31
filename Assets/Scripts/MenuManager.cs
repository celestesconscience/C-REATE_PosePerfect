using UnityEngine;
using UnityEngine.SceneManagement;

// Manages scene navigation, menu overlays, game pausing,
// and audio associated with menus and game state changes.
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

    // References to the menu button click and confirmation sound objects.
    public GameObject buttonClickAudio;
    public GameObject confirmAudio;

    // Shared references to menu audio objects across MenuManager instances.
    private static GameObject persistentButtonClickAudio;
    private static GameObject persistentConfirmAudio;


    // ==================== GAME AUDIO ====================

    // Background music played during gameplay.
    public AudioSource backgroundMusic;

    // Sound effect played when the game ends.
    public AudioSource gameOverAudio;


    // ==================== INITIALIZATION ====================

    void Awake()
    {
        // Store the first button click audio object.
        // Destroy duplicates if another instance is already assigned.
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
        // Destroy duplicates if another instance is already assigned.
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


    // ==================== MENU SOUND EFFECTS ====================

    // Plays the sound effect when a menu button is clicked.
    public void Play_Button_Click_SFX()
    {
        // Check that the shared button click audio object exists.
        if (persistentButtonClickAudio != null)
        {
            // Retrieve the AudioSource component.
            AudioSource audioSource =
                persistentButtonClickAudio.GetComponent<AudioSource>();

            // Play the sound if the AudioSource is available.
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    // Plays the confirmation sound effect for menu selections.
    public void Play_Confirm_SFX()
    {
        // Check that the shared confirmation audio object exists.
        if (persistentConfirmAudio != null)
        {
            // Retrieve the AudioSource component.
            AudioSource audioSource =
                persistentConfirmAudio.GetComponent<AudioSource>();

            // Play the sound if the AudioSource is available.
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }


    // ==================== SCENE NAVIGATION ====================

    // Returns the player to the Main Menu scene.
    public void Open_MainMenu_Scene()
    {
        // Restore normal game time before switching scenes.
        Time.timeScale = 1;

        SceneManager.LoadScene("MainMenu");
    }

    // Opens the How to Play / Instructions scene.
    public void Open_Instructions_Scene()
    {
        SceneManager.LoadScene("Instructions");
    }

    // Opens the Game scene when starting or restarting a game.
    public void Open_Game_Scene()
    {
        // Restore normal game time before starting gameplay.
        Time.timeScale = 1;

        SceneManager.LoadScene("Game");

        // Debug message confirming the game scene was loaded.
        print("Game restarted");
    }

    // Opens the High Scores scene.
    public void Open_HighScores_Scene()
    {
        SceneManager.LoadScene("HighScores");
    }

    // Opens the Credits scene.
    public void Open_Credits_Scene()
    {
        SceneManager.LoadScene("Credits");
    }


    // ==================== PAUSE MENU ====================

    // Opens the Pause Menu and freezes gameplay.
    public void Pause_Open_UI()
    {
        // Display the Pause Menu overlay.
        pauseOverlay.SetActive(true);

        // Freeze gameplay by setting the time scale to zero.
        Time.timeScale = 0;

        // Pause background music while the game is paused.
        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }

        // Debug message confirming the game is paused.
        print("Pause menu opened. Game frozen");
    }

    // Closes the Pause Menu and resumes gameplay.
    public void Pause_Close_UI()
    {
        // Hide the Pause Menu overlay.
        pauseOverlay.SetActive(false);

        // Resume gameplay by restoring the normal time scale.
        Time.timeScale = 1;

        // Resume background music from where it was paused.
        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause();
        }

        // Debug message confirming gameplay has resumed.
        print("Pause menu closed. Game resumed");
    }


    // ==================== CONTROLS MENU ====================

    // Opens the Controls overlay.
    public void Controls_Open_UI()
    {
        // Display the Controls overlay.
        controlsOverlay.SetActive(true);

        // Debug message confirming the Controls overlay opened.
        print("Controls menu opened. Game frozen");
    }

    // Closes the Controls overlay.
    public void Controls_Close_UI()
    {
        // Hide the Controls overlay.
        controlsOverlay.SetActive(false);

        // Debug message confirming the Controls overlay closed.
        print("Controls menu closed. Returned to pause menu.");
    }


    // ==================== GAME OVER MENU ====================

    // Opens the Game Over overlay and stops gameplay.
    public void GameOver_Open_UI()
    {
        // Freeze gameplay when the player loses.
        Time.timeScale = 0;

        // Stop background music when the game ends.
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        // Play the Game Over sound effect.
        if (gameOverAudio != null)
        {
            gameOverAudio.Play();
        }

        // Display the Game Over overlay.
        gameOverOverlay.SetActive(true);
    }


    // ==================== APPLICATION CONTROLS ====================

    // Closes the application when the Quit button is selected.
    public void QuitGame()
    {
        Application.Quit();
    }
}
