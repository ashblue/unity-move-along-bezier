using UnityEngine;

namespace CleverCrow.Curves {
    [System.Serializable]
    public class CurvePoint {
        public Transform transform;
        public Vector3 tangent = Vector3.right;

        public Vector3 Position => transform == null ? Vector3.zero : transform.position;
    }
}