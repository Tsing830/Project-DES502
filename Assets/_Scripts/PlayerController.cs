using UnityEngine;
using UnityEngine.EventSystems;

public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;        // Movement speed
    public float sprintMultiplier = 2f; // Sprint speed multiplier   
    public float sneakMultiplier = 0.5f;  // Sneak speed multiplier

    public float crouchHeight = 1f;    // Crouch height
    private float originalHeight;      // Original player height

    public float mouseSensitivity = 3f; // Mouse look sensitivity
    public float jumpForce = 5f;        // Jump force

    [Header("Camera Settings")]
    public float lookSensitivity = 2f; // Camera look sensitivity
    public float maxLookAngle = 80f;  // Max vertical look angle
    public float minLookAngle = -80f; // Min vertical look angle
    private float verticalLookRotation = 0f; // Current vertical look rotation

    private Camera playerCam;
    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    void Awake()
    {
        // Get references to components
        playerCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        originalHeight = playerCollider.height;

        // Lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        HandleSneak();
        CamLook();
    
    }

    void Move() {

        // --- 1. Movement (WASD) ---
        // Get keyboard input (A/D for Horizontal, W/S for Vertical)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction (relative to player orientation)
        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = moveSpeed;

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }


        // Sneaking
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed *= sneakMultiplier;
        }

        // Execute movement
        transform.position += move * currentSpeed * Time.deltaTime;

     
    }

    void HandleSneak()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            // Lower height
            playerCollider.height = Mathf.Lerp(playerCollider.height, crouchHeight, Time.deltaTime * 10f);
        }
        else
        {
            // Return to normal height
            playerCollider.height = Mathf.Lerp(playerCollider.height, originalHeight, Time.deltaTime * 10f);
        }
    }


    void CamLook() {

        // --- 2. Rotation (Mouse Left/Right) ---
        // Get mouse horizontal movement amount
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;

        //Get mouse vertical movement amount
        verticalLookRotation += Input.GetAxis("Mouse Y") * lookSensitivity;

        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minLookAngle, maxLookAngle);


        // Rotate camera up/down
        playerCam.transform.localEulerAngles = Vector3.left * verticalLookRotation;

        // Rotate around the Y-axis
        transform.Rotate(Vector3.up * mouseX);

    }

    void Jump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (!Physics.Raycast(ray, 1.1f))
        {
            return; // Not grounded, exit the method
        }
        // Apply upward force for jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}