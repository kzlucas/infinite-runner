using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Controller))]
    public class Health : MonoBehaviour, IDamageable
    {

        public int currentHealth { get; set; }
        public int maxHealth { get; set; } = 5;
        public bool isInvincible = false;
        public bool isDie = false;


        void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (isInvincible || isDie) return;
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            
            // brief invincibility after taking damage to prevent multiple hits
            // as multiple colliders can hit at same frame
            InvincibleForSeconds(.2f); 


            UiManager.Instance.screenOverlay.Flash("red");

            
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                var player = GetComponent<Controller>();
                player.history.Load();
            }
        }

        public void Die()
        {
            isDie = true;
            Debug.Log("[PlayerHealth] Player has died.");
            GetComponent<Controller>().TriggerCrashEvent();
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
