using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionFace : MonoBehaviour
{
    public PlayerController playerController;  // Drag and drop the PlayerController here
    public GameObject visualPlayer;  // Drag and drop the object that should rotate (visual player)
    public GameObject centralObject;  // Drag and drop the central object to determine the new forward
    public float rotationSpeed = 5f;  // Editable rotation/transition speed

    private Vector3 lastMoveDirection;

    void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned in PlayerDirectionFace.");
        }

        if (visualPlayer == null)
        {
            Debug.LogError("VisualPlayer is not assigned in PlayerDirectionFace.");
        }

        if (centralObject == null)
        {
            Debug.LogError("CentralObject is not assigned in PlayerDirectionFace.");
        }

        // Initialize the last move direction to a zero vector
        lastMoveDirection = Vector3.zero;
    }

    void Update()
    {
        if (playerController == null || visualPlayer == null || centralObject == null)
            return;

        // Always track movement direction regardless of button presses
        TrackMovementDirection();

        // Rotate the visual player only if there is valid movement
        if (playerController.canMove)
        {
            HandleRotation();
        }
    }

    // Method to handle the visual rotation of the player
    private void HandleRotation()
    {
        // If there is movement, rotate the visual player based on the last direction
        if (lastMoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
            visualPlayer.transform.rotation = Quaternion.Slerp(visualPlayer.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Track the movement direction of the player (if moving)
    private void TrackMovementDirection()
    {
        if (playerController != null)
        {
            // Get the centralObject's rotation to determine new forward
            Quaternion centralRotation = Quaternion.Euler(0, centralObject.transform.eulerAngles.y, 0);

            // Get movement input in local space
            Vector3 localMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Transform the local movement direction to world space relative to the centralObject's orientation
            Vector3 worldMoveDirection = centralRotation * localMoveDirection;

            // Flip the direction to reverse the movement
            Vector3 flippedDirection = -worldMoveDirection;

            // If there is movement, set the last direction
            if (flippedDirection.magnitude > 0.1f)
            {
                lastMoveDirection = flippedDirection.normalized;
            }
        }
    }
}