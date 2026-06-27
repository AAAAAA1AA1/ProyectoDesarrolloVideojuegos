using UnityEngine;
using TMPro;

namespace JuegoMental
{
    public class LevelTimer : MonoBehaviour
    {
        public float timeLimit = 60f;
        public TextMeshProUGUI timerDisplayText;
        public GameOverUI gameOverManager; // Arrastra aquí el objeto con el script GameOverUI

        private float _timeRemaining;
        private bool _isLevelActive = true;

        void Start() => _timeRemaining = timeLimit;

        void Update()
        {
            if (!_isLevelActive) return;

            _timeRemaining -= Time.deltaTime;
            UpdateUIText();

            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                _isLevelActive = false;
                if (gameOverManager != null) gameOverManager.ShowByTime();
            }
        }

        void UpdateUIText()
        {
            if (timerDisplayText != null)
                timerDisplayText.text = string.Format("Tiempo: {0:00}:{1:00}", Mathf.FloorToInt(_timeRemaining / 60), Mathf.FloorToInt(_timeRemaining % 60));
        }
    }
}