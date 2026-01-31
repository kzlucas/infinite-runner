using System;
using System.Collections;
using StateMachine;
using UnityEngine;

namespace Player.States
{
    public class MoveState : BaseState
    {
        private Controller player;

        private bool hasReleasedKey = true;

        public int targetXPosition = 0;

        /// <summary> Input direction for movement along X axis (-1 left, 1 right)</summary>
        public Vector2 inputMoveDir = Vector2.zero;


        /// <summary> Reference to the lane change coroutine</summary>
        private IEnumerator goToLaneRoutine;


        public MoveState(StateMachine.StateMachine stateMachine, Controller player)
            : base(stateMachine, "Move")
        {
            this.player = player;
        }


        public override void OnEnter()
        {
            if (inputMoveDir.x == 0) return;

            // Update target X position based on input direction
            if (hasReleasedKey && inputMoveDir.x != 0)
            {
                hasReleasedKey = false; // force player to release key before next input
                var reqTargetXPosition = targetXPosition + (inputMoveDir.x > 0 ? 2 : -2); 
                targetXPosition = Mathf.Clamp(reqTargetXPosition, player.minX, player.maxX);

                // (re)start coroutine
                if(goToLaneRoutine != null)
                {
                    player.StopCoroutine(goToLaneRoutine);
                }
                goToLaneRoutine = GoToLaneRoutine();
                player.StartCoroutine(goToLaneRoutine);
            }
        }

        public void OnRelease()
        {
            hasReleasedKey = true;
        }


        /// <summary>
        /// Move the player to the target lane smoothly
        /// </summary>
        /// <returns></returns>
        private IEnumerator GoToLaneRoutine()
        {
            player.isChangingLane = true;

            // Play lane change sfx effect
            var sign = targetXPosition - player.transform.position.x > 0 ? 1 : -1;
            if (sign > 0) player.laneChangeParticles.transform.rotation = Quaternion.Euler(-90, 0, 80);
            else player.laneChangeParticles.transform.rotation = Quaternion.Euler(-90, 0, -80);
            player.laneChangeParticles.Play();


            // Handle case where player is already very close to target
            float initialDistance = Math.Abs(targetXPosition - player.transform.position.x);
            if (initialDistance < 0.1f)
            {
                player.rb.position = new Vector3(targetXPosition, player.rb.position.y, player.rb.position.z);
                player.rb.linearVelocity = new Vector3(0f, player.rb.linearVelocity.y, player.rb.linearVelocity.z);
                yield break;
            }

            while (Math.Abs(targetXPosition - player.transform.position.x) > 0.05f)
            {
                if (player.controlReleased) yield break;

                // Stop particles when close enough
                if (Math.Abs(targetXPosition - player.transform.position.x) < 0.5f)
                    player.laneChangeParticles.Stop();

                float currentDistance = Math.Abs(targetXPosition - player.transform.position.x);
                float direction = targetXPosition - player.transform.position.x > 0 ? 1 : -1;

                // Ease-out movement: faster when far, slower when close
                float easeMultiplier = currentDistance / initialDistance;
                easeMultiplier = Mathf.Clamp(easeMultiplier, 0.3f, 1f); // minimum 30% speed to avoid getting stuck

                float targetSpeed = direction * easeMultiplier * player.xMoveSpeed;

                // Prevent overshooting by capping speed when very close
                if (currentDistance < 0.5f)
                {
                    float maxSpeedForDistance = currentDistance * 10f; // max speed based on distance
                    targetSpeed = Mathf.Clamp(targetSpeed, -maxSpeedForDistance, maxSpeedForDistance);
                }

                player.rb.linearVelocity = new Vector3(targetSpeed, player.rb.linearVelocity.y, player.rb.linearVelocity.z);
                yield return null;
            }

            if (player.controlReleased) yield break;

            // Snap to exact position and stop horizontal movement
            player.rb.linearVelocity = new Vector3(0f, player.rb.linearVelocity.y, player.rb.linearVelocity.z);
            player.rb.position = new Vector3(targetXPosition, player.rb.position.y, player.rb.position.z);
            player.isChangingLane = false;
        }

    }
}