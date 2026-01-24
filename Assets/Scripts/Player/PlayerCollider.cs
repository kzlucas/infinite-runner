using System;
using UnityEngine;

public enum ColliderPosition
{
    Body,
    Left,
    Right,
    Front,
}


public class PlayerCollider : MonoBehaviour
{

    public ColliderPosition colliderPosition;

    public Action<ColliderPosition, Collider> OnTriggered;



    /// <summary>
    ///   Handles trigger events with other objects with isTrigger enabled.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        OnTriggered?.Invoke(colliderPosition, other);
    }


    /// <summary>
    ///   Handles collision events with other objects without isTrigger enabled.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        OnTriggered?.Invoke(colliderPosition, collision.collider);
    }

}
