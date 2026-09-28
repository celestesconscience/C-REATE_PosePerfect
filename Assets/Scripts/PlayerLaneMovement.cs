using UnityEngine;
using UnityEngine.InputSystem;

// Didn't chang/touch your code just cleaned up comments and renamed file for confusion! It works!
public class PlayerScript : MonoBehaviour
{
    // Button input variables
    InputAction buttonPress;
    public bool pressed;

    // Player movement variables
    public float[] Locations = new float[3];
    public int currentLocation;
    public int changeAMT = 1;

    // Start is called once when the game starts
    void Start()
    {
        // Finds the "Button" input action
        buttonPress = InputSystem.actions.FindAction("Button");

        // Starts the player in the middle location
        currentLocation = 1;

        // Moves the player to the starting location
        transform.position = new Vector2(Locations[currentLocation], -1.8f);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the button was pressed this frame
        // pressed = buttonPress.IsPressed();
        pressed = buttonPress.WasPressedThisFrame();;

        // Prints when the button is pressed
        if(pressed == true)
        {
            print("PRESSING");
        }

        // Moves the player between the locations when the button is pressed
        if(pressed == true)
        {
            // Moves to the next location
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
            transform.position = new Vector2(Locations[currentLocation], -1.8f);
        }
    }
}