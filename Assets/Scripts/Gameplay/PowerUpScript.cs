using UnityEngine;

// Handles power-up selection, collection animations,
// and activation of gameplay effects.
public class PowerUpScript : MonoBehaviour
{
    // ==================== POWER-UP SPRITES ====================

    // Power-up IDs:
    // 0 = +500 Points
    // 1 = x2 Points
    // 2 = Invincibility / Armor
    // 3 = +1 Heart

    public Sprite[] powerUps = new Sprite[4];

    private SpriteRenderer spriteRenderer;
    private Animator anim;


    // ==================== COLLECTION ANIMATIONS ====================

    [Header("Power-Up Collection Animations")]

    // Animation prefabs played when each power-up is collected.
    public GameObject bonus500CollectFX;
    public GameObject doublePointsCollectFX;
    public GameObject armorCollectFX;
    public GameObject heartCollectFX;

    // Position adjustment relative to the player.
    public Vector3 collectionEffectOffset = Vector3.zero;


    // ==================== POWER-UP SELECTION ====================

    [Header("Power-Up Selection")]

    // Random value used to determine the power-up type.
    public int powerUpRandom;

    // Selected power-up ID (0–3).
    public int powerUpType;


    // ==================== SPAWN PROBABILITIES ====================

    [Header("Power-Up Spawn Chances")]

    // Relative selection weights for each power-up.
    // When the total is 100, these equal percentages.

    [Min(0)] public int bonusPointsChance = 30;
    [Min(0)] public int doublePointsChance = 25;
    [Min(0)] public int armorChance = 25;
    [Min(0)] public int heartChance = 20;


    // ==================== TESTING SETTINGS ====================

    [Header("Testing Settings")]

    // Forces every spawned power-up to be a heart.
    // Disable this during regular gameplay.
    public bool forceHeartForTesting = false;

    // Prevents the same power-up from being collected twice.
    private bool collected = false;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Randomly select the power-up type.
        SelectPowerUp();

        // Display the sprite matching the selected type.
        if (spriteRenderer != null &&
            powerUpType >= 0 &&
            powerUpType < powerUps.Length)
        {
            spriteRenderer.sprite = powerUps[powerUpType];
        }
    }


    // ==================== POWER-UP SELECTION ====================

    private void SelectPowerUp()
    {
        // Testing option: always select a heart.
        if (forceHeartForTesting)
        {
            powerUpType = 3;
            powerUpRandom = -1;
            return;
        }

        // Calculate the total selection weight.
        int totalChance =
            bonusPointsChance +
            doublePointsChance +
            armorChance +
            heartChance;

        // Default to a heart if all weights are zero.
        if (totalChance <= 0)
        {
            powerUpType = 3;
            return;
        }

        // Select a random value within the total weight.
        powerUpRandom = Random.Range(0, totalChance);

        // +500 Points
        if (powerUpRandom < bonusPointsChance)
        {
            powerUpType = 0;
        }

        // x2 Points
        else if (powerUpRandom <
                 bonusPointsChance + doublePointsChance)
        {
            powerUpType = 1;
        }

        // Invincibility / Armor
        else if (powerUpRandom <
                 bonusPointsChance +
                 doublePointsChance +
                 armorChance)
        {
            powerUpType = 2;
        }

        // +1 Heart
        else
        {
            powerUpType = 3;
        }
    }


    // ==================== POWER-UP MOVEMENT ====================

    void Update()
    {
        if (anim == null || GameManager.instance == null)
            return;

        // Match the collectible's animation speed to wall speed.
        anim.SetFloat(
            "Speed",
            GameManager.instance.wallSpeed
        );
    }


    // ==================== POWER-UP COLLECTION ====================

    void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent duplicate collection.
        if (collected)
            return;

        // Only the player can collect power-ups.
        PlayerScript player =
            other.GetComponentInParent<PlayerScript>();

        if (player == null || GameManager.instance == null)
            return;

        collected = true;

        // A heart animation should only play if the player
        // has fewer than the maximum of three hearts.
        bool canPlayAnimation =
            powerUpType != 3 ||
            GameManager.instance.livesLeft < 3;

        if (canPlayAnimation)
        {
            PlayCollectionAnimation(player.transform);
        }

        // Apply the selected power-up effect.
        ActivatePowerUp();

        // Clear the occupied power-up lane.
        GameManager.instance.powerUpLocation = 10;

        // Remove the collected power-up.
        Destroy(gameObject);
    }


    // ==================== COLLECTION ANIMATIONS ====================

    private void PlayCollectionAnimation(Transform player)
    {
        GameObject effectPrefab = null;

        // Choose the correct collection animation.
        switch (powerUpType)
        {
            case 0:
                effectPrefab = bonus500CollectFX;
                break;

            case 1:
                effectPrefab = doublePointsCollectFX;
                break;

            case 2:
                effectPrefab = armorCollectFX;
                break;

            case 3:
                effectPrefab = heartCollectFX;
                break;
        }

        // Skip animation if no prefab is assigned.
        if (effectPrefab == null)
            return;

        // Position the effect relative to the player.
        Vector3 effectPosition =
            player.position + collectionEffectOffset;

        // Spawn the animation independently so it can finish
        // after the collected power-up has been destroyed.
        Instantiate(
            effectPrefab,
            effectPosition,
            Quaternion.identity
        );
    }


    // ==================== POWER-UP EFFECTS ====================

    private void ActivatePowerUp()
    {
        GameManager manager = GameManager.instance;

        switch (powerUpType)
        {
            // ==================== +500 POINTS ====================

            case 0:

                // Immediately award 500 bonus points.
                manager.score += 500f;

                break;


            // ==================== x2 POINTS ====================

            case 1:

                // Stop any existing double-points countdown.
                // Prevents an older timer from ending a refreshed effect.
                manager.StopCoroutine("activateDoublePoints");

                // Restore the full duration.
                manager.doublePointsTimeLeft =
                    manager.doublePointsTimeStart;

                // Activate the score multiplier.
                manager.doublePoints = true;

                // Start a new countdown.
                manager.StartCoroutine("activateDoublePoints");

                break;


            // ==================== INVINCIBILITY / ARMOR ====================

            case 2:

                // Stop any existing armor countdown.
                // Prevents an older timer from removing new armor.
                manager.StopCoroutine("activateArmor");

                // Restore the full armor duration.
                manager.armorTimeLeft =
                    manager.armorTimeStart;

                // Activate the shield.
                manager.armor = true;

                // Start a new countdown.
                // GameManager sets armorAlreadyOn to true.
                manager.StartCoroutine("activateArmor");

                break;


            // ==================== +1 HEART ====================

            case 3:

                // Restore one heart without exceeding three lives.
                manager.livesLeft = Mathf.Min(
                    manager.livesLeft + 1,
                    3
                );

                break;
        }
    }
}