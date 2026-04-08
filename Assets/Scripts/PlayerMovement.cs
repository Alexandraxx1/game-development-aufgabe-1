using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 120f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 platformVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Bewegung nur mit Pfeiltasten
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveX = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) moveZ = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveZ = -1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 playerMovement = move.normalized * speed;

        // Plattform-Geschwindigkeit nur anwenden, wenn Spieler nicht springt
        platformVelocity = isGrounded ? GetPlatformVelocity() : Vector3.zero;

        Vector3 combinedMovement = playerMovement + platformVelocity;
        controller.Move(combinedMovement * Time.fixedDeltaTime);

        // Rotation nur mit A / D
        float rotationY = 0f;

        if (Input.GetKey(KeyCode.A)) rotationY = -1f;
        if (Input.GetKey(KeyCode.D)) rotationY = 1f;

        transform.Rotate(Vector3.up * rotationY * rotationSpeed * Time.fixedDeltaTime);

        // Springen
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravitation
        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);
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