namespace JuegoMental.Core
{
    public class EnemyHealth
    {
        public float Max { get; }
        public float Current { get; private set; }
        public bool IsDead => Current <= 0f;
        public float Fraction => Max <= 0f ? 0f : Current / Max;

        public EnemyHealth(float max)
        {
            Max = max;
            Current = max;
        }

        public void Damage(float amount)
        {
            Current -= amount;
            if (Current < 0f) Current = 0f;
        }
    }
}
