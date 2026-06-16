using UnityEngine;

namespace JuegoMental
{
    [CreateAssetMenu(menuName = "Juego Mental/Level Config", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int floor = 1;
        public int patrolEnemies = 2;
        public int chaserEnemies = 0;
        public float cortisolMax = 100f;
        public float enemyContactCortisol = 15f;
    }
}
