using Components.Audio.Scripts;
using Components.ServiceLocator.Scripts;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Collectible : MonoBehaviour
{
    [Header("Dependencies")]
    private static AudioManager AudioManager => ServiceLocator.Get<AudioManager>();


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

        if(SoundToPlayOnCollection != "")
        {
            AudioManager?.PlaySound(SoundToPlayOnCollection);
        }

        OnCollide();
        
        return this;
    }
}