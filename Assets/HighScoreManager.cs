using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{

     public TextMeshProUGUI highScoreList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        System.Array.Sort(GameManager.instance.highScore);
        System.Array.Reverse(GameManager.instance.highScore);
    }

    // Update is called once per frame
    void Update()
    {
        highScoreList.text = 
        "1.     " + GameManager.instance.highScore[0].ToString("N0") + "\n\n"+
        "2.     " + GameManager.instance.highScore[1].ToString("N0") + "\n\n" + 
        "3.     " + GameManager.instance.highScore[2].ToString("N0") + "\n\n" + 
        "4.     " + GameManager.instance.highScore[3].ToString("N0") + "\n\n" + 
        "5.     " + GameManager.instance.highScore[4].ToString("N0") + "\n\n";
    }
}
