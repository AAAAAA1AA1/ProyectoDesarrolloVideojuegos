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
        public float Value => _model.Value;
        public float Max => _model.Max;
        public float Fraction => _model.Value / _model.Max;

        void Awake()
        {
            _model = new CortisolModel(max);
        }

        public void Add(float delta)
        {
            if (_model.IsLost) return;
            _model.Add(delta);
            OnChanged?.Invoke();
            if (_model.IsLost) OnLost?.Invoke();
        }
    }
}
