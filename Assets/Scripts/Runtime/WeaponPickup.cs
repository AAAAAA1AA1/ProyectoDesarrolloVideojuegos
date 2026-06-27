using UnityEngine;

namespace JuegoMental
{
    public class WeaponPickup : MonoBehaviour
    {
        [Header("Configuración de Equipamiento")]
        public Transform handSlot;      // Arrastra aquí el "WeaponSlot" del hueso
        public GameObject armaVisual;   // Arrastra el objeto que tiene el Sprite

        [Header("Ajustes de Posición")]
        public Vector3 offsetPosition = Vector3.zero; // Ajusta esto si el arma queda mal posicionada

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                // 1. Aseguramos que el arma visual se muestre
                if (armaVisual != null)
                {
                    armaVisual.SetActive(true);
                }

                // 2. Mover el arma al hueso y ajustar posición
                // IMPORTANTE: Si el objeto con este script ES armaVisual, esto funciona bien
                transform.SetParent(handSlot);
                transform.localPosition = offsetPosition;
                transform.localRotation = Quaternion.identity;

                // 3. Activar la capa del animador
                Animator anim = collision.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetLayerWeight(1, 1f);
                }

                // 4. Desactivar el trigger y el script de recogida para ahorrar recursos
                GetComponent<Collider2D>().enabled = false;
                this.enabled = false;

                Weapon weapon = GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapon.isEquipped = true; // Aquí activamos la capacidad de disparar
                }
            }
        }

      
    }
}