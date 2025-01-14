using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeToAspectRatioScreen : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // The SpriteRenderer of the object

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        ResizeToFillScreenWidth(); // Call the resizing function at the start
    }

    void ResizeToFillScreenWidth()
    {
        // Get the screen height in world units
        float screenHeight = Camera.main.orthographicSize * 2f;

        // Calculate the screen width in world units based on the aspect ratio
        float screenWidth = screenHeight * Screen.width / Screen.height;

        // Get the size of the sprite in world units
        Vector2 spriteSize = spriteRenderer.bounds.size;

        // Calculate scale for width and height separately
        float scaleX = screenWidth / spriteSize.x; // Scale to fill the screen width
        float scaleY = screenHeight / spriteSize.y; // Scale to fill the screen height (optional)

        // Apply the scale to the object
        transform.localScale = new Vector3(scaleX, scaleY, 1);
    }
}
