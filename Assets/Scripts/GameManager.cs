using UnityEngine;

// Everything related to controlling the Game
public class GameManager : MonoBehaviour // <-- MonoBehaviour allows Unity to attach script to a GameObject
{
    public static GameManager instance;//allows other scripts to access gameManager variables

    public float score;

    public float[] highScore = new float [5];

    //happens before start, only for important stuff
    void Awake()
        {
            //Singleton Pattern, allows it to persist across scenes
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

    
    void Update()
    {
        //if Score is greater than highScore make highScore score
        if(score > highScore[0])
        {
            highScore[0] = score;
        }
    }
    
}