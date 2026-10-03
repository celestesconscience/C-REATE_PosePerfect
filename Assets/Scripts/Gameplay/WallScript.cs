using UnityEngine;

public class WallScript : MonoBehaviour
{
    // Wall Variables
    public float wallEndPosition = -3.42f; // The position where the wall should collide with the player
    public float wallStartPosition = -1.84f; // The position where the wall should start or spawn in the game
    public float wallSpeed = 5.0f; // The speed at which the wall moves towards the player
    public float wallStartScale = 0.5f; // The initial scale of the wall when it spawns
    public float wallEndScale = 1.05f; // The final scale of the wall when it reaches the player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector2(transform.position.x, wallStartPosition);
        transform.localScale = new Vector2(wallStartScale, wallStartScale);
    }

    // Update is called once per frame
    void Update()
    {
        // Variables to store the new position and scale of the wall + track progress
        float newWallPosition; // Variable to store the new position of the wall    
        float newWallScale; // Variable to store the new scale of the wall
        float wallProgress;

        // Moves the wall towards the player and scales it up as it approaches, stores the new position in the newWallPosition variable
        newWallPosition = Mathf.MoveTowards(
            transform.position.y,
            wallEndPosition,
            wallSpeed * Time.deltaTime
        );

        // Moves the wall towards the player with the new wall position 
        transform.position = new Vector2(transform.position.x, newWallPosition);

        // Calculates the progress of the wall's movement from its start position to its end position,
        // Uses that progress to interpolate the scale of the wall between its start and end scales
        wallProgress = Mathf.InverseLerp(wallStartPosition, wallEndPosition, transform.position.y); // Give the walls progress 

        // Interpolates the scale of the wall based on the calculated progress, 
        // Stores the new scale in the newWallScale variable (so the wall grows equally as it approaches)
        newWallScale = Mathf.Lerp(wallStartScale, wallEndScale, wallProgress); // Given the progress, gives what the wall's scale should currently be

        // Scales the wall up as it approaches the player with the new wall scale
        transform.localScale = new Vector2(newWallScale, newWallScale); 
    }
}
