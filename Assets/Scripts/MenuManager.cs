using UnityEngine;
using UnityEngine.SceneManagement;

// Controls the scene management and UI of the game
public class MenuManager : MonoBehaviour // <-- MonoBehaviour allows Unity to attach script to a GameObject
// class = container for objects behaviour; class name is GameManager
{   
    // Reference to the Pause Overlay
    public GameObject pauseOverlay;

    // Reference to the Controls Overlay
    public GameObject controlsOverlay;

    // Reference to the Controls Overlay
    public GameObject gameOverOverlay;

    // Returns to the Main Menu from the Game Pause
    public void Open_MainMenu_Scene()

    {
        Time.timeScale = 1; // Resumes the game since you paused to get to press Main Menu
        SceneManager.LoadScene("MainMenu");
    }

    public void Open_Instructions_Scene()
    {
        SceneManager.LoadScene("Instructions");
    }

    // Goes to game scene, can be used as both a start game and restart game
    public void Open_Game_Scene()
    {
        Time.timeScale = 1; // <-- Resume the game by setting the time scale back to 1
        SceneManager.LoadScene("Game");
        //print("Game restarted"); // For debugging purposes
    }

    public void Open_HighScores_Scene()
    {
        SceneManager.LoadScene("HighScores");
    }

    public void Open_Credits_Scene()
    {
        SceneManager.LoadScene("Credits");
    }

    // Opens the Pause Overlay
    public void Pause_Open_UI()
    {
        if(!controlsOverlay.activeSelf) // Checks that the controls overlay isn't open before allowing pause
        {
            pauseOverlay.SetActive(true);
            Time.timeScale = 0; // Pause the game by setting the time scale to 0
            //print("Pause menu opened. Game frozen"); // For debugging purposes
        }
    }

    // Closes the Pause Overlay
    public void Pause_Close_UI()
    {
        pauseOverlay.SetActive(false);
        Time.timeScale = 1; // Resume the game by setting the time scale back to 1
        //print("Pause menu closed. Game resumed"); // For debugging: print a message when the pause menu is closed
    }

    // Opens the Controls Overlay
    public void Controls_Open_UI()
    {
        //if(pauseOverlay.activeSelf) // Checks if the pause overlay is currently active
        //{
        //    pauseOverlay.SetActive(false); // Hide the pause overlay before showing the controls overlay
        controlsOverlay.SetActive(true); // Show the controls overlay
        //    Time.timeScale = 0; // Pause the game by setting the time scale to 0
        //    print("Controls menu opened. Game frozen"); // For debugging purposes
        //}
    }

    public void GameOver_Open_UI()
    {
        //if(pauseOverlay.activeSelf) // Checks if the pause overlay is currently active
        //{
        //    pauseOverlay.SetActive(false); // Hide the pause overlay before showing the controls overlay
        Time.timeScale = 0; 
        gameOverOverlay.SetActive(true); // Show the controls overlay
        //    Time.timeScale = 0; // Pause the game by setting the time scale to 0
        //    print("Controls menu opened. Game frozen"); // For debugging purposes
        //}
    }

    // Closes the Controls Overlay
    public void Controls_Close_UI() // Method for closing the controls overlay and showing the pause overlay
    {
            controlsOverlay.SetActive(false); // Hide the controls overlay before showing the pause overlay
        //    pauseOverlay.SetActive(true); // Show the pause overlay after closing the controls overlay
        //    Time.timeScale = 0; // This should be 0 to keep the game paused
        //    print("Controls menu closed. Returned to pause menu."); // For debugging purposes
    }

    // Quits the game
    public void QuitGame()
    {
        Application.Quit(); // Quit the application
    }

}