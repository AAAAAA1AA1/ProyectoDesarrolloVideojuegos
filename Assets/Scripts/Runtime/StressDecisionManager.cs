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
                timerText.text = "Tiempo: "+Mathf.Ceil(_timeLeft).ToString();
                yield return null;
            }
            HandleResult(false); // Tiempo agotado = falla
        }

        public void HandleResult(bool playerChoseA)
        {
            StopAllCoroutines();
            decisionPanel.SetActive(false);
            Time.timeScale = 1f;

            if (playerChoseA == _currentData.isOptionACorrect)
            {
                // RESPUESTA CORRECTA
                playerCortisol.Add(_currentData.cortisolChange);
                playerWeapon.currentAmmo += _currentData.ammoReward;
            }
            else
            {
                // RESPUESTA INCORRECTA
                playerCortisol.Add(35f);

                // Lógica de enemigos
                if (_currentData.enemiesToSpawn > 0)
                {
                    EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
                    if (spawner != null)
                    {
                        spawner.SpawnExtraEnemies(_currentData.enemiesToSpawn);
                    }
                }
                Debug.Log("Respuesta incorrecta: Estrés aumentado y enemigos invocados");
            }
        }
    }
}