using System;
using UnityEngine;
using JuegoMental.Core;

namespace JuegoMental
{
    public class CortisolSystem : MonoBehaviour
    {
        public float max = 100f;
        public event Action OnChanged;
        public event Action OnLost;

        CortisolModel _model;

        public float Value => _model != null ? _model.Value : 0f;
        public float Max => _model != null ? _model.Max : max;
        public float Fraction => _model != null ? _model.Value / _model.Max : 0f;

        void Awake() => _model = new CortisolModel(max);

        public void Add(float delta)
        {
            if (_model == null || _model.IsLost) return;

            _model.Add(delta);
            OnChanged?.Invoke();

            // Verificación forzada de seguridad
            if (_model.Value >= max)
            {
                Debug.Log("Cortisol máximo alcanzado: Game Over");
                OnLost?.Invoke();
            }
        }
    }
}