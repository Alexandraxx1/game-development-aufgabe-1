using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField]
    private MovingPlatform[] platformsToActivate;

    private bool playerInRange = false;
    private bool leverActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = false;
        }
    }

    private void FixedUpdate()
    {
        if (leverActivated)
            return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            leverActivated = true;
            Debug.Log("Lever activated");

            foreach (MovingPlatform platform in platformsToActivate)
            {
                if (platform != null)
                {
                    platform.ActivatePlatform();
                }
            }
        }
    }
}