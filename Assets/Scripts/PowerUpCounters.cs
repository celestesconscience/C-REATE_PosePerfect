using UnityEngine;
using TMPro;

// Controls the expanding power-up status panel and heart HUD.
public class PowerUpCounters : MonoBehaviour
{
    // ==================== POWER-UP STATUS PANEL ====================

    [Header("Power-Up Status Panel")]

    public GameObject powerUpStatusPanel;

    public GameObject doublePointsRow;
    public GameObject armorRow;

    public TextMeshProUGUI doublePointsCounterText;
    public TextMeshProUGUI armorCounterText;


    // ==================== HEART HUD ====================

    [Header("Heart HUD")]

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        if (doublePointsRow != null)
            doublePointsRow.SetActive(false);

        if (armorRow != null)
            armorRow.SetActive(false);

        if (powerUpStatusPanel != null)
            powerUpStatusPanel.SetActive(false);
    }


    // ==================== UPDATE ====================

    void Update()
    {
        if (GameManager.instance == null)
            return;

        UpdatePowerUpPanel();
        UpdateHeartDisplay();
    }


    // ==================== DYNAMIC POWER-UP PANEL ====================

    private void UpdatePowerUpPanel()
    {
        GameManager manager = GameManager.instance;

        // Check whether each timed power-up is active.
        bool doublePointsActive =
            manager.doublePoints &&
            manager.doublePointsAlreadyOn &&
            manager.doublePointsTimeLeft > 0;

        bool armorActive =
            manager.armor &&
            manager.armorAlreadyOn &&
            manager.armorTimeLeft > 0;


        // ==================== DOUBLE POINTS ====================

        if (doublePointsCounterText != null)
        {
            doublePointsCounterText.text =
                "x2 Points    " +
                manager.doublePointsTimeLeft + "s";
        }

        if (doublePointsRow != null &&
            doublePointsRow.activeSelf != doublePointsActive)
        {
            doublePointsRow.SetActive(doublePointsActive);
        }


        // ==================== ARMOR ====================

        if (armorCounterText != null)
        {
            armorCounterText.text =
                "Armor Up    " +
                manager.armorTimeLeft + "s";
        }

        if (armorRow != null &&
            armorRow.activeSelf != armorActive)
        {
            armorRow.SetActive(armorActive);
        }


        // ==================== PANEL VISIBILITY ====================

        bool panelActive = doublePointsActive || armorActive;

        if (powerUpStatusPanel != null &&
            powerUpStatusPanel.activeSelf != panelActive)
        {
            powerUpStatusPanel.SetActive(panelActive);
        }
    }


    // ==================== HEART DISPLAY ====================

    private void UpdateHeartDisplay()
    {
        float lives = GameManager.instance.livesLeft;

        if (heart1 != null)
            heart1.SetActive(lives >= 1);

        if (heart2 != null)
            heart2.SetActive(lives >= 2);

        if (heart3 != null)
            heart3.SetActive(lives >= 3);
    }
}
