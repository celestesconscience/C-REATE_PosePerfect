using UnityEngine;
using TMPro;

// Displays power-up timers and the player's remaining hearts.
public class PowerUpCounters : MonoBehaviour
{
    // ==================== POWER-UP STATUS PANEL ====================

    [Header("Power-Up Status Panel")]

    public GameObject powerUpStatusPanel;

    public TextMeshProUGUI doublePointsCounterText;
    public TextMeshProUGUI armorCounterText;

    // ==================== HEART DISPLAY ====================

    [Header("Heart HUD")]

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    // ==================== INITIALIZATION ====================

    void Start()
    {
        if (doublePointsCounterText != null)
            doublePointsCounterText.gameObject.SetActive(false);

        if (armorCounterText != null)
            armorCounterText.gameObject.SetActive(false);

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

        // Determine which effects are currently active.
        bool doublePointsActive =
            manager.doublePoints &&
            manager.doublePointsAlreadyOn &&
            manager.doublePointsTimeLeft > 0;

        bool armorActive =
            manager.armor &&
            manager.armorAlreadyOn &&
            manager.armorTimeLeft > 0;

        // ==================== DOUBLE POINTS ROW ====================

        if (doublePointsCounterText != null)
        {
            if (doublePointsActive)
            {
                doublePointsCounterText.text =
                    "2x Points     " +
                    manager.doublePointsTimeLeft + "s";
            }

            doublePointsCounterText.gameObject.SetActive(
                doublePointsActive
            );
        }

        // ==================== ARMOR ROW ====================

        if (armorCounterText != null)
        {
            if (armorActive)
            {
                armorCounterText.text =
                    "Armor     " +
                    manager.armorTimeLeft + "s";
            }

            armorCounterText.gameObject.SetActive(
                armorActive
            );
        }

        // ==================== PANEL VISIBILITY ====================

        // Show the panel if at least one timed effect is active.
        // Hide it completely when neither effect is active.
        if (powerUpStatusPanel != null)
        {
            bool showPanel =
                doublePointsActive || armorActive;

            if (powerUpStatusPanel.activeSelf != showPanel)
            {
                powerUpStatusPanel.SetActive(showPanel);
            }
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
