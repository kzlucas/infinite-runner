using Components.Audio.Scripts;
using UnityEngine;


namespace Components.Collectible
{
    [RequireComponent(typeof(Collider))]
    public abstract class Collectible : MonoBehaviour
    {


        [Header("Collectible Settings")]
        public Animator animator;
        public string SoundToPlayOnCollection = "";
        public abstract void OnCollide();
        public bool IsCollected = false;


        /// <summary>
        ///    Defines behavior when collided.
        /// </summary>

        public virtual Collectible TriggerCollision()
        {
            if (animator != null)
            {
                animator.SetTrigger("OnCollide");
            }

            if (SoundToPlayOnCollection != "")
            {
                AudioManager.Instance?.PlaySound(SoundToPlayOnCollection);
            }

            OnCollide();

            return this;
        }
    }
}