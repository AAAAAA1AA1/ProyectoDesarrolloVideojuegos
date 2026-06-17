using UnityEngine;

namespace JuegoMental
{
    /// <summary>Muestra el panel de derrota cuando el cortisol llega al maximo.</summary>
    public class GameOverUI : MonoBehaviour
    {
        public CortisolSystem cortisol;
        public GameObject panel;

        bool _shown;

        void Start()
        {
            if (panel != null) panel.SetActive(false);
            if (cortisol != null) cortisol.OnLost += Show;
        }

        void OnDestroy()
        {
            if (cortisol != null) cortisol.OnLost -= Show;
        }

        void Show()
        {
            if (_shown) return;
            _shown = true;
            if (panel != null) panel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
