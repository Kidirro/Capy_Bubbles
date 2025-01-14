using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollRect2D : MonoBehaviour
{
    public Transform content; // The parent object containing all scrollable items
    public float scrollSpeed = 10f; // Speed of scrolling
    public bool scrollFromTop = true; // Flag to determine whether scrolling starts from top or bottom
    public float spacing = 10f; // Space between each child

    private Vector3 dragStartPosition;
    private Vector3 contentStartPosition;
    private bool isDragging;

    // Dynamic bounds (in world space)
    private float minY;
    private float maxY;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main; // Get the main camera

        // Initialize dynamic bounds based on content size
        SetScrollBounds();

        // Set the initial scroll position
        SetInitialScrollPosition();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseScroll(); // Handle mouse wheel scrolling
        HandleDragScroll();  // Handle drag-based scrolling
    }

    // Dynamically set the scroll bounds based on content height and screen size
    private void SetScrollBounds()
    {
        // Calculate the screen height in world space (this is the vertical height of the camera view)
        float screenHeight = mainCamera.orthographicSize * 2f;

        // Calculate the total height of the content based on the children's sizes and spacing
        float totalHeight = 0f;
        foreach (Transform child in content)
        {
            // Calculate the child's height based on its SpriteRenderer bounds
            SpriteRenderer spriteRenderer = child.GetComponent<ResizeToAspectRatioScreen>().spriteRenderer;
            if (spriteRenderer != null)
            {
                totalHeight += spriteRenderer.bounds.size.y;
            }
        }

        // Add spacing between the children (if necessary)
        totalHeight += (content.childCount - 1) * spacing; // Adjust spacing between children

        // Set the scroll bounds based on content size and screen size
        maxY = totalHeight / 2f;
        minY = -totalHeight / 2f + screenHeight; // Adjust for screen height
    }

    // Set the initial position of the content based on the scrollFromTop flag
    private void SetInitialScrollPosition()
    {
        if (content == null || mainCamera == null) return;

        // Calculate the correct Y-position to make the content visible within the camera's view
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calculate the starting position of content based on scrollFromTop
        if (scrollFromTop)
        {
            // Position the content at the top of the viewable area
            content.position = new Vector3(content.position.x, maxY, content.position.z);
        }
        else
        {
            // Position the content at the bottom of the viewable area
            content.position = new Vector3(content.position.x, minY, content.position.z);
        }

        // Adjust the content position to make sure it fits within the camera's viewable area
        Vector3 contentPosition = content.position;
        contentPosition.y = Mathf.Clamp(contentPosition.y, -cameraHeight / 2f, cameraHeight / 2f); // Ensures content stays within the camera view
        content.position = contentPosition;
    }

    private void HandleMouseScroll()
    {
        // Scroll using the mouse wheel
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            Vector3 moveDelta = new Vector3(0, scrollDelta * scrollSpeed, 0);
            content.position += moveDelta;
            ClampContentPosition();
        }
    }

    private void HandleDragScroll()
    {
        if (Input.GetMouseButtonDown(0)) // On mouse down
        {
            isDragging = true;
            dragStartPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            contentStartPosition = content.position;
        }
        else if (Input.GetMouseButton(0) && isDragging) // While dragging
        {
            Vector3 currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dragDelta = currentMousePosition - dragStartPosition;

            content.position = contentStartPosition + new Vector3(0, dragDelta.y, 0); // Only scroll vertically
            ClampContentPosition();
        }
        else if (Input.GetMouseButtonUp(0)) // On mouse release
        {
            isDragging = false;
        }
    }

    private void ClampContentPosition()
    {
        // Clamp content position to ensure it stays within the bounds
        Vector3 clampedPosition = content.position;
        clampedPosition.y = Mathf.Clamp(content.position.y, minY, maxY);
        content.position = clampedPosition;
    }
}
