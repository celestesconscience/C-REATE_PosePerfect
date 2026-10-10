using UnityEngine;

// Everything related to controlling the Game
public class GameManager : MonoBehaviour // <-- MonoBehaviour allows Unity to attach script to a GameObject
// class = container for objects behaviour; class name is GameManager
{   
    //Makes GameManager accessible to everyone
    public static GameManager instance; 

    //Happens before start, only for important stuff
    void Awake()
    {
        //Singleton pattern, makes gamemanager persist even when changing scenes, this helps keep high scores across diff scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
}