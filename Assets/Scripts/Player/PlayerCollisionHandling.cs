using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class CollisionHandling : MonoBehaviour
    {
        public Action OnLanded;
        public Controller player;
        private bool previouslyGrounded = true;


        public PlayerCollider bodyCollider;
        public PlayerCollider leftCollider;
        public PlayerCollider rightCollider;
        public PlayerCollider frontCollider;


        private void Start()
        {
            bodyCollider.OnTriggered += HandleCollision;
            leftCollider.OnTriggered += HandleCollision;
            rightCollider.OnTriggered += HandleCollision;
            frontCollider.OnTriggered += HandleCollision;
        }

        private void OnDestroy()
        {
            bodyCollider.OnTriggered -= HandleCollision;
            leftCollider.OnTriggered -= HandleCollision;
            rightCollider.OnTriggered -= HandleCollision;
            frontCollider.OnTriggered -= HandleCollision;
        }


        /// <summary>
        ///   Processes the collision with the specified collider.
        /// </summary>
        /// <param name="other">The collider of the object collided with.</param>
        private void HandleCollision(ColliderPosition position, Collider other)
        {
            if (player.controlReleased) return;
            var colliderType = other.GetComponent<ColliderType>();
            if (colliderType == null) return;
            var t = colliderType.colliderType;
            var sound = colliderType.soundToPlayOnCollision;
            if (sound != "") AudioManager.Instance?.PlaySound(sound);

            switch (t)
            {
                case ColliderType.Type.DeathZone:
                    Debug.Log("[PlayerCollisionHandling] Collision with Death Zone: " + other.name);
                    player.health.TakeDamage(1);
                    break;

                case ColliderType.Type.Platform:
                case ColliderType.Type.MovingPlatform:
                    if (position != ColliderPosition.Body)
                    {
                        // Debug.Break();
                        Debug.Log("[PlayerCollisionHandling] Platform collided at Player." + position.ToString() + other.GetInstanceID()  + " with " + other.name + " (instance ID: " + other.GetInstanceID() + ")");
                        player.health.TakeDamage(1);
                    }
                    break;

                case ColliderType.Type.Collectible:
                    if (position == ColliderPosition.Body)
                    {
                        Debug.Log("[PlayerCollisionHandling] Collectible collided: " + other.name);
                        other.GetComponent<Collectible>().TriggerCollision();
                    }
                    break;

                case ColliderType.Type.ZoneChange:
                    if (position == ColliderPosition.Body)
                    {
                        StatsRecorder.UpdateLastBiomeReached(BiomesData.Instance.current.BiomeName);
                    }
                    break;

                default:
                    Debug.LogWarning("[PlayerCollisionHandling] Unhandled collider type: " + t.ToString());
                    break;
            }
        }



        /// <summary>
        ///  Check grounded state each frame
        /// </summary>
        private void Update()
        {
            if (SceneInitializer.Instance.isInitialized)
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

            // Grounded detection
            bool isCurrentlyGrounded = false;
            if (raycastHit)
            {
                var colliderType = hit.collider.GetComponent<ColliderType>();
                if (colliderType != null 
                && (
                    colliderType.colliderType == ColliderType.Type.Platform
                    || colliderType.colliderType == ColliderType.Type.MovingPlatform
                ))
                {
                    isCurrentlyGrounded = hit.distance <= .2f;
                }
            }

            // Update grounded state
            player.isGrounded = isCurrentlyGrounded;

            // Set jump animation state ~
            player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", !player.isGrounded);

            // If just landed, invoke the OnLanded event
            if (!previouslyGrounded && player.isGrounded)
            {
                OnLanded?.Invoke();
            }
            previouslyGrounded = player.isGrounded;
        }



        /// <summary>
        ///  Check if there is an obstacle in front of the player
        /// </summary>
        /// <param name="distance">The distance to check for obstacles.</param>
        public bool CheckIfObstacleInFront(float distance)
        {
            RaycastHit hit;
            List<Vector3> rays = new()
            {
                transform.position + (Vector3.forward * .2f) + (Vector3.up * 0.2f)
                , transform.position + (Vector3.right * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.2f)
                , transform.position + (Vector3.left * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.2f)
                , transform.position + (Vector3.forward * .2f) + (Vector3.up * 0.5f)
                , transform.position + (Vector3.right * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.5f)
                , transform.position + (Vector3.left * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.5f)
                , transform.position + (Vector3.forward * .2f) + (Vector3.up * 1f)
                , transform.position + (Vector3.right * 1f) + (Vector3.forward * .2f) + (Vector3.up * 1f)
                , transform.position + (Vector3.left * 1f) + (Vector3.forward * .2f) + (Vector3.up * 1f)
            };

            var raysDir = Vector3.forward * distance;
            foreach (var start in rays)
            {
                Debug.DrawLine(start, start + raysDir, Color.red);
                if (Physics.Raycast(start, raysDir, out hit, distance))
                {
                    return true;
                }
            }
            return false;
        }


        public bool CheckIfGroundInFront(float distance)
        {
            RaycastHit hit;
            var ray = transform.position + (Vector3.forward * .5f) + (Vector3.up * 0.5f);
            var raysDir = (Vector3.forward * distance) - (Vector3.up * .6f);
            Debug.DrawLine(ray, ray + raysDir, Color.yellow);
            if (Physics.Raycast(ray, raysDir, out hit, distance))
            {
                var c = hit.collider.GetComponent<ColliderType>();
                if (c != null && c.colliderType == ColliderType.Type.Platform )
                    return true;
            }
            return false;
        }
    }
}
