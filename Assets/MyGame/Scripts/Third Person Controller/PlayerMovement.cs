using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;            // Movement speed
    public float jumpHeight = 1.5f;     // Jump height
    public float gravity = -9.81f;      // Gravity strength

    [Header("Rotation Settings")]
    public float mouseSensitivity = 100f;  // Sensitivity for looking around

    private CharacterController controller;
    private Vector3 velocity;               // Stores gravity and jumping force
    private Transform cameraTransform;      // Reference to the player's camera
    private float xRotation = 0f;           // Used to clamp camera rotation

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;  // Assumes the main camera is the player's view

        // Lock cursor to center and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Move relative to the player's forward direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jump physics formula
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to ensure staying grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically (look up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical rotation
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
