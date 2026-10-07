using UnityEngine;
using UnityEngine.SceneManagement;

// Everything related to controlling the Game Manager goes in here
public class GameManager : MonoBehaviour // <-- MonoBehaviour allows Unity to attach script to a GameObject
// class = container for objects behaviour; class name is GameManager
{
    // Reference to the Pause Overlay
    public GameObject pauseOverlay;

    // Reference to the Controls Overlay
    public GameObject controlsOverlay;

    // Opens the Controls Overlay
    public void OpenControls()
    {
        if(pauseOverlay.activeSelf) // Checks if the pause overlay is currently active
        {
            pauseOverlay.SetActive(false); // Hide the pause overlay before showing the controls overlay
            controlsOverlay.SetActive(true); // Show the controls overlay
            Time.timeScale = 0; // Pause the game by setting the time scale to 0
            print("Controls menu opened. Game frozen"); // For debugging purposes
        }
    }

    // Closes the Controls Overlay
    public void CloseControls() // Method for closing the controls overlay and showing the pause overlay
    {
            controlsOverlay.SetActive(false); // Hide the controls overlay before showing the pause overlay
            pauseOverlay.SetActive(true); // Show the pause overlay after closing the controls overlay
            Time.timeScale = 0; // This should be 0 to keep the game paused
            print("Controls menu closed. Returned to pause menu."); // For debugging purposes
    }

    // Toggles the visiblity of the Controls Panel
    public void ToggleControls()
    {
        // If the Controls overlay is currently open
        if(controlsOverlay.activeSelf)
        {
            CloseControls();
        }
        else
        {
            OpenControls();
        }
    }

    // Opens the Pause Overlay
    public void OpenPause()
    {
        if(!controlsOverlay.activeSelf) // Checks that the controls overlay isn't open before allowing pause
        {
            pauseOverlay.SetActive(true);
            Time.timeScale = 0; // Pause the game by setting the time scale to 0
            print("Pause menu opened. Game frozen"); // For debugging purposes
        }
    }

    // Closes the Pause Overlay
    public void ClosePause()
    {
        pauseOverlay.SetActive(false);
        Time.timeScale = 1; // Resume the game by setting the time scale back to 1
        print("Pause menu closed. Game resumed"); // For debugging: print a message when the pause menu is closed
    }

    // // Toggles the visibility of the Pause Panel  
    // public void TogglePause()  
    // {  
    //     // Set the Pause Overlay to the opposiite of its current active state  
    //     // activeSelf checks if the panel is currently active (true) or inactive (false)
    //     // ! means "Not", so it changes true to false or false to true
    //     // Basically, set pause overlay((!opposite)pauseoverlay.whatever state)

    //     // If the Pause overlay is currently open
    //     if(pauseOverlay.activeSelf)
    //     {
    //         ClosePause();
    //     }
    //     else
    //     {
    //         OpenPause();
    //     }
    // }

    // Returns to the Main Menu from the Game Pause
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Resumes the game since you paused to get to press Main Menu
        SceneManager.LoadScene("MainMenu");
    }

    // Restarts the game by reloading the Game scene
    public void RestartGame()
    {
        Time.timeScale =1; // <-- Resume the game by setting the time scale back to 1
        SceneManager.LoadScene("TestGameScene");
        print("Game restarted"); // For debugging purposes
    }

    // Quits the game
    public void QuitGame()
    {
        Application.Quit(); // Quit the application
        print("Game quit"); // For debugging purposes
    }
}