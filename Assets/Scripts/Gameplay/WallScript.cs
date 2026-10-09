using UnityEngine;

public class WallScript : MonoBehaviour
{
    

    public Sprite[] walls;//all possible walls
    private SpriteRenderer spriteRenderer;
    public int wallType;//what wall it should be

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallType = Random.Range(0,5);//returns a number from 0-4
    }

    // Update is called once per frame
    void Update()
    {
        
        spriteRenderer.sprite = walls[wallType];


        switch (wallType)//makes the sprite into whatever walltype it is
        {
            case 0:  
                gameObject.tag = "Pose_1";
                break;
            case 1: 
                gameObject.tag = "Pose_2";
                break;
            case 2: 
                gameObject.tag = "Pose_3";
                break;
            case 3: 
                gameObject.tag = "Pose_4";
                break;
            case 4: 
                gameObject.tag = "Pose_5";
                break;
        }
    }
}
