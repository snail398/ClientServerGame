using System;
using UnityEngine;

namespace Shared {
    public class Rotator : MonoBehaviour {
        [SerializeField] 
        private Transform _Transform;

        private void OnValidate() {
            _Transform = GetComponent<Transform>();
        }

        private void Update() {
            _Transform.rotation *= Quaternion.AngleAxis(1, Vector3.up);
        }
    }
}