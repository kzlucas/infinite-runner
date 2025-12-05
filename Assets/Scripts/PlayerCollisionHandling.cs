using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerCollisionHandling : MonoBehaviour
{
    
    /// <summary>
    ///   Handles collision events with other objects.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("[PlayerCollisionHandling] Collided with obstacle: " + other.name);

            Camera.main.GetComponent<CameraBehaviors>().ShakeCamera(0.1f, 0.15f);
            AudioManager.Instance?.PlaySound("hit obstacle");
        }
    }
}
