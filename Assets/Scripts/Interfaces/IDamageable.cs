public interface IDamageable
{
    int currentHealth { get; set; }
    int maxHealth { get; set; }

    void TakeDamage(int damage);
    void Die();


}