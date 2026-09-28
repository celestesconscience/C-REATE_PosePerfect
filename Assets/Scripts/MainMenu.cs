using UnityEngine;
using UnityEngine.SceneManagement;

// Everything related to controlling the Main Menu goes in here
public class MainMenu : MonoBehaviour // <-- MonoBehaviour allows Unity to attach script to a GameObject
// class = container for objects behaviour; class name is MainMenu
{
    // Reference to the Controls Overlay
    public GameObject controlsOverlay;

    // Opens the Controls Overlay
    public void OpenControls()
    {
        controlsOverlay.SetActive(true);
    }

    // Closes the Controls Overlay
    public void CloseControls()
    {
        controlsOverlay.SetActive(false);
    }

    // Toggles the visibility of the Controls Panel
    public void ToggleControls()
    {
        // Set the Controls Overlay to the opposiite of its current active state
        // activeSelf checks if the panel is currently active (true) or inactive (false)
        // ! means "Not", so it changes true to false or false to true
        // Basically, set controls overlay((!opposite)controlsoverlay.whatever state)
        controlsOverlay.SetActive(!controlsOverlay.activeSelf);
    }

    // Opens the Credits Scene
    public void OpenCredits() // <-- This is a method that opens the Credits scene
    {
        SceneManager.LoadScene("Credits"); // <-- SceneManager = Unity's scene management system
                                          // . = Acesssing something that belongs to SceneManager
                                          // LoadScene = a method that loads a new scene by its name
    }

    // Opens the High Scores Scene
    public void OpenHighScores()
    {
        SceneManager.LoadScene("HighScores"); // <-- SceneManager = Unity's scene management system
                                              // . = Acesssing something that belongs to SceneManager
                                              // LoadScene = a method that loads a new scene by its name
    }

    // Opens the Tutorial Scene
    public void OpenTutorial()
    {
        SceneManager.LoadScene("Tutorial"); // <-- SceneManager = Unity's scene management system
                                              // . = Acesssing something that belongs to SceneManager
                                              // LoadScene = a method that loads a new scene by its name
    }

    // Opens the Main Game Scene using the START Button
    public void OpenMainGame()
    {
        SceneManager.LoadScene("Game"); // <-- SceneManager = Unity's scene management system
                                              // . = Acesssing something that belongs to SceneManager
                                              // LoadScene = a method that loads a new scene by its name
    }
}