using UnityEngine;
using UnityEngine.InputSystem;

public class IndicatorScript : MonoBehaviour// BASICALLY JUST COPYING CODE FROM PLAYERSCRIPT
{
    // Button Input Variables
    InputAction buttonPress;
    private bool pressed;

    // Indicator Location Variables
    public GameObject[] Location; // Array of possible locations for the indicator
    private int startingPose = 0; // The initial location of the indicator

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPress = InputSystem.actions.FindAction("TapButton"); // Finds the Input Action for tapping the button
        
        transform.localPosition = new Vector2(Location[startingPose].transform.localPosition.x, transform.localPosition.y); // Moves the indicator to the starting location
    }

    // Update is called once per frame
    void Update()
    {   
        pressed = buttonPress.WasPerformedThisFrame(); // Checks if the button was pressed this frame
        
        // Changes the indicator's location when the button is pressed
        if(pressed == true)
        {
            startingPose++; // Increments the starting location of the indicator
            if(startingPose == Location.Length) // Checks if the indicator has gone past the last location
            {
                startingPose = 0; // Resets the starting location to the first location
            }


        // Moves the indicator to the new location
        transform.localPosition = new Vector2(Location[startingPose].transform.localPosition.x, transform.localPosition.y); // Updates the position of the indicator based on the new starting location
    }
    }
}

