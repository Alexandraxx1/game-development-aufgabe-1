using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 0.8f;
    public float moveDistance = 0.8f;

    [Header("Push")]
    public float pushStrength = 0.08f;
    public float pushedOffDistance = 2.2f;

    [Header("Squash")]
    public AudioClip hitSound;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private bool movingRight = true;
    private bool defeated = false;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (defeated)
            return;

        MoveEnemy();

        if (Vector3.Distance(transform.position, startPosition) > pushedOffDistance)
        {
            SquashEnemy();
        }
    }

    private void MoveEnemy()
    {
        float direction = movingRight ? 1f : -1f;

        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);

        if (transform.position.x >= startPosition.x + moveDistance)
            movingRight = false;

        if (transform.position.x <= startPosition.x - moveDistance)
            movingRight = true;
    }

    public void Push(Vector3 direction)
    {
        if (defeated)
            return;

        direction.y = 0f;
        transform.position += direction.normalized * pushStrength;
    }

    public void SquashEnemy()
    {
        if (defeated)
            return;

        defeated = true;

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        transform.localScale = new Vector3(
            originalScale.x,
            originalScale.y * 0.2f,
            originalScale.z
        );

        Destroy(gameObject, 0.7f);
    }
}