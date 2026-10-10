using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI highScoreText, scoreText;
    public TextMeshProUGUI gameOverHighScoreText, gameOverScoreText;

    public float highScore, score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     //Gets the high score from the gameManager and puts it in highScoreText
    //     highScore = GameManager.instance.highScore[0];

    //     //Gets the score from the gameManager and puts it in highScoreText
    //     score = GameManager.instance.score;
    // }

    // Update is called once per frame
    void Update()
    {
        //Gets high score from gameManager and puts it in highScoreText
        highScore = GameManager.instance.highScore[0];
        highScoreText.text = "HI-SCORE: " + highScore.ToString("N0");
        gameOverHighScoreText.text = "HI-SCORE: " + highScore.ToString("N0");

        //Gets score from gameManager and puts it in scoreText
        score = GameManager.instance.score;
        scoreText.text = "SCORE: " + score.ToString("N0");
        gameOverScoreText.text = "SCORE: " + score.ToString("N0");

    }
}
