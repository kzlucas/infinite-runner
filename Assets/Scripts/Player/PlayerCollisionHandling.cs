using System;
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
                    player.TriggerCrashEvent();
                    break;
                    
                case ColliderType.Type.Platform:
                    if(position != ColliderPosition.Body)
                    {
                        player.TriggerCrashEvent();
                    }
                    break;

                case ColliderType.Type.Collectible:
                    if (position == ColliderPosition.Body)
                    {
                        other.GetComponent<Animator>()?.SetTrigger("OnCollide");
                    }
                    break;

                case ColliderType.Type.ZoneChange:
                    if (position == ColliderPosition.Body)
                    {
                        StatsRecorder.Instance.UpdateLastBiomeReached(BiomesData.Instance.current.BiomeName);
                    }
                    break;

                default:
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
                if (colliderType != null && colliderType.colliderType == ColliderType.Type.Platform)
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

    }
}
