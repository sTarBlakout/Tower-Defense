public interface IDamageable
{
    float GetCurrentHealth();

    void Die();

    void SelfDestroy(bool instantly);

    void TakeDamage(float damage);

    bool IsDead();
}
