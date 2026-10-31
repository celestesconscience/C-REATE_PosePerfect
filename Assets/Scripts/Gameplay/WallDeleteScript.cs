using UnityEngine;

// Automatically destroys wall GameObjects after a set amount of time
// to prevent unused walls from accumulating in the scene.
public class WallDeleteScript : MonoBehaviour
{
    // ==================== WALL CLEANUP ====================

    void Start()
    {
        // Destroy this wall 16 seconds after it is created.
        // Prevents off-screen walls from remaining in memory
        // and unnecessarily affecting game performance.
        Destroy(gameObject, 16f);
    }
}