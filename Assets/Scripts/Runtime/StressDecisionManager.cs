using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace JuegoMental
{
    public class StressDecisionManager : MonoBehaviour
    {
        [Header("UI")]
        public GameObject decisionPanel;
        public Text questionText;
        public Text timerText;
        public Text textOptionA;
        public Text textOptionB;

        [Header("Configuración")]
        public float timeLimit = 7f;
        private float _timeLeft;
        private DecisionData _currentData;

        [Header("Referencias")]
        public CortisolSystem playerCortisol;
        public Weapon playerWeapon;

        public void StartQuestion(DecisionData data)
        {
            _currentData = data;
            decisionPanel.SetActive(true);

            questionText.text = data.questionText;
            textOptionA.text = data.optionAText;
            textOptionB.text = data.optionBText;

            _timeLeft = timeLimit;
            Time.timeScale = 0f;
            StartCoroutine(TimerRoutine());
        }

        IEnumerator TimerRoutine()
        {
            while (_timeLeft > 0)
            {
                _timeLeft -= Time.unscaledDeltaTime;
                timerText.text = "Tiempo: " + Mathf.Ceil(_timeLeft).ToString();
                yield return null;
            }
            // Tiempo agotado: enviamos false (no eligió A) y true (fue por timeout)
            HandleResult(false, true);
        }

        public void HandleResult(bool playerChoseA, bool isTimeOut = false)
        {
            StopAllCoroutines();
            decisionPanel.SetActive(false);
            Time.timeScale = 1f;

            LevelManager lm = FindObjectOfType<LevelManager>();
            string resultadoTexto = "";

            if (isTimeOut)
            {
                playerCortisol.Add(35f);
                resultadoTexto = "No respondido: + Estrés.";
            }
            else if (playerChoseA == _currentData.isOptionACorrect)
            {
                playerCortisol.Add(_currentData.cortisolChange);
                playerWeapon.currentAmmo += _currentData.ammoReward;
                resultadoTexto = "¡Bien! Estrés disminuido, +" + _currentData.ammoReward + " balas.";
            }
            else
            {
                playerCortisol.Add(35f);
                if (_currentData.enemiesToSpawn > 0)
                {
                    EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
                    if (spawner != null) spawner.SpawnExtraEnemies(_currentData.enemiesToSpawn);
                }
                resultadoTexto = "¡Error! Estrés aumentado.";
            }

            // Enviamos el resultado al LevelManager
            if (lm != null) lm.RegistrarSignoEncontrado(resultadoTexto);
        }

        public void OnClickOptionA()
        {
            HandleResult(true, false);
        }

        // Función para el botón B
        public void OnClickOptionB()
        {
            HandleResult(false, false);
        }
    }
}