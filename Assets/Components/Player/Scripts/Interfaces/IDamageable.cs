
namespace Components.Player
{

    public interface IDamageable
    {
        int CurrentHealth { get; set; }
        int MaxHealth { get; set; }

        void TakeDamage(int damage);
        void Die();


    }
}