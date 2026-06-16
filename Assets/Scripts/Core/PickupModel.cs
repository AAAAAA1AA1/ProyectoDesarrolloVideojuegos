namespace JuegoMental.Core
{
    /// <summary>Objeto recogible. amount es siempre positivo; el tipo decide el signo.</summary>
    public class PickupModel
    {
        public PickupKind Kind { get; }
        public float Amount { get; }

        public PickupModel(PickupKind kind, float amount)
        {
            Kind = kind;
            Amount = amount;
        }

        public void ApplyTo(CortisolModel cortisol)
        {
            float delta = Kind == PickupKind.Bad ? Amount : -Amount;
            cortisol.Add(delta);
        }
    }
}
