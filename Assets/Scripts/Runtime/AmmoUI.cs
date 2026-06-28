using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class AmmoUI : MonoBehaviour
    {
        public Text ammoText;
        public Weapon weaponToTrack;

        void Update()
        {
            if (weaponToTrack != null && weaponToTrack.isEquipped)
            {
                // Si la munición es 0 o menos, mostramos el aviso
                if (weaponToTrack.currentAmmo <= 0)
                {
                    ammoText.text = "SIN BALAS";
                    // Opcional: Podrías cambiar el color a rojo
                    ammoText.color = Color.red;
                }
                else
                {
                    // Si tenemos balas, mostramos el contador normal y color blanco
                    ammoText.text = "Balas: " + weaponToTrack.currentAmmo;
                    ammoText.color = Color.white;
                }
            }
            else
            {
                ammoText.text = "";
            }
        }
    }
}