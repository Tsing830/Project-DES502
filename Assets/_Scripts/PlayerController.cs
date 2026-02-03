using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;        // Movement speed
    public float mouseSensitivity = 2f; // Mouse look sensitivity

    void Update()
    {
        // --- 1. Movement (WASD) ---
        // Get keyboard input (A/D for Horizontal, W/S for Vertical)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction (relative to player orientation)
        Vector3 move = transform.right * x + transform.forward * z;

        // Execute movement
        transform.position += move * moveSpeed * Time.deltaTime;

        // --- 2. Rotation (Mouse Left/Right) ---
        // Get mouse horizontal movement amount
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        // Rotate around the Y-axis
        transform.Rotate(Vector3.up * mouseX);
    }
}