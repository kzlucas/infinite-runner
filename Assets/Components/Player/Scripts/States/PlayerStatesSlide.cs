using System.Collections;
using Components.Audio.Scripts;
using StateMachine;
using UnityEngine;
using Components.Player;

namespace Player.States
{
    public class SlideState : BaseState
    {

        [Header("References")]
        private Controller player;

        /// <summary> Reference to the player's collider</summary>
        private BoxCollider bCollider;

        /// <summary> Reference to the player's collider normal dimensions: height</summary>
        private float originalColliderHeight;

        /// <summary> Reference to the player's collider normal dimensions: center Y position</summary>
        private float originalColliderCenterY;


        public SlideState(StateMachine.StateMachine stateMachine, Controller player)
            : base(stateMachine, "Slide")
        {
            this.player = player;

            // store original body collider size (will update when sliding)
            bCollider = player.CollisionHandler.BodyCollider.GetComponent<BoxCollider>();
            originalColliderHeight = bCollider.size.y;
            originalColliderCenterY = bCollider.center.y;
        }

        public override void OnEnter()
        {
            // (re)start coroutine
            if(player.SlideRoutine != null)
            {
                player.StopCoroutine(player.SlideRoutine);
            }
            player.SlideRoutine = SlideRoutine();
            player.StartCoroutine(player.SlideRoutine);
        }

        /// <summary>
        ///  Handle the slide action
        /// </summary>
        private IEnumerator SlideRoutine()
        {
            AudioManager.Instance.PlaySound("slide");

            player.IsSliding = true;
            player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", true);
            SetCollidersToSlidingPosition(); // @improve: match the real mesh dimension accross frames to avoid pass through obstacle colliders during animation transition

            // Play slide particles
            player.SlideParticles.Play();

            yield break;
        }


        public void OnRelease()
        {
            if ((player == null) || (!player.IsSliding))
                return;

            // stop slide
            player.IsSliding = false;
            player.transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", false);
            SetCollidersToNormalPosition();

            // disable slide particles
            player.SlideParticles.Stop();
        }


        /// <summary>
        /// Set the player's collider to the sliding position
        /// </summary>
        private void SetCollidersToSlidingPosition()
        {
            player.CollisionHandler.transform.localScale = new Vector3(1f, 0.5f, 1f);
            player.CollisionHandler.transform.localPosition = new Vector3(0f, .1f, 0f);
            bCollider.size = new Vector3(bCollider.size.x, originalColliderHeight / 2f, bCollider.size.z); // ~0.5
            bCollider.center = new Vector3(bCollider.center.x, bCollider.center.y - originalColliderHeight / 4f, bCollider.center.z); // ~0.3
        }


        /// <summary>
        /// Set the player's collider back to the normal position
        /// </summary>
        private void SetCollidersToNormalPosition()
        {
            player.CollisionHandler.transform.localScale = new Vector3(1f, 1f, 1f);
            player.CollisionHandler.transform.localPosition = Vector3.zero;
            bCollider.size = new Vector3(bCollider.size.x, originalColliderHeight, bCollider.size.z);
            bCollider.center = new Vector3(bCollider.center.x, originalColliderCenterY, bCollider.center.z);
        }
    }

}