using UnityEngine;
using StateMachine;
using System;
using System.Collections;

namespace Player.States
{
    public class MoveState : BaseState
    {
        private Controller player;


        /// <summary> Reference to the lane change coroutine</summary>
        private IEnumerator goToLaneRoutine;


        public MoveState(StateMachine.StateMachine stateMachine, Controller player) 
            : base(stateMachine, "Move")
        {
            this.player = player;
        }



        public override void OnEnter()
        {
            Vector2 direction = player.inputMoveDir;

            // Do not process new input until reaching target position 
            // (0.1 tolerance to prevent input spam)
            if (Math.Abs(player.transform.position.x - player.targetXPosition) > 0.1f)
            {
                return;
            }

            // Update target X position based on input direction
            if (direction.x > 0 || (direction.x < 0))
            {
                var dirX = direction.x > 0 ? 1 : -1;
                player.targetXPosition = (int)Mathf.Round(player.transform.position.x + (dirX * 2f));
                player.targetXPosition = Mathf.Clamp(player.targetXPosition, player.minX, player.maxX);

                goToLaneRoutine.Replace(GoToLaneRoutine());
            }

        }


        /// <summary>
        /// Move the player to the target lane smoothly
        /// </summary>
        /// <returns></returns>
        private IEnumerator GoToLaneRoutine()
        {
            // Play lane change sfx effect
            var sign = player.targetXPosition - player.transform.position.x > 0 ? 1 : -1;
            if (sign > 0) player.laneChangeParticles.transform.rotation = Quaternion.Euler(-90, 0, 80);
            else player.laneChangeParticles.transform.rotation = Quaternion.Euler(-90, 0, -80);
            player.laneChangeParticles.Play();


            // Handle case where player is already very close to target
            float initialDistance = Math.Abs(player.targetXPosition - player.transform.position.x);
            if (initialDistance < 0.1f)
            {
                player.rb.position = new Vector3(player.targetXPosition, player.rb.position.y, player.rb.position.z);
                player.rb.linearVelocity = new Vector3(0f, player.rb.linearVelocity.y, player.rb.linearVelocity.z);
                yield break;
            }

            while (Math.Abs(player.targetXPosition - player.transform.position.x) > 0.05f)
            {
                if (player.controlReleased) yield break;

                // Stop particles when close enough
                if (Math.Abs(player.targetXPosition - player.transform.position.x) < 0.5f)
                    player.laneChangeParticles.Stop();

                float currentDistance = Math.Abs(player.targetXPosition - player.transform.position.x);
                float direction = player.targetXPosition - player.transform.position.x > 0 ? 1 : -1;

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
            player.rb.position = new Vector3(player.targetXPosition, player.rb.position.y, player.rb.position.z);

        }


    }
    
}