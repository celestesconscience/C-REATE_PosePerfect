using UnityEngine;
using TMPro;

// Displays the five highest saved scores.
public class HighScoreManager : MonoBehaviour
{
    // ==================== HIGH SCORE DISPLAY ====================

    public TextMeshProUGUI highScoreList;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        UpdateHighScoreList();
    }


    // ==================== DISPLAY SAVED SCORES ====================

    private void UpdateHighScoreList()
    {
        if (GameManager.instance == null || highScoreList == null)
            return;

        float[] scores = GameManager.instance.highScore;

        // Display the five saved scores in descending order.
        highScoreList.text =
            "1.     " + scores[0].ToString("N0") + "\n\n" +
            "2.     " + scores[1].ToString("N0") + "\n\n" +
            "3.     " + scores[2].ToString("N0") + "\n\n" +
            "4.     " + scores[3].ToString("N0") + "\n\n" +
            "5.     " + scores[4].ToString("N0");
    }
}
