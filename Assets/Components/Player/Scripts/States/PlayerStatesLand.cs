using UnityEngine;
using StateMachine;
using Components.Audio.Scripts;
using Components.Player;

namespace Player.States
{
    public class LandState : BaseState
    {


        [Header("References")]
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
            player.JumpParticles.transform.parent = player.transform;
        }


        /// <summary>
        ///  Play jump particules at specified offset
        /// </summary>
        private void PlayLandParticules(float zOffset = 0f)
        {
            player.JumpParticles.transform.parent = null; // detach from player
            player.JumpParticles.transform.position = player.transform.position + Vector3.up * zOffset; // slightly above ground
            player.JumpParticles.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, player.Rb.linearVelocity.z);
            player.JumpParticles.Play();
        }

    }
    
}