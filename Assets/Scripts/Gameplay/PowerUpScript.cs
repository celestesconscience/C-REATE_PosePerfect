using UnityEngine;

public class PowerUpScript : MonoBehaviour
{   
    public Sprite[] powerUps = new Sprite[4];
    
    private SpriteRenderer spriteRenderer;

    public int powerUpRandom;

    public int powerUpType;

    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        powerUpRandom = Random.Range(0, 101);

        //probability of each determined by second num
        if(powerUpRandom <= 40)// +500 points
        {
            powerUpType = 0;
        }
        else if(powerUpRandom <= 60)// x2 points
        {
            powerUpType = 1;
        }
        else if(powerUpRandom <= 90)// temp armor
        {
            powerUpType = 2;
        }
        else if(powerUpRandom <= 100)// +1 health
        {
            powerUpType = 3;
        }

    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sprite = powerUps[powerUpType];

        anim.SetFloat("Speed", GameManager.instance.wallSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        //theres prob a better way of doing this 
        //player collects powerup
        if (other.CompareTag("Pose_1") || 
        other.CompareTag("Pose_2") ||
        other.CompareTag("Pose_3") ||
        other.CompareTag("Pose_4") || 
        other.CompareTag("Pose_5"))
        {
            if(powerUpType == 0)// +500 Points
            {
                GameManager.instance.score += 500f;
            }
            else if(powerUpType == 1)// x2 Points for 20s
            {
                GameManager.instance.doublePointsTimeLeft = GameManager.instance.doublePointsTimeStart;
                GameManager.instance.doublePoints = true;
                GameManager.instance.StartCoroutine("activateDoublePoints");

            }
            else if(powerUpType == 2)// armor for 30s
            {   
                GameManager.instance.armorTimeLeft = GameManager.instance.armorTimeStart;
                GameManager.instance.armor = true;
                GameManager.instance.StartCoroutine("activateArmor");


            }
            else if(powerUpType == 3)// +1 heart
            {
                if(GameManager.instance.livesLeft < 3)
                {
                    GameManager.instance.livesLeft ++;
                }
            }
            
            GameManager.instance.powerUpLocation = 10;
            Destroy(gameObject);
        }
    }

    }
