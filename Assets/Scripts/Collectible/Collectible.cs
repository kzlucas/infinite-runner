using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Collectible : MonoBehaviour
{
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

        if(SoundToPlayOnCollection != "")
        {
            AudioManager.Instance?.PlaySound(SoundToPlayOnCollection);
        }

        OnCollide();
        
        return this;
    }
}