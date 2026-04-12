using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float platformSpeed = 1.0f;

    [SerializeField]
    private Vector3 start;

    [SerializeField]
    private Vector3 end;

    [SerializeField]
    private bool isActive = false;

    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Start()
    {
        this.lastPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        if (!isActive)
        {
            this.velocity = Vector3.zero;
            this.lastPosition = this.transform.position;
            return;
        }

        float pingPong = Mathf.PingPong(Time.fixedTime * this.platformSpeed, 1.0f);
        Vector3 newPosition = Vector3.Lerp(this.start, this.end, pingPong);
        this.transform.localPosition = newPosition;

        this.velocity = (this.transform.position - this.lastPosition) / Time.fixedDeltaTime;
        this.lastPosition = this.transform.position;
    }

    public Vector3 GetVelocity()
    {
        return this.velocity;
    }

    public void ActivatePlatform()
    {
        this.isActive = true;
    }
}