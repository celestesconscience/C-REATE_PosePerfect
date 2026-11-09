using UnityEngine;
using TMPro;

public class PowerUpCounters : MonoBehaviour
{

    public TextMeshProUGUI doublePointsCounterText, armorCounterText;

    public GameObject heart1, heart2, heart3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doublePointsCounterText.text = "";
        armorCounterText.text = "";
    }

    // Update is called once per frame
    void Update()
    {   
        if(GameManager.instance.doublePointsTimeLeft > 0)
            doublePointsCounterText.text = "2x Points [" + GameManager.instance.doublePointsTimeLeft + "]";
        else
            doublePointsCounterText.text = "";

        if(GameManager.instance.armorTimeLeft > 0)
            armorCounterText.text = "Armor [" + GameManager.instance.armorTimeLeft + "]";
        else
            armorCounterText.text = "";




        if(GameManager.instance.livesLeft == 3)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);
        }
        else if(GameManager.instance.livesLeft == 2)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(false);
        }
        else if(GameManager.instance.livesLeft == 1)
        {
            heart1.SetActive(true);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }
        else
        {
            heart1.SetActive(false);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }


    }
}
