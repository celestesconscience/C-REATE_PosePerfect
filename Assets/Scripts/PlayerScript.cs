using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    // Button input variables
    InputAction buttonPress, buttonHold;
    private bool pressed, held;

    // Player changing poses variables;
    public Sprite[] poses;
    private SpriteRenderer spriteRenderer;
    private int startingPose = 1;// leave 0 empty

    // Player movement variables
    public float[] Locations = new float[3];//in inspector you can change where you want to player to be
    public int currentLocation;//change from 0-2 in inspector to start left(0) middle(1) right(2)
    private int changeAMT = 1;

    // Start is called once when the game starts
    void Start()
    {
        // Finds the input actions
        buttonPress = InputSystem.actions.FindAction("TapButton");
        buttonHold = InputSystem.actions.FindAction("HoldButton");

        // Gets the spriteRenderer from the inspector and changes pose to starting pose
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = poses[startingPose];
        gameObject.tag = "Pose_1";

        // Moves the player to the starting location
        transform.position = new Vector2(Locations[currentLocation], transform.position.y);
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

    //SCRIPT FOR POSE CHANGING
    private void poseChange()
    {
        startingPose++;
        if(startingPose == poses.Length)
        {
            startingPose = 1;
        }

        spriteRenderer.sprite = poses[startingPose];

        switch (startingPose)
        {
            case 1:
                gameObject.tag = "Pose_1";
                break;
            case 2:
                gameObject.tag = "Pose_2";
                break;
            case 3:
                gameObject.tag = "Pose_3";
                break;
            case 4:
                gameObject.tag = "Pose_4";
                break;
            case 5:
                gameObject.tag = "Pose_5";
                break;
        }

     
    }

    //SCRIPT FOR LANE MOVEMENT
    private void laneMovement()
    {
        currentLocation+= changeAMT;

            // Checks if the player goes past the last location
            if(currentLocation== Locations.Length)
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

            // Moves the player to the new location
            transform.position = new Vector2(Locations[currentLocation], transform.position.y);
    }
}