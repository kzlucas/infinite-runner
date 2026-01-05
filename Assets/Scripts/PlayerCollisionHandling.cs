using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerCollisionHandling : MonoBehaviour
{
    public Action OnLanded;
    private PlayerController playerController;


    /// <summary>
    ///   Initialize references
    /// </summary>
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    /// <summary>
    ///  Check grounded state each frame
    /// </summary>
    private void Update()
    {
        CheckIfGrounded();
    }


    /// <summary>
    ///   Check if the player is grounded using a raycast.
    /// </summary>
    private void CheckIfGrounded()
    {
        // Send a raycast down to check if grounded
        RaycastHit hit;
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);
        float rayDistance = .2f;
        bool raycastHit = Physics.Raycast(rayStart, Vector3.down, out hit, rayDistance);
        bool previouslyGrounded = playerController.isGrounded;
        
        // Grounded detection
        bool isCurrentlyGrounded = false;
        if (raycastHit)
        {
            var colliderType = hit.collider.GetComponent<ColliderType>();
            if (colliderType != null && colliderType.colliderType == ColliderType.Type.Ground)
            {
                isCurrentlyGrounded = hit.distance <= .2f;
            }
        }
        
        // Update grounded state
        playerController.isGrounded = isCurrentlyGrounded;

        // If just landed, invoke the OnLanded event
        if (!previouslyGrounded && playerController.isGrounded)
        {
            OnLanded?.Invoke();
        }
    }


    /// <summary>
    ///   Handles trigger events with other objects with isTrigger enabled.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }


    /// <summary>
    ///   Handles collision events with other objects without isTrigger enabled.
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

        if (playerController.controlReleased) return;
        var colliderType = other.GetComponent<ColliderType>();
        if (colliderType == null) return;
        var t = colliderType.colliderType;
        var sound = colliderType.soundToPlayOnCollision;
        if (sound != "") AudioManager.Instance?.PlaySound(sound);

        switch (t)
        {
            case ColliderType.Type.Obstacle:
            case ColliderType.Type.Wall:
                Debug.Log("[PlayerCollisionHandling] Collided with obstacle: " + other.name);
                playerController.OnCrash();
                break;

            case ColliderType.Type.Ground:
                playerController.isGrounded = true;
                break;

            case ColliderType.Type.Collectible:
                other.GetComponent<Animator>().SetTrigger("OnCollide");
                break;

            default:
                Debug.Log("[PlayerCollisionHandling] Collided with: " + other.name);
                break;
        }
    }
}
