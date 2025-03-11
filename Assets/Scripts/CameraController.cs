using UnityEngine;

public class Follow_player : MonoBehaviour
{
    public Transform player; // Reference to the player's transform

    // Camera boundaries
    public Vector2 minBounds; // Minimum X and Y bounds for the camera
    public Vector2 maxBounds; // Maximum X and Y bounds for the camera

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set in the Follow_player script.");
            return;
        }

        // Calculate the desired camera position
        Vector3 desiredPosition = player.position + new Vector3(0, 1, -5);

        // Clamp the camera position within the defined boundaries
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        // Update the camera's position
        transform.position = desiredPosition;
    }
}