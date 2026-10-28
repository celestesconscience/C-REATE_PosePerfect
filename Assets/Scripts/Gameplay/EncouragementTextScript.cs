using UnityEngine;
using System.Collections;

public class EncouragementTextScript : MonoBehaviour
{
    // Reference to the encouragement text Animator
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayRandomEncouragement()
{
    int randomText = Random.Range(0, 7);

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

    StartCoroutine(HideEncouragement());

}

    IEnumerator HideEncouragement()
    {
        yield return new WaitForSeconds(1f);

        anim.Play("Idle");
        spriteRenderer.sprite = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
