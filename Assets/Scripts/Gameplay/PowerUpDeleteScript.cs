using System.Collections;
using UnityEngine;

public class PowerUpDeleteScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine("DeleteObject");
    }

    // IEnumerator DeleteObject()
    // {
    //     yield return new WaitForSeconds(GameManager.instance.wallSpawnSpeed);
    //     Destroy(gameObject);
    //     GameManager.instance.powerUpLocation = 10;
    // }

    void Update()
    {   
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
        else if(transform.GetChild(0).localPosition.y < -7)
        {
            Destroy(gameObject);
            GameManager.instance.powerUpLocation = 10;
        }
    }
}
