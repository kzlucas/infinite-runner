using UnityEngine;
using StateMachine;
using Components.Audio.Scripts;
using Components.Player;

namespace Player.States
{
    public class JumpState : BaseState
    {


        [Header("References")]
        private Controller player;
        


        public JumpState(StateMachine.StateMachine stateMachine, Controller player) 
            : base(stateMachine, "Jump")
        {
            this.player = player;
        }


        public override void OnEnter()
        {
            // Play jump sound
            AudioManager.Instance.PlaySound("jump");

            // Start jump animation
            player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", true);

            // Physic jump
            player.Rb.linearVelocity = Vector3.zero;
            player.Rb.AddForce(Vector3.up * player.JumpHeight, ForceMode.Impulse);

            // Play jump particles
            PlayJumpParticules(0.3f);
        }


        /// <summary>
        ///  Play jump particules at specified offset
        /// </summary>
        private void PlayJumpParticules(float zOffset = 0f)
        {
            player.JumpParticles.transform.parent = null; // detach from player
            player.JumpParticles.transform.position = player.transform.position + Vector3.up * zOffset; // slightly above ground
            player.JumpParticles.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, player.Rb.linearVelocity.z);
            player.JumpParticles.Play();
        }

    }

}