using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components.ServiceLocator.Scripts;
using Components.TimeScale;
using Components.UI.Scripts;
using Player.States;
using UnityEngine;
using WorldGenerator.Scripts;


namespace Player
{
    /// <summary>
    /// Represents a single record of the player's position 
    /// and velocity for history tracking
    /// </summary>
    public class PlayerHistoryRecord
    {
        public Vector3 position;
        public Vector3 rbVelocity;

        public PlayerHistoryRecord(Vector3 pos, Vector3 vel)
        {
            position = pos;
            rbVelocity = vel;
        }
    }


    /// <summary>
    ///   Records and restores player position history for respawning
    /// </summary>
    public class PlayerHistory : MonoBehaviour
    {
        
        [Header("Dependencies")]
        private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();


        [Header("History Settings")]
        public List<PlayerHistoryRecord> records = new List<PlayerHistoryRecord>();
        public bool _disableRecord = false;
        public bool HasObstacleInFront = false;
        public bool HasGroundInFront = true;
        private float lastRecordTime;
        private Vector3 lastRecordPosition;


        private bool IsSafeZoneToRespawn
        {
            get
            {
            var player = GetComponent<Controller>();

                // Conditions to be able to record a new player position
                // - player is grounded
                // - player is not sliding
                // - player is not changing lane
                // - recording is not disabled
                // - no obstacle in front of the player
                // - there is ground in front of the player
                // - at least 1 second since last record
                // - player is far from last record at least 10 units

                return( 
                    player.IsGrounded 
                    && !player.IsSliding 
                    && !player.IsChangingLane 
                    && !HasObstacleInFront
                    && HasGroundInFront
                    && !_disableRecord
                    && Time.time - lastRecordTime > 1f 
                    && Vector3.Distance(lastRecordPosition, player.transform.position) > 10f
                )
            ;
            }
        }


        /// <summary>
        /// Check if we should record the player position this frame
        /// </summary>
        void LateUpdate()
        {
            var player = GetComponent<Controller>();
            HasObstacleInFront = Utils.CheckIfObstacleInFront(player.transform, 15f);
            HasGroundInFront = Utils.CheckIfGroundInFront(player.transform, 5f);
            if(IsSafeZoneToRespawn)
            {
                lastRecordTime = Time.time;
                lastRecordPosition = transform.position;
                Record();
            }
        }




        /// <summary>
        /// Record the current player position and velocity
        /// </summary>
        public void Record()
        {
            var record = new PlayerHistoryRecord(transform.position, GetComponent<Rigidbody>().linearVelocity);
            records.Add(record);
            if (records.Count > 5)
            {
                records.RemoveAt(0);
            }

            // save last record z position to world generation manager to avoid removing segments where player can spawn too
            WorldGenerationManager.Instance.lastRecordZPosition = records.OrderBy(r => r.position.z).First().position.z;
        }




        /// <summary>
        /// Load the last recorded player position and velocity
        /// and apply it to the player with a smooth transition
        /// </summary>
        public void Load()
        {
            StartCoroutine(LoadCoroutine());
        }
        private IEnumerator LoadCoroutine()
        {
            _disableRecord = true;
            var player = GetComponent<Controller>();
            player.ControlReleased = true;
            TimeScaleManager.Instance.PauseGame();
            player.Animator.speed = 0f;

            yield return new WaitForSecondsRealtime(0.2f);

            // ~make sure we dont load the current position as sometime 
            // the Record is taken from the current frame
            records = records.FindAll(r => Vector3.Distance(r.position, player.transform.position) > 1f);
            var record = records.LastOrDefault();
            
            // remove used record, this prevent respawn loop if player die again right after respawn
            if(records.Count > 1) records.Remove(record); 

            // Restore player position and velocity 
            // and stop all particle effects
            // and lane change coroutine if any
            player.Rb.linearVelocity = record.rbVelocity;
            player.sm.GetState<MoveState>().targetXPosition = Mathf.RoundToInt(record.position.x);
            player.sm.GetState<SlideState>().OnRelease();
            while( Vector3.Distance(player.transform.position, record.position) > 0.1f )
            {
                player.transform.position = Vector3.MoveTowards(
                    player.transform.position, 
                    record.position, 
                    60f* Time.unscaledDeltaTime
                );
                yield return null;
            }

            UiRegistry.Countdown.Run();
            yield return new WaitUntil(() => UiRegistry.Countdown.animationFinished == true);

            TimeScaleManager.Instance.ResumeGame();
            player.Animator.speed = 1f;
            player.ControlReleased = false;
            _disableRecord = false;
            yield return null;
        }
    }
}