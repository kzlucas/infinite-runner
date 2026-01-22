using UnityEngine;
using StateMachine;

namespace Player.States
{
    public class JumpState : BaseState
    {
        private Controller player;
        

        public JumpState(StateMachine.StateMachine stateMachine, Controller player) 
            : base(stateMachine, "Jump")
        {
            this.player = player;
        }


        public override void OnEnter()
        {
            player.currentJumpCount++;

            // Play jump sound
            AudioManager.Instance.PlaySound("jump");

            // Start jump animation
            player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", true);

            // Physic jump
            player.rb.linearVelocity = Vector3.zero;
            player.rb.AddForce(Vector3.up * player.jumpHeight, ForceMode.Impulse);

            // Play jump particles
            PlayJumpParticules(0.3f);
        }


        /// <summary>
        ///  Play jump particules at specified offset
        /// </summary>
        private void PlayJumpParticules(float zOffset = 0f)
        {
            player.jumpParticles.transform.parent = null; // detach from player
            player.jumpParticles.transform.position = player.transform.position + Vector3.up * zOffset; // slightly above ground
            player.jumpParticles.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, player.rb.linearVelocity.z);
            player.jumpParticles.Play();
        }

    }

}