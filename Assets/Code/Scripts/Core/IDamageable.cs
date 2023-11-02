namespace Blooding.Runtime.Core
{
    public interface IDamageable
    {
        void Damage(DamageArgs args);

        static event System.Action<DamageArgs> DamageEvent;

        protected static void OnDamage(DamageArgs args) => DamageEvent?.Invoke(args);
    }
}