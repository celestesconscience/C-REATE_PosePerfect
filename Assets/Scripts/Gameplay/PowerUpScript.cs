using UnityEngine;

// Handles power-up selection, collection animations,
// sound effects, and activation of gameplay effects.
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

    public GameObject bonus500CollectFX;
    public GameObject doublePointsCollectFX;
    public GameObject armorCollectFX;
    public GameObject heartCollectFX;

    // Position adjustment relative to the player.
    public Vector3 collectionEffectOffset = Vector3.zero;


    // ==================== POWER-UP SELECTION ====================

    [Header("Power-Up Selection")]

    public int powerUpRandom;
    public int powerUpType;


    // ==================== SPAWN PROBABILITIES ====================

    [Header("Power-Up Spawn Chances")]

    [Min(0)] public int bonusPointsChance = 30;
    [Min(0)] public int doublePointsChance = 25;
    [Min(0)] public int armorChance = 25;
    [Min(0)] public int heartChance = 20;


    // ==================== TESTING SETTINGS ====================

    [Header("Testing Settings")]

    // Forces every spawned power-up to be a heart.
    // Disable this during regular gameplay.
    public bool forceHeartForTesting = false;

    // Prevent duplicate collection.
    private bool collected = false;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Randomly select the power-up type.
        SelectPowerUp();

        // Display the matching sprite.
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

        // Randomly select a value within the total weight.
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

        // Match collectible animation speed to wall speed.
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

        // Do not collect after the game has ended.
        if (GameManager.instance.livesLeft <= 0)
            return;

        collected = true;

        // Heart effects only play when a heart can be gained.
        bool canPlayAnimation =
            powerUpType != 3 ||
            GameManager.instance.livesLeft < 3;

        // Play the existing collection animation.
        if (canPlayAnimation)
        {
            PlayCollectionAnimation(player.transform);
        }

        // Apply the selected power-up effect.
        ActivatePowerUp(player);

        // Play the matching collection sound.
        // Skip the heart sound if the player already has 3 hearts.
        if (canPlayAnimation)
        {
            player.PlayPowerUpCollectionSound(powerUpType);
        }

        // Clear the occupied power-up lane.
        GameManager.instance.powerUpLocation = 10;

        // Remove the collected power-up.
        Destroy(gameObject);
    }


    // ==================== COLLECTION ANIMATIONS ====================

    private void PlayCollectionAnimation(Transform player)
    {
        GameObject effectPrefab = null;

        // Select the correct collection animation.
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

        // Skip if no animation prefab is assigned.
        if (effectPrefab == null)
            return;

        // Spawn at the player's position without a parent.
        GameObject effectInstance = Instantiate(
            effectPrefab,
            player.position + collectionEffectOffset,
            Quaternion.identity
        );

        // Preserve its original world size while
        // attaching it to the player.
        effectInstance.transform.SetParent(player, true);
    }


    // ==================== POWER-UP EFFECTS ====================

    private void ActivatePowerUp(PlayerScript player)
    {
        GameManager manager = GameManager.instance;

        switch (powerUpType)
        {
            // ==================== +500 POINTS ====================

            case 0:

                // Award exactly 500 bonus points.
                manager.score += 500f;

                // Show +500 feedback beside the score HUD.
                if (player != null)
                {
                    player.ShowBonus500();
                }

                break;


            // ==================== x2 POINTS ====================

            case 1:

                // Stop the previous countdown if active.
                manager.StopCoroutine("activateDoublePoints");

                // Restore the full duration.
                manager.doublePointsTimeLeft =
                    manager.doublePointsTimeStart;

                // Activate the multiplier.
                manager.doublePoints = true;

                // Start a fresh countdown.
                manager.StartCoroutine("activateDoublePoints");

                break;


            // ==================== INVINCIBILITY / ARMOR ====================

            case 2:

                // Stop the previous armor countdown.
                manager.StopCoroutine("activateArmor");

                // Restore the full duration.
                manager.armorTimeLeft =
                    manager.armorTimeStart;

                // Activate the shield.
                manager.armor = true;

                // Start a fresh countdown.
                manager.StartCoroutine("activateArmor");

                break;


            // ==================== +1 HEART ====================

            case 3:

                // Restore one heart, maximum 3.
                manager.livesLeft = Mathf.Min(
                    manager.livesLeft + 1,
                    3
                );

                break;
        }
    }
}
