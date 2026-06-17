using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class StressBarUI : MonoBehaviour
    {
        public CortisolSystem cortisol;
        public Image fill;       // Image type Filled, Horizontal
        public Text percent;

        void Update()
        {
            if (cortisol == null || fill == null) return;
            float f = cortisol.Fraction;
            fill.fillAmount = f;
            fill.color = Color.Lerp(new Color(0.95f, 0.82f, 0.2f), new Color(0.85f, 0.12f, 0.12f), f);
            if (percent != null) percent.text = Mathf.RoundToInt(f * 100f) + "%";
        }
    }
}
