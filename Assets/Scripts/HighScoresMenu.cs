using UnityEngine;
using UnityEngine.SceneManagement;


// Everything related to controlling the High Scores Menu goes in here
public class HighScoresMenu : MonoBehaviour
{
    // Closes the High Scores Scene and Returns to Main Menu
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // <-- SceneManager = Unity's scene management system
                                            // . = Acesssing something that belongs to SceneManager
                                            // LoadScene = a method that loads a new scene by its name
    }
}