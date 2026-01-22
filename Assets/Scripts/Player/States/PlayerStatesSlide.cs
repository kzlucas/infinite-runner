using System.Collections;
using StateMachine;
using UnityEngine;

namespace Player.States
{
    public class SlideState : BaseState
    {
        private Controller player;

        public SlideState(StateMachine.StateMachine stateMachine, Controller player)
            : base(stateMachine, "Slide")
        {
            this.player = player;
        }

        public override void OnEnter()
        {
            player.slideRoutine.Replace(SlideRoutine());

        }


        /// <summary>
        ///  Handle the slide action
        /// </summary>
        private IEnumerator SlideRoutine()
        {
            AudioManager.Instance.PlaySound("slide");

            player.isSliding = true;
            player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", true);
            SetColliderToSlidingPosition(); // @improve: match the real mesh dimension accross frames to avoid pass through obstacle colliders during animation transition

            // Play slide particles
            player.slideParticles.Play();

            yield break;
        }


        public override void OnExit()
        {
            if ((player == null)
                || (player.controlReleased)
                || (!player.isSliding))
                return;

            // stop slide
            player.isSliding = false;
            player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", false);
            SetColliderToNormalPosition();

            // disable slide particles
            player.slideParticles.Stop();
        }


        /// <summary>
        /// Set the player's collider to the sliding position
        /// </summary>
        private void SetColliderToSlidingPosition()
        {
            player.bcollider.size = new Vector3(player.bcollider.size.x, player.originalColliderHeight / 2f, player.bcollider.size.z); // ~0.5
            player.bcollider.center = new Vector3(player.bcollider.center.x, player.bcollider.center.y - player.originalColliderHeight / 4f, player.bcollider.center.z); // ~0.3
        }


        /// <summary>
        /// Set the player's collider back to the normal position
        /// </summary>
        private void SetColliderToNormalPosition()
        {
            player.bcollider.size = new Vector3(player.bcollider.size.x, player.originalColliderHeight, player.bcollider.size.z);
            player.bcollider.center = new Vector3(player.bcollider.center.x, player.originalColliderCenterY, player.bcollider.center.z);
        }
    }

}