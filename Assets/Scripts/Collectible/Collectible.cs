using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    public Animator animator;
    public string OnCollectedSoundLabel = "";


    /// <summary>
    ///    Defines behavior when collided.
    /// </summary>

    public virtual Collectible OnCollide()
    {
        if (animator != null)
        {
            animator.SetTrigger("OnCollide");
        }

        if(OnCollectedSoundLabel != "")
        {
            AudioManager.Instance?.PlaySound(OnCollectedSoundLabel);
        }
        
        return this;
    }
}