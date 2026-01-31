using Assets.Components.SquareColliders.Scripts;
using Components.Audio.Scripts;
using Components.Events;
using Components.Stats;
using UnityEngine;
using WorldGenerator.Scripts;

namespace Components.Player
{
    public class CollisionHandling : MonoBehaviour
    {


        [Header("Flags")]
        private bool wasPreviouslyGrounded = true;
        

        [Header("References")]
        public Controller Player;


        [Header("Colliders References")]
        public PlayerCollider BodyCollider;
        public PlayerCollider LeftCollider;
        public PlayerCollider RightCollider;
        public PlayerCollider FrontCollider;


        private void Start()
        {
            BodyCollider.OnTriggered += HandleCollision;
            LeftCollider.OnTriggered += HandleCollision;
            RightCollider.OnTriggered += HandleCollision;
            FrontCollider.OnTriggered += HandleCollision;
        }

        private void OnDestroy()
        {
            BodyCollider.OnTriggered -= HandleCollision;
            LeftCollider.OnTriggered -= HandleCollision;
            RightCollider.OnTriggered -= HandleCollision;
            FrontCollider.OnTriggered -= HandleCollision;
        }


        /// <summary>
        ///   Processes the collision with the specified collider.
        /// </summary>
        /// <param name="other">The collider of the object collided with.</param>
        private void HandleCollision(ColliderPosition position, Collider other)
        {
            if (Player.ControlReleased) return;
            var colliderType = other.GetComponent<ColliderType>();
            if (colliderType == null) return;
            var t = colliderType.colliderType;
            var sound = colliderType.soundToPlayOnCollision;
            if (sound != "") AudioManager.Instance?.PlaySound(sound);

            switch (t)
            {
                case ColliderType.Type.DeathZone:
                    Debug.Log("[PlayerCollisionHandling] Collision with Death Zone: " + other.name);
                    Player.Health.TakeDamage(1);
                    break;

                case ColliderType.Type.Platform:
                case ColliderType.Type.MovingPlatform:
                    if (position != ColliderPosition.Body)
                    {
                        // Debug.Break();
                        Debug.Log("[PlayerCollisionHandling] Platform collided at Player." + position.ToString() + other.GetInstanceID()  + " with " + other.name + " (instance ID: " + other.GetInstanceID() + ")");
                        Player.Health.TakeDamage(1);
                    }
                    break;

                case ColliderType.Type.Collectible:
                    if (position == ColliderPosition.Body)
                    {
                        Debug.Log("[PlayerCollisionHandling] Collectible collided: " + other.name);
                        other.GetComponent<Components.Collectible.Collectible>().TriggerCollision();
                    }
                    break;

                case ColliderType.Type.ZoneChange:
                    if (position == ColliderPosition.Body)
                    {
                        StatsRecorder.UpdateLastBiomeReached(BiomesDataManager.Instance.Current.BiomeName);
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
            Player.IsGrounded = isCurrentlyGrounded;

            // Set jump animation state ~
            Player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", !Player.IsGrounded);

            // If just landed, invoke the OnLanded event
            if (!wasPreviouslyGrounded && Player.IsGrounded)
            {
                EventBus.Publish(new Events.Landed());
            }
            wasPreviouslyGrounded = Player.IsGrounded;
        }


    }
}
