using UnityEngine;
using UnityEngine.UI;

public class LogoLoop : MonoBehaviour
{

    private float cycleInterval = 0.3f; // Time between image changes
    
    private Image imageComponent;
    private Sprite[] sprites;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        
        // Load your 3 images from Resources folder
        sprites = new Sprite[3];
        sprites[0] = Resources.Load<Sprite>("Images/UI/frame1");
        sprites[1] = Resources.Load<Sprite>("Images/UI/frame2");
        sprites[2] = Resources.Load<Sprite>("Images/UI/frame3");
        
        imageComponent.sprite = sprites[0];
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= cycleInterval)
        {
            timer = 0f;
            
            // Move to next image
            currentIndex = (currentIndex + 1) % sprites.Length;
            imageComponent.sprite = sprites[currentIndex];
        }
    }
}
