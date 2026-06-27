using UnityEngine;

namespace JuegoMental
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemyPrefab; // Arrastra aquí tu enemigo desde los assets
        public Transform[] spawnPoints; // Crea puntos vacíos en tu escena para que aparezcan ahí

        public void SpawnExtraEnemies(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                // Elige un punto de aparición al azar
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // Crea al enemigo
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            }
            Debug.Log("¡Se han invocado " + amount + " enemigos!");
        }
    }
}