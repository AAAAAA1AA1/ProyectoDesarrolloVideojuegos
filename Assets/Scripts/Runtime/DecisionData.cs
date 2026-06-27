using UnityEngine;

namespace JuegoMental
{
    [CreateAssetMenu(fileName = "NuevaDecision", menuName = "JuegoMental/Decision")]
    public class DecisionData : ScriptableObject
    {
        [TextArea] public string questionText;
        public string optionAText;
        public string optionBText;
        public bool isOptionACorrect;

        [Header("Efectos")]
        public float cortisolChange = -20f;
        public int ammoReward = 10;
        public int enemiesToSpawn = 0;
    }
}