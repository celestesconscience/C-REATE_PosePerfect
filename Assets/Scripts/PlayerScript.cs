using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    //makes sure the button works
    InputAction buttonPress;
    public bool pressed;

    //player movement script
    public float[] Locations = new float[3];
    public int currentLocation;
    public int changeAMT = 1;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   

        buttonPress = InputSystem.actions.FindAction("Button");//for button input

        //MOVES THE PLAYER TO SPECIFIC LOCATION
        currentLocation = 1;//START LOCATION
        transform.position = new Vector2(Locations[currentLocation], -1.8f);
    }

    // Update is called once per frame
    void Update()
    {
        //pressed = buttonPress.IsPressed();
        pressed = buttonPress.WasPressedThisFrame();;

        if(pressed == true)
        {
            print("PRESSING");
        }  

        if(pressed == true)//MAKES THE GUY MOVE LEFT TO RIGHT
        {
            currentLocation+= changeAMT;

            if(currentLocation== Locations.Length)//goes past limit
            {
                changeAMT = -changeAMT;//turns positive to negative

                currentLocation+= changeAMT;
                currentLocation+= changeAMT;
            }
            else if(currentLocation == -1)//goes to negative
            {
                changeAMT = -changeAMT;//turns negative to positive

                currentLocation+= changeAMT;
                currentLocation+= changeAMT;
            }

            transform.position = new Vector2(Locations[currentLocation], -1.8f);
        }

    }
}
