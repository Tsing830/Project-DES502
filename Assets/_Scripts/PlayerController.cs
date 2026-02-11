using UnityEngine;
using UnityEngine.EventSystems;

public class SimplePlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int curHealth = 100;          // Player health
    public int maxHealth = 100;          // Max health

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

    [Header("Lean Settings")]
    public float leanAngle = 20f; // Maximum lean angle
    public float leanOffset = 0.3f; // How much to offset the camera when leaning
    public float leanSpeed = 10f; // Speed at which the player leans
    private float currentLean = 0f; // Current lean amount (-1 for left, 1 for right, 0 for no lean)

    [Header("Glance Behind")]
    public KeyCode glanceKey = KeyCode.Mouse2; // Key to glance behind
    private float glanceRotation = 0f; // Current glance rotation (0 for forward, 180 for behind)
    public float glanceSpeed = 6f; // Speed at which the player glances behind

    [Header("Head Shake")]
    public float walkShakeIntensity = 0.05f; // Intensity of head shake when moving
    public float walkShakeFrequency = 8f; // Frequency of head shake when moving

    public float sprintShakeIntensity = 0.1f; // Intensity of head shake when sprinting
    public float sprintShakeFrequency = 12f; // Frequency of head shake when sprinting

    public float sneakShakeIntensity = 0.03f; // Intensity of head shake when sneaking
    public float sneakShakeFrequency = 5f; // Frequency of head shake when sneaking

    private float bobTimer = 0f;
    private float defaultCamY;



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
        defaultCamY = playerCam.transform.localPosition.y;


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
        HandleLean();
        HandleGlance();
        CamLook();

        float moveMagnitude = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isSneaking = Input.GetKey(KeyCode.LeftControl);

        HandleCamShake(moveMagnitude, isSprinting, isSneaking);


    }

    void Move()
    {

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

    void HandleLean()
    {
        float targetLean = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            targetLean = -leanAngle; // Lean left
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetLean = leanAngle; // Lean right
        }

        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed); // Apply lean rotation and offset
                                                                                       // Apply roll + sideways offset
        Vector3 camPos = playerCam.transform.localPosition;
        camPos.x = Mathf.Lerp(camPos.x, currentLean / leanAngle * leanOffset, Time.deltaTime * leanSpeed);
        playerCam.transform.localPosition = camPos;

        Vector3 camRot = playerCam.transform.localEulerAngles;
        camRot.z = currentLean;


        Quaternion baseRotation = Quaternion.Euler(-verticalLookRotation, playerCam.transform.localEulerAngles.y, 0f);

        Quaternion leanRotation = Quaternion.Euler(0f, 0f, currentLean);




    }

    void HandleGlance()
    {
        float targetRotation = 0f;

        if (Input.GetKey(KeyCode.Tab))
            targetRotation = 180f;

        glanceRotation = Mathf.Lerp(glanceRotation, targetRotation, Time.deltaTime * glanceSpeed);

    }

    void HandleCamShake(float moveMagnitude, bool isSprinting, bool isSneaking)
    {
        if (moveMagnitude > 0.1f)
        {

            float intensity = walkShakeIntensity;
            float frequency = walkShakeFrequency;

            if (isSprinting)
            {
                intensity = sprintShakeIntensity;
                frequency = sprintShakeFrequency;
            }
            else if (isSneaking)
            {
                intensity = sneakShakeIntensity;
                frequency = sneakShakeFrequency;
            }
            bobTimer += Time.deltaTime * frequency * moveMagnitude;
            float shakeAmount = Mathf.Sin(bobTimer) * intensity * moveMagnitude;
            Vector3 camPos = playerCam.transform.localPosition;
            camPos.y = defaultCamY + shakeAmount;
            playerCam.transform.localPosition = camPos;

        }
        else
        {
            bobTimer = 0f;
            Vector3 camPos = playerCam.transform.localPosition;
            camPos.y = defaultCamY;
            playerCam.transform.localPosition = camPos;
        }
    }
    void CamLook()
    {

        //Mouse Look 
        // Get mouse horizontal movement amount
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        //Get mouse vertical movement amount
        verticalLookRotation += Input.GetAxis("Mouse Y") * lookSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minLookAngle, maxLookAngle);



        // Rotate around the Y-axis
        transform.Rotate(Vector3.up * mouseX);

        //Camera Rotation
        Quaternion pitch = Quaternion.Euler(-verticalLookRotation, 0f, 0f);
        Quaternion glance = Quaternion.Euler(0f, glanceRotation, 0f);
        Quaternion roll = Quaternion.Euler(0f, 0f, -currentLean);

        playerCam.transform.localRotation = glance * pitch * roll;

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

    public void TakeDamage(int damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle player death (e.g., respawn, game over screen)
        Debug.Log("Player has died!");
    }
}