using System;
using UnityEngine;

namespace CleverCrow.Curves {
    [Serializable]
    public class CurvePoint {
        public Transform transform;
        
        [SerializeField]
        private Vector3 _tangent = Vector3.right;

        [SerializeField]
        private CurveMode _mode;
        
        public Vector3 Tangent {
            get {
                switch (_mode) {
                    case CurveMode.Free:
                        return _tangent;
                    case CurveMode.StraightLine:
                        return Vector3.zero;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set => _tangent = value;
        }

        public CurveMode Mode {
            get => _mode;
            set => _mode = value;
        }
        
        public Vector3 Position => transform == null ? Vector3.zero : transform.position;
    }
}