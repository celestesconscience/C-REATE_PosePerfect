using UnityEngine;
using UnityEngine.UI;

public class PoseUIScript : MonoBehaviour
{   
    public Sprite[] poses;
    public Sprite[] currentPoses = new Sprite [5];

    private Image image;

    public int poseNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        currentPoses[0] = poses[GameManager.instance.randomWallNums[0]];
        currentPoses[1] = poses[GameManager.instance.randomWallNums[1]];
        currentPoses[2] = poses[GameManager.instance.randomWallNums[2]];
        currentPoses[3] = poses[GameManager.instance.randomWallNums[3]];
        currentPoses[4] = poses[GameManager.instance.randomWallNums[4]];
        
        image.sprite = currentPoses[poseNum];
    }
}
