using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class StressBarUI : MonoBehaviour
    {
        public CortisolSystem cortisol;
        public Image fill; // Image type Filled, Horizontal

        void OnEnable()
        {
            if (cortisol != null) cortisol.OnChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (cortisol != null) cortisol.OnChanged -= Refresh;
        }

        void Refresh()
        {
            if (fill != null && cortisol != null) fill.fillAmount = cortisol.Fraction;
        }
    }
}
