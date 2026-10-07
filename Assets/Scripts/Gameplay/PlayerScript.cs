using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    // Button Input Variables
    InputAction buttonPress, buttonHold; // References to the input actions for tapping and holding the button
    private bool pressed, held; // Stores the state of the button press and hold actions

    // Player Changing Poses Variables;
    public Sprite[] poses; // Leave 0 empty; array of poses for the player character
    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer component of the player character
    private int startingPose = 1; // The initial pose of the player character

    // Player Movement Variables
    public float[] locations = new float[3]; // In inspector you can change where you want the player to be
    public int currentLocation; // Change from 0-2 in inspector to start left(0) middle(1) right(2)
    private int changeAMT = 1; // The amount by which the player's location changes when moving lanes

    //TEMPORARY
    public GameObject winObject, failObject;

    // Start is called once when the game starts
    void Start()
    {
        // Finds the Input Actions for tapping and holding the button
        buttonPress = InputSystem.actions.FindAction("TapButton"); 
        buttonHold = InputSystem.actions.FindAction("HoldButton");

        // Gets the spriteRenderer from the Inspector and Changes Pose to Starting Pose
        spriteRenderer = GetComponent<SpriteRenderer>(); // Gets the sprite renderer component from the player character
        spriteRenderer.sprite = poses[startingPose]; // Sets the initial pose of the player character
        gameObject.tag = "Pose_1"; // Sets the initial tag of the player character based on the starting pose

        // Moves the Player to the Starting Location
        transform.position = new Vector2(locations[currentLocation], transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the button was pressed
        pressed = buttonPress.WasPerformedThisFrame();
        held = buttonHold.WasPerformedThisFrame();

        // Debug to see if press and hold is working as intended
        if(pressed == true)
        {
            print("tap");
        }
        if(held == true)
        {
            print("held");
        }

        // Changes the players poses when the button is pressed
        if(pressed == true)
        {
            poseChange();
        }

        // Moves the player when the button is held
        if(held == true)
        {
            laneMovement();
        }
    }

    void OnTriggerEnter2D(Collider2D other)//detects if wall tag is the same as player
    {
        if (other.CompareTag(gameObject.tag))
        {   
            print("SAME POSE");//TEST
            winObject.SetActive(true);
        }
        else
        {
            print("DIFFERENT POSE");
            failObject.SetActive(true);
        }

    }

      void OnTriggerExit2D(Collider2D other)//detects if wall exits the player
    {
        winObject.SetActive(false);
        failObject.SetActive(false);
    }

    // SCRIPT FOR POSE CHANGING
    private void poseChange() // Method for changing the player's pose when the button is pressed
    {
        startingPose++; // Increments the starting pose of the player character (e.g., startingPose = 1 ++ -> startingPose = 2)
        if(startingPose == poses.Length) // Checks if the player has gone past the last pose in array
        {
            startingPose = 1; // Resets the starting pose to the first pose in the array (index 1, not 0)
        }

        spriteRenderer.sprite = poses[startingPose]; // Sets the sprite of the player character to the new pose

        switch (startingPose) // What value does this variable currently have?
        {
            case 1: // If the starting pose is 1
                gameObject.tag = "Pose_1"; // Sets the tag of the player character to "Pose_1"
                break;
            case 2: // If the starting pose is 2
                gameObject.tag = "Pose_2"; // Sets the tag of the player character to "Pose_2"
                break;
            case 3: // If the starting pose is 3
                gameObject.tag = "Pose_3"; // Sets the tag of the player character to "Pose_3"
                break;
            case 4: // If the starting pose is 4
                gameObject.tag = "Pose_4"; // Sets the tag of the player character to "Pose_4"
                break;
            case 5: // If the starting pose is 5
                gameObject.tag = "Pose_5"; // Sets the tag of the player character to "Pose_5"
                break;
        }

        // End of pose change logic
    }

    // SCRIPT FOR LANE MOVEMENT
    private void laneMovement() // Method for moving the player character between lanes
    {
        currentLocation+= changeAMT; // Updates the current location of the player character based on the change amount

            // Checks if the player goes past the last location
            if(currentLocation== locations.Length)
            {
                // Changes direction from positive to negative
                changeAMT = -changeAMT;

                // Moves the location back into the valid range
                currentLocation+= changeAMT;
                currentLocation+= changeAMT;
            }

            // Checks if the player goes past the first location
            else if(currentLocation == -1)
            {
                // Changes direction from negative to positive
                changeAMT = -changeAMT;

                // Moves the location back into the valid range
                currentLocation+= changeAMT;
                currentLocation+= changeAMT;
            }

            // Actually moves the player character game object to the new location based on the current location index
            transform.position = new Vector2(locations[currentLocation], transform.position.y);
    }
}