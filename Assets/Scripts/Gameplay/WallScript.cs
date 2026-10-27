using UnityEngine;

public class WallScript : MonoBehaviour
{
    public Sprite[] walls;//all possible walls
    public Sprite[] currentWalls = new Sprite [5];
    private SpriteRenderer spriteRenderer;
    public int wallType;//what wall it should be
    private Animator anim;//gets animator component to control wall speed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   

        spriteRenderer = GetComponent<SpriteRenderer>();
        wallType = Random.Range(0,5);//returns a number from 0-4
        anim = GetComponent<Animator>();


        //POSE CHANGES
        currentWalls[0] = walls[GameManager.instance.randomWallNums[0]];
        currentWalls[1] = walls[GameManager.instance.randomWallNums[1]];
        currentWalls[2] = walls[GameManager.instance.randomWallNums[2]];
        currentWalls[3] = walls[GameManager.instance.randomWallNums[3]];
        currentWalls[4] = walls[GameManager.instance.randomWallNums[4]];

        spriteRenderer.sprite = currentWalls[wallType];

        //COLOR CHANGES
        switch (GameManager.instance.randomColor)
        {
            // case 0:
            //     spriteRenderer.color = new Color32(39, 136, 217, 0);//arcade blue
            //     break;
            case 1:
                spriteRenderer.color = new Color32(255, 92, 138, 0);//punchy pink
                break;
            case 2:
                spriteRenderer.color = new Color32(255, 212, 59, 0);//game show yellow
                break;
            case 3:
                spriteRenderer.color = new Color32(255, 122, 48, 0);//orange lol
                break;
            case 4:
                spriteRenderer.color = new Color32(82, 201, 122, 0);//playful green
                break;
            case 5:
                spriteRenderer.color = new Color32(155, 109, 227, 0);//purple
                break;
            // case 6:
            //     spriteRenderer.color = new Color32(240, 68, 90, 0);//red
            //     break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", GameManager.instance.wallSpeed);//changes speed of wall

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
