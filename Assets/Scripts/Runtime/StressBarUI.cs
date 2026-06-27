using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class StressBarUI : MonoBehaviour
    {
        public CortisolSystem cortisol;
        public Image fill; // Objeto con el componente Image (Type: Filled)
        public Text percent;

        void Update()
        {
            if (cortisol == null || fill == null) return;

            float f = cortisol.Fraction;

            // 1. Controlamos solo el llenado
            fill.fillAmount = f;

            // 2. Cambiamos el color según el nivel
            fill.color = Color.Lerp(new Color(0.95f, 0.82f, 0.2f), new Color(0.85f, 0.12f, 0.12f), f);

            // 3. Actualizamos texto si existe
            if (percent != null)
                percent.text = Mathf.RoundToInt(f * 100f) + "%";
        }
    }
}