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

        [SerializeField] 
        private Vector3 _position;
        
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

        public Vector3 Position {
            get => transform == null ? _position : transform.position;
            set => _position = value;
        }

        public void SetRelativeTangent (Vector3 target) {
            var heading = target - Position;
            var x = Mathf.Clamp(Mathf.Round(heading.x), -1, 1);
            var z = Mathf.Clamp(Mathf.Round(heading.z), -1, 1);
            
            Tangent = new Vector3(x, 0, z);
        }
    }
}