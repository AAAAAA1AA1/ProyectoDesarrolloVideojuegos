namespace JuegoMental.Core
{
    /// <summary>Estrés del jugador. Vida invertida: 0 = calma, max = derrota.</summary>
    public class CortisolModel
    {
        public float Max { get; }
        public float Value { get; private set; }
        public bool IsLost => Value >= Max;

        public CortisolModel(float max)
        {
            Max = max;
            Value = 0f;
        }

        /// <summary>delta positivo sube estrés, negativo lo baja. Clamp 0..Max.</summary>
        public void Add(float delta)
        {
            Value += delta;
            if (Value < 0f) Value = 0f;
            if (Value > Max) Value = Max;
        }
    }
}
