using UnityEngine;

namespace JuegoMental
{
    public class Weapon : MonoBehaviour
    {
        [Header("Configuración de Disparo")]
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float fireRate = 0.5f;
        public float bulletSpeed = 10f;

        [Header("Gestión de Estrés")]
        public CortisolSystem playerCortisol;
        public float stressPerShot = 2.0f;
        public float stressEmptyPenalty = 5.0f;

        [Header("Munición")]
        public int currentAmmo = 20;
        public int maxAmmo = 20;

        [Header("Estado")]
        public bool isEquipped = false;

        private float _nextFireTime;
        private PlayerController2D _playerController;

        void Awake()
        {
            _playerController = GetComponentInParent<PlayerController2D>();
        }

        void Update()
        {
            if (isEquipped && Input.GetButtonDown("Fire1") && Time.time >= _nextFireTime)
            {
                if (currentAmmo > 0)
                {
                    Shoot();
                    _nextFireTime = Time.time + fireRate;
                }
                else
                {
                    if (playerCortisol != null)
                        playerCortisol.Add(stressEmptyPenalty);
                }
            }
        }

        void Shoot()
        {
            if (bulletPrefab == null || firePoint == null) return;

            // 1. Instanciamos en la posición del firePoint, pero SIN usar su rotación
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            PlayerBullet bullet = bulletObj.GetComponent<PlayerBullet>();

            if (bullet != null)
            {
                // 2. Determinamos la dirección horizontal pura:
                // Si el padre tiene escala negativa, disparamos a la izquierda, si no, a la derecha.
                float dirX = (transform.root.lossyScale.x >= 0) ? 1f : -1f;

                // 3. Forzamos velocidad en X, 0 en Y (esto elimina la diagonal)
                bullet.velocity = new Vector2(dirX * bulletSpeed, 0);
            }
            currentAmmo--;
        }
    }
}