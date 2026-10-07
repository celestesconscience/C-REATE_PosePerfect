using UnityEngine;

public class WallDeleteScript : MonoBehaviour
{
   void FixedUpdate()//helps game remain consistent across different pc specs.
    {
        Destroy(gameObject, 16f);//destroys itself after x seconds to prevent lag
    }
}
