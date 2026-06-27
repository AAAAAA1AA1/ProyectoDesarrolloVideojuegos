using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class GameOverUI : MonoBehaviour
    {
        public CortisolSystem cortisol; // Arrastra el objeto Player aquí
        public GameObject panel;        // Arrastra el panel de "Has perdido" aquí
        public UnityEngine.UI.Text statusText;
        void Start()
        {
            if (panel != null) panel.SetActive(false);
            if (cortisol != null) cortisol.OnLost += ShowByStress;
        }

        void OnDestroy()
        {
            if (cortisol != null) cortisol.OnLost -= ShowByStress;
        }

        public void ShowByStress() => Show("¡Estrés al máximo!");
        public void ShowByTime() => Show("¡Se acabó el tiempo!");

        void Show(string message)
        {
            if (statusText != null) statusText.text = message;
            if (panel != null) panel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}