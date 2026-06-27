using UnityEngine;

namespace JuegoMental
{
    public class InstruccionesUI : MonoBehaviour
    {
        public GameObject panelInstrucciones;
        public GameObject botonInterrogacion;


        void Start()
        {
            
            if (panelInstrucciones != null)
                botonInterrogacion.SetActive(false);


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