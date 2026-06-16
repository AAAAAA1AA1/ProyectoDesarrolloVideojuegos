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

        /// <summary>delta firmado a aplicar al cortisol: malo sube (+), bueno baja (-).</summary>
        public float Delta => Kind == PickupKind.Bad ? Amount : -Amount;

        public void ApplyTo(CortisolModel cortisol) => cortisol.Add(Delta);
    }
}
