using UnityEngine;
using UnityEngine.InputSystem;

public class IndicatorScript : MonoBehaviour// BASICALLY JUST COPYING CODE FROM PLAYERSCRIPT
{
    InputAction buttonPress;
    private bool pressed;

    public float[] Locations;
    private int startingPose = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPress = InputSystem.actions.FindAction("TapButton");
        
        transform.localPosition = new Vector2(Locations[startingPose], transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {   
        pressed = buttonPress.WasPerformedThisFrame();
        
         // Changes the players poses when the button is pressed
        if(pressed == true)
        {
            startingPose++;
            if(startingPose == Locations.Length)
            {
                startingPose = 0;
            }


                transform.localPosition = new Vector2(Locations[startingPose], transform.position.y);
            }
    }
}

