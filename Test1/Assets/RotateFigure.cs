using UnityEngine;
using UnityEngine.EventSystems;

public class MouseRotateWithSpecificButtonHover : MonoBehaviour
{
    public GameObject targetObject; // The object to rotate, set in the Inspector
    public GameObject hoverButton; // The specific UI button to check for hover
    public float rotationSpeed = 5f; // Speed of rotation, adjustable in the Inspector

    private bool isMouseOverButton = false; // Tracks if the mouse is over the specified button
    private bool isDragging = false; // Tracks if the user is dragging the mouse

    private void Update()
    {
        // Check if the mouse is over the specific button using EventSystem
        isMouseOverButton = hoverButton != null && EventSystem.current.IsPointerOverGameObject();

        // If the mouse is over the button
        if (isMouseOverButton)
        {
            // Start rotation when the left mouse button is pressed
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
            }

            // Stop rotation when the left mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            // Rotate the object if dragging
            if (isDragging && targetObject != null)
            {
                float mouseDeltaX = Input.GetAxis("Mouse X"); // Get horizontal mouse movement
                targetObject.transform.Rotate(0f, -mouseDeltaX * rotationSpeed, 0f, Space.World); // Invert rotation direction
            }
        }
        else
        {
            isDragging = false; // Ensure dragging stops if the mouse leaves the button
        }
    }
}
