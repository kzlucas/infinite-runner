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

        [Header("Health Settings")]
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; } = 10;
        public bool IsInvincible = false;
        public bool IsDie = false;


        void Start()
        {
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (IsInvincible || IsDie) return;

            // Play crash sound
            AudioManager.Instance.PlaySound("crash");

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            
            // brief invincibility after taking damage to prevent multiple hits
            // as multiple colliders can hit at same frame
            InvincibleForSeconds(.2f); 

            UiRegistry.Instance.Hud.UpdateHp((float)CurrentHealth / MaxHealth);
            UiRegistry.Instance.ScreenOverlay.Flash("red");

            
            if (CurrentHealth <= 0)
            {
                Die();
            }
            else
            {
                var _history = Utils.PlayerController.GetComponent<PlayerHistory>();
                _history.Load();
            }
        }

        public void Heal(int amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            UiRegistry.Instance.Hud.UpdateHp((float)CurrentHealth / MaxHealth);
        }

        public void Die()
        {
            IsDie = true;
            Debug.Log("[PlayerHealth] Player has died.");
            Utils.PlayerController.sm.TransitionTo<CrashState>();
        }

        private void InvincibleForSeconds(float seconds)
        {
            StartCoroutine(InvincibleCoroutine(seconds));
        }

        private IEnumerator InvincibleCoroutine(float seconds)
        {
            IsInvincible = true;
            yield return new WaitForSecondsRealtime(seconds);
            IsInvincible = false;
        }

    }
}
