using UnityEngine;

namespace JuegoMental
{
    public class InstruccionesUI : MonoBehaviour
    {
        public GameObject panelInstrucciones;
        public GameObject botonInterrogacion;

        void Start()
        {
            // Verificamos si ya se mostraron las instrucciones antes
            bool yaVioInstrucciones = PlayerPrefs.GetInt("InstruccionesVistas", 0) == 1;

            if (!yaVioInstrucciones)
            {
                // Si es la primera vez, mostramos instrucciones
                panelInstrucciones.SetActive(true);
                botonInterrogacion.SetActive(false);
                PlayerPrefs.SetInt("InstruccionesVistas", 1); // Marcamos como vistas
                PlayerPrefs.Save();
            }
            else
            {
                // Si ya las vio alguna vez, ocultamos panel y mostramos botón
                panelInstrucciones.SetActive(false);
                botonInterrogacion.SetActive(true);
            }
        }

        public void AbrirInstrucciones()
        {
            panelInstrucciones.SetActive(true);
            botonInterrogacion.SetActive(false);
        }

        public void CerrarInstrucciones()
        {
            panelInstrucciones.SetActive(false);
            botonInterrogacion.SetActive(true);
        }
    }
}