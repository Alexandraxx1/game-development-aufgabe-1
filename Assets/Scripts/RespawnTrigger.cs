using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.gameObject.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
            controller.transform.position = respawnPoint.position;
            controller.enabled = true;
        }
    }
}