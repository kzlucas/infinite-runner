using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerCollisionHandling : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();        
    }

    /// <summary>
    ///   Handles collision events with other objects.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.collider);
    }

    /// <summary>
    ///   Processes the collision with the specified collider.
    /// </summary>
    /// <param name="other">The collider of the object collided with.</param>
    private void HandleCollision(Collider other)
    {

        if(playerController.controlReleased) return;
        var colliderType = other.GetComponent<ColliderType>().colliderType;


        if ((colliderType == ColliderType.Type.Obstacle)
           || (colliderType == ColliderType.Type.Wall))
        {
            Debug.Log("[PlayerCollisionHandling] Collided with obstacle: " + other.name);

            Camera.main.GetComponent<CameraBehaviors>().ShakeCamera(0.1f, 0.15f);
            AudioManager.Instance?.PlaySound("hit obstacle");

            playerController.OnCrash();
        }

        if (colliderType == ColliderType.Type.Ground)
        {
            Debug.Log("[PlayerCollisionHandling] Collided with ground: " + other.name);            
            playerController.isGrounded = true;
        }

        else
        {
            Debug.Log("[PlayerCollisionHandling] Collided with: " + other.name);
        }
    }
}
