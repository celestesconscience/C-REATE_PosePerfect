using UnityEngine;
using TMPro;
using System;

// Updates the score and high score displayed in the gameplay HUD
// and Game Over screen using values stored in the GameManager.
public class ScoreController : MonoBehaviour
{
    // ==================== GAMEPLAY SCORE UI ====================

    // Text elements displaying the high score and current score during gameplay.
    public TextMeshProUGUI highScoreText, scoreText;


    // ==================== GAME OVER SCORE UI ====================

    // Text elements displaying the final score and high score on the Game Over screen.
    public TextMeshProUGUI gameOverHighScoreText, gameOverScoreText;


    // ==================== SCORE VALUES ====================

    // Local copies of the current high score and player score.
    // These values are retrieved from the GameManager.
    public float highScore, score;


    // ==================== SCORE UI UPDATE ====================

    void Start()
    {
        GameManager.instance.currentHighScore = GameManager.instance.highScore[0];
    }

    void Update()
    {   
        GameManager.instance.highScore[5] = GameManager.instance.score;

        if (GameManager.instance.score > GameManager.instance.currentHighScore)
        {
            //GameManager.instance.highScore[6] = GameManager.instance.highScore[1];
            GameManager.instance.currentHighScore = score;
        }


        // Retrieve the highest recorded score from the GameManager.
        if(GameManager.instance.currentHighScore > GameManager.instance.highScore[0])
        {
            highScore = GameManager.instance.currentHighScore;
            GameManager.instance.highScore[5] = GameManager.instance.currentHighScore;
        }
        else
        {
            highScore = GameManager.instance.highScore[0];
        }
        

        // Update the high score displayed during gameplay and on Game Over.
        // "N0" formats the number with separators and no decimal places.
        highScoreText.text = "HI-SCORE: " + highScore.ToString("N0");
        gameOverHighScoreText.text = "HI-SCORE: " + highScore.ToString("N0");


        // Retrieve the player's current score from the GameManager.
        score = GameManager.instance.score;

        // Update the current score displayed during gameplay and on Game Over.
        scoreText.text = "SCORE: " + score.ToString("N0");
        gameOverScoreText.text = "SCORE: " + score.ToString("N0");
    }
}
