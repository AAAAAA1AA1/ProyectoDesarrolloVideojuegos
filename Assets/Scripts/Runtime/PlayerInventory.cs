using UnityEngine;

namespace JuegoMental
{


    public class PlayerInventory : MonoBehaviour
    {
        public Animator anim;
        private bool _hasWeapon = false;

        // Esta función la llamas cuando tocas el arma
        public void EquipWeapon()
        {
            _hasWeapon = true;

            // 1. Activa la capa del arma (si tu ArmaLayer es la capa 1)
            anim.SetLayerWeight(1, 1f);

            // 2. Si usaras parámetros bool en lugar de capas:
            // anim.SetBool("IsHoldingWeapon", true);
        }
    }
}
