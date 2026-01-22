using UnityEngine;
using StateMachine;

namespace Player.States
{
    public class LandState : BaseState
    {
        private Controller player;

        public LandState(StateMachine.StateMachine stateMachine, Controller player) 
            : base(stateMachine, "Land")
        {
            this.player = player;
        }

        public override void OnEnter()
        {
            if (player == null) return;

            // Play land sound
            AudioManager.Instance.PlaySound("land");

            // Play again jump particles on landing
            PlayLandParticules();

            // reattach ps to player to clean up hierarchy
            player.jumpParticles.transform.parent = player.transform;

            // Reset jump count when landing
            player.currentJumpCount = 0;
        }


        /// <summary>
        ///  Play jump particules at specified offset
        /// </summary>
        private void PlayLandParticules(float zOffset = 0f)
        {
            player.jumpParticles.transform.parent = null; // detach from player
            player.jumpParticles.transform.position = player.transform.position + Vector3.up * zOffset; // slightly above ground
            player.jumpParticles.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, player.rb.linearVelocity.z);
            player.jumpParticles.Play();
        }

    }
    
}