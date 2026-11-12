using UnityEngine;
using TMPro;

// Updates the gameplay and Game Over score displays.
public class ScoreController : MonoBehaviour
{
    // ==================== GAMEPLAY SCORE UI ====================

    public TextMeshProUGUI highScoreText, scoreText;


    // ==================== GAME OVER SCORE UI ====================

    public TextMeshProUGUI gameOverHighScoreText, gameOverScoreText;


    // ==================== SCORE VALUES ====================

    public float highScore, score;


    // ==================== SCORE UI UPDATE ====================

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.currentHighScore =
                GameManager.instance.highScore[0];
        }
    }

    void Update()
    {
        if (GameManager.instance == null)
            return;

        GameManager manager = GameManager.instance;

        // Retrieve the current score.
        score = manager.score;

        // Show the best saved score or current run,
        // whichever is higher.
        highScore = Mathf.Max(
            manager.highScore[0],
            score
        );

        manager.currentHighScore = highScore;

        // ==================== GAMEPLAY HUD ====================

        if (highScoreText != null)
        {
            highScoreText.text =
                "HI-SCORE: " + highScore.ToString("N0");
        }

        if (scoreText != null)
        {
            scoreText.text =
                "SCORE: " + score.ToString("N0");
        }


        // ==================== GAME OVER HUD ====================

        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text =
                "HI-SCORE: " + highScore.ToString("N0");
        }

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text =
                "SCORE: " + score.ToString("N0");
        }
    }
}
