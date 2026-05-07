using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Mouse Rotation")]
    public float mouseSensitivity = 180f;

    [Header("Audio")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip jumpClip;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 platformVelocity;

    private AudioSource audioSource;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = 0f;
        float moveZ = 0f;

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

        controller.Move(combinedMovement * Time.deltaTime);

        HandleFootsteps(moveX, moveZ, isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            if (jumpClip != null)
            {
                audioSource.PlayOneShot(jumpClip);
            }

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        RotateWithMouse();
    }

    private void HandleFootsteps(float moveX, float moveZ, bool isGrounded)
    {
        if (footstepClip == null)
            return;

        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        if (isMoving && isGrounded)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = footstepClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void RotateWithMouse()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");

            transform.Rotate(
                Vector3.up * mouseX * mouseSensitivity * Time.deltaTime
            );
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
            if (hit.collider.gameObject.layer ==
                LayerMask.NameToLayer("Platforms"))
            {
                MovingPlatform platform =
                    hit.collider.GetComponent<MovingPlatform>();

                if (platform != null)
                {
                    return platform.GetVelocity();
                }
            }
        }

        return Vector3.zero;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Enemy enemy = hit.collider.GetComponent<Enemy>();

        if (enemy == null)
            return;

        bool playerAboveEnemy =
            transform.position.y > enemy.transform.position.y + 1f;

        bool playerIsFalling = velocity.y < 0f;

        if (playerAboveEnemy && playerIsFalling)
        {
            enemy.SquashEnemy();

            velocity.y =
                Mathf.Sqrt(jumpHeight * -2f * gravity) * 0.5f;

            return;
        }

        Vector3 pushDirection =
            new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        enemy.Push(pushDirection);
    }
}