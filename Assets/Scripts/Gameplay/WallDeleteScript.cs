using UnityEngine;
using System.Collections;

// Automatically destroys wall GameObjects after a set amount of time
// to prevent unused walls from accumulating in the scene.
// WORKS FOR BOTH WALL AND POWERUP
public class WallDeleteScript : MonoBehaviour
{
    // ==================== WALL CLEANUP ====================

    // void Start()
    // {
    //     StartCoroutine("DeleteObject");
    // }

    // IEnumerator DeleteObject()
    // {
    //     // Destroy this object 16 seconds after it is created.
    //     // Prevents off-screen walls from remaining in memory
    //     // and unnecessarily affecting game performance.
    //     yield return new WaitForSeconds(GameManager.instance.wallSpawnSpeed * 1.5f);
    //     Destroy(gameObject);
    // }

    void Update()
    {
        if(transform.GetChild(0).localPosition.y < -7)
        {
            Destroy(gameObject);
        }
    }
}