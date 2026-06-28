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

        [Header("Munición")]
        public int currentAmmo = 20;

        [Header("Estado")]
        public bool isEquipped = false;

        private float _nextFireTime;
        private PlayerController2D _playerController;

        void Update()
        {
            // AÑADIMOS: La condición 'currentAmmo > 0' aquí
            if (isEquipped && currentAmmo > 0 && Input.GetButtonDown("Fire1") && Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + fireRate;
            }
        }

        void Shoot()
        {
            if (bulletPrefab == null || firePoint == null) return;

            // Asegurar referencia del jugador
            if (_playerController == null)
            {
                _playerController = transform.root.GetComponentInChildren<PlayerController2D>();
            }

            if (_playerController == null) return;

            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            PlayerBullet bullet = bulletObj.GetComponent<PlayerBullet>();

            if (bullet != null)
            {
                float dirX = _playerController.isFacingRight ? 1f : -1f;
                bullet.velocity = new Vector2(dirX * bulletSpeed, 0);
            }

            // Ahora, como el 'Update' ya impide disparar si es 0, 
            // esto solo se ejecutará cuando sea seguro restar.
            currentAmmo--;
        }
    }
}