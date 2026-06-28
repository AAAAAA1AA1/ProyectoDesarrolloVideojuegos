using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace JuegoMental
{
    public class LevelManager : MonoBehaviour
    {
        public int totalSignosNecesarios = 5;
        private int _signosEncontrados = 0;
        public GameObject exitDoor;
        public GameObject mensajeInicio;
        public Text textoComponente;
        private bool _juegoTerminado = false;

        [Header("Configuración de Tiempo")]
        public float timeLimit = 60f;

        void Start()
        {
            if (exitDoor != null) exitDoor.SetActive(false);
            MostrarMensaje("Encuentra los 5 '?' y cuidado con los enemigos.", 5f);
        }

        public void RegistrarSignoEncontrado(string mensajeRecompensa)
        {
            if (_juegoTerminado) return;
            _signosEncontrados++;

            if (_signosEncontrados < totalSignosNecesarios)
                MostrarMensaje(mensajeRecompensa + "\nSignos: " + _signosEncontrados + "/" + totalSignosNecesarios, 4f);
            else
            {
                MostrarMensaje("¡Objetivo cumplido! " + mensajeRecompensa + "\nAhora encuentra la puerta.", 6f);
                if (exitDoor != null) exitDoor.SetActive(true);
            }
        }

        public void MostrarMensaje(string texto, float duracion)
        {
            if (_juegoTerminado) return;
            if (mensajeInicio != null && textoComponente != null)
            {
                StopAllCoroutines();
                textoComponente.text = texto;
                StartCoroutine(MostrarMensajeTemporal(duracion));
            }
        }

        IEnumerator MostrarMensajeTemporal(float duracion)
        {
            mensajeInicio.SetActive(true);
            yield return new WaitForSeconds(duracion);
            mensajeInicio.SetActive(false);
        }

        public void EsconderMensajes()
        {
            _juegoTerminado = true;
            StopAllCoroutines();
            if (mensajeInicio != null) mensajeInicio.SetActive(false);
        }
    }
}