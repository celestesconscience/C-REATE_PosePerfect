using UnityEngine;
using System.Collections;

// Controls the random encouragement animations shown after successful wall matches.
public class EncouragementTextScript : MonoBehaviour
{
    // ==================== ANIMATION COMPONENTS ====================

    // Controls which encouragement animation is played.
    private Animator anim;

    // Displays the sprite frames used by the encouragement animations.
    private SpriteRenderer spriteRenderer;


    // ==================== INITIALIZATION ====================

    void Start()
    {
        // Get the Animator and SpriteRenderer attached to this GameObject.
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // ==================== RANDOM ENCOURAGEMENT ====================

    // Selects and plays a random encouragement animation.
    // Called by PlayerScript after every five successful wall matches.
    public void PlayRandomEncouragement()
    {
        // Generate a random number from 0 to 7 (8 possible animations).
        // The maximum value is exclusive for integer Random.Range().
        int randomText = Random.Range(0, 8);

        // Play the animation corresponding to the randomly selected number.
        switch (randomText)
        {
            case 0:
                anim.Play("AwesomeText", 0, 0f);
                break;

            case 1:
                anim.Play("CoolText", 0, 0f);
                break;

            case 2:
                anim.Play("GroovyText", 0, 0f);
                break;

            case 3:
                anim.Play("WowText", 0, 0f);
                break;

            case 4:
                anim.Play("GreatText", 0, 0f);
                break;

            case 5:
                anim.Play("NiceText", 0, 0f);
                break;

            case 6:
                anim.Play("RadicalText", 0, 0f);
                break;

            case 7:
                anim.Play("TubularText", 0, 0f);
                break;
        }

        // Start the coroutine that hides the animation after one second.
        StartCoroutine(HideEncouragement());
    }


    // ==================== ANIMATION CLEANUP ====================

    // Waits one second before returning to the invisible Idle state.
    IEnumerator HideEncouragement()
    {
        // Allow the encouragement animation to remain visible for one second.
        yield return new WaitForSeconds(1f);

        // Return the Animator to its Idle state.
        anim.Play("Idle");

        // Clear the displayed sprite so no encouragement remains visible.
        spriteRenderer.sprite = null;
    }
}