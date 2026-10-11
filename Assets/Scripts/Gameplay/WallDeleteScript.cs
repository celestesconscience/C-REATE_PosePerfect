using UnityEngine;

public class WallDeleteScript : MonoBehaviour
{
   void Start() // Helps game remain consistent across different PC specs
    {
        Destroy(gameObject, 16f); // Destroys itself after x seconds to prevent lag
    }
}
