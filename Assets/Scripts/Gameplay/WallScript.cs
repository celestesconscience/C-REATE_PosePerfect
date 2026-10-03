using UnityEngine;

public class WallScript : MonoBehaviour
{
    // Wall Position Variables
    private float wallStartPosition = -1.84f; // The position where the wall should start or spawn in the game
    private float wallMatchPosition = -3.14f; // The position where the wall should collide with the player
    private float wallEndPosition = -6.99f; // The position where the wall should go off screen

    // Wall Scale Variables
    private float wallStartScale = 0.5f; // The initial scale of the wall when it spawns
    private float wallEndScale = 1.05f; // The final scale of the wall when it reaches the player

    // Wall Speed Variables
    public float wallSpeed = 0.5f; // The speed at which the wall moves towards the player
    public float wallExitSpeed = 1.0f; // The speed at which the wall moves off screen after reaching the match position

    // Wall Timers
    public float wallResultPauseDuration = 0.5f;
    private float wallResultPauseTimer = 0.0f;

    // Wall State Variables
    private bool reachedMatchPosition =  false; // A flag to indicate if the wall has reached the match position
    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer component of the wall
    public int requiredPose = 1; // The required pose for the player to match with the wall

    public GameObject player; // Reference to the player GameObject

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ------- WALL SETUP -------
        // Gets the spriteRenderer from the GameObject to change Layering Order
        spriteRenderer = GetComponent<SpriteRenderer>(); // Gets the sprite renderer component from the wall GameObject
        spriteRenderer.sortingOrder = 1; // Sets the sorting order of the wall to be behind the player character
        
        // Put Wall in starting scale and y
        transform.position = new Vector2(transform.position.x, wallStartPosition);
        transform.localScale = new Vector2(wallStartScale, wallStartScale);
    }

    // Update is called once per frame
    void Update()
    {
        // -------- WALL MOVEMENT --------
        float newWallPosition; // Variable to store the new position of the wall    
        float newWallScale; // Variable to store the new scale of the wall
        float newWallAlpha; // Variable to store the new alpha of the wall
        float wallProgress; // Variable to store wall's moving and scale progress from start pos to match pos
        float wallExitProgress; // Variable to store wall's progress from the match pos to the end pos for fade

        // -------- MATCH POSITION CHECK --------
       if(reachedMatchPosition == false) // If the wall didn't reach the mid point
        {
            newWallPosition = Mathf.MoveTowards(
                transform.position.y,
                wallMatchPosition, // then move wall towards match position
                wallSpeed * Time.deltaTime
                );

            transform.position = new Vector2(transform.position.x, newWallPosition); // actually moves to new pos
        }
        else // If wall did reach the mid point, then move wall towards end position
        {
            wallResultPauseTimer += Time.deltaTime; // Increment the wall result pause timer by the time elapsed since the last frame

            if(wallResultPauseTimer >= wallResultPauseDuration) // Also, wall is timer reached pause duration
            {
            newWallPosition = Mathf.MoveTowards(
                transform.position.y,
                wallEndPosition, // then move wall towards end position
                wallExitSpeed * Time.deltaTime
                );

            transform.position = new Vector2(transform.position.x, newWallPosition); // actually moves to new pos
            }
        }

        // -------- WALL MATCH POSITION, CHANGE LAYERING ORDER --------
        if(transform.position.y <= wallMatchPosition)
        {
            reachedMatchPosition = true;
            spriteRenderer.sortingOrder = 3; // Sets the sorting order of the wall to be in front of the player character
        }

        // -------- WALL EXIT POSITION, DESTROY WALL --------
        if(transform.position.y <= wallEndPosition)
        {
            Destroy(gameObject);
        }

        // Calculates the wall's exit progress from the match position to the end position
        wallExitProgress = Mathf.InverseLerp(wallMatchPosition, wallEndPosition, transform.position.y); // Give the walls progress

        // -------- WALL SCALING + TRANSPARENCY --------
        // Calculates the progress of the wall's movement from its start position to its end position,
        // Uses that progress to interpolate the scale of the wall between its start and end scales
        wallProgress = Mathf.InverseLerp(wallStartPosition, wallMatchPosition, transform.position.y); // Give the walls progress 

        // Interpolates the scale of the wall based on the calculated progress, 
        // Stores the new scale in the newWallScale variable (so the wall grows equally as it approaches)
        newWallScale = Mathf.Lerp(wallStartScale, wallEndScale, wallProgress); // Given the progress, gives what the wall's scale should currently be

        // Interpolates the alpha of the wall based on the calculated exit progress,
        // Stores the new alpha in the newWallAlpha variable (so the wall fades out as it leaves)
        newWallAlpha = Mathf.Lerp(1.0f, 0.0f, wallExitProgress); // Given the progress, gives what the wall's alpha should currently be
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, newWallAlpha); // Sets the alpha of the wall's color to the new alpha value

        // Scales the wall up as it approaches the player with the new wall scale
        transform.localScale = new Vector2(newWallScale, newWallScale); 
    }
}
