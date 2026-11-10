using UnityEngine;

// Automatically removes a collection effect
// after its one-time animation finishes.
public class CollectionEffectCleanup : MonoBehaviour
{
    // ==================== EFFECT LIFETIME ====================

    [Header("Effect Settings")]

    // How long the animation lasts, in seconds.
    public float lifetime = 1.5f;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Remove the effect after its animation finishes.
        Destroy(gameObject, lifetime);
    }
}