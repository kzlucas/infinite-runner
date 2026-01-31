using System.Collections;
using Components.Audio.Scripts;
using Components.UI.Scripts;
using Player.States;
using UnityEngine;

namespace Components.Player
{
    [RequireComponent(typeof(Controller))]
    public class Health : MonoBehaviour, IDamageable
    {

        [Header("Dependencies")]
        private UiRegistry UiRegistry => ServiceLocator.Scripts.ServiceLocator.Get<UiRegistry>();
        private PlayerHistory History => ServiceLocator.Scripts.ServiceLocator.Get<PlayerHistory>();


        [Header("Health Settings")]
        public int currentHealth { get; set; }
        public int maxHealth { get; set; } = 10;
        public bool isInvincible = false;
        public bool isDie = false;


        void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (isInvincible || isDie) return;

            // Play crash sound
            AudioManager.Instance.PlaySound("crash");

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            
            // brief invincibility after taking damage to prevent multiple hits
            // as multiple colliders can hit at same frame
            InvincibleForSeconds(.2f); 

            UiRegistry.Hud.UpdateHp((float)currentHealth / maxHealth);
            UiRegistry.ScreenOverlay.Flash("red");

            
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                History.Load();
            }
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UiRegistry.Hud.UpdateHp((float)currentHealth / maxHealth);
        }

        public void Die()
        {
            isDie = true;
            Debug.Log("[PlayerHealth] Player has died.");
            Utils.PlayerController.sm.TransitionTo<CrashState>();
        }

        private void InvincibleForSeconds(float seconds)
        {
            StartCoroutine(InvincibleCoroutine(seconds));
        }

        private IEnumerator InvincibleCoroutine(float seconds)
        {
            isInvincible = true;
            yield return new WaitForSecondsRealtime(seconds);
            isInvincible = false;
        }

    }
}
