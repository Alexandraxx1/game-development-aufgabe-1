using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 180f;
    public Transform cameraTransform;
    public float minPitch = -60f;
    public float maxPitch = 60f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 platformVelocity;

    private float cameraPitch = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        RotateWithMouse();

        bool isGrounded = controller.isGrounded;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void FixedUpdate()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float moveX = 0f;
        float moveZ = 0f;

        // WASD oder Pfeiltasten
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveX = -1f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveX = 1f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveZ = 1f;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveZ = -1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 playerMovement = move.normalized * speed;

        platformVelocity = isGrounded ? GetPlatformVelocity() : Vector3.zero;

        Vector3 combinedMovement = playerMovement + platformVelocity;
        controller.Move(combinedMovement * Time.fixedDeltaTime);

        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    private void RotateWithMouse()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Spieler links/rechts drehen
            transform.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);

            // Kamera hoch/runter drehen
            cameraPitch -= mouseY * mouseSensitivity * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

            if (cameraTransform != null)
            {
                cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private Vector3 GetPlatformVelocity()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platforms"))
            {
                MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();

                if (platform != null)
                {
                    return platform.GetVelocity();
                }
            }
        }

        return Vector3.zero;
    }
}