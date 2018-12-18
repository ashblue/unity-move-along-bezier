using System;
using UnityEngine;

namespace CleverCrow.Curves {
    [Serializable]
    public class CurvePoint {
        public Transform transform;
        
        [SerializeField]
        private Vector3 _tangentA = Vector3.left;
        
        [SerializeField]
        private Vector3 _tangentB = Vector3.right;

        [SerializeField]
        private CurveMode _mode;

        [SerializeField]
        private Vector3 _position;
        
        public Vector3 TangentA {
            get => EnforceTangentMode(_tangentA);
            set => _tangentA = value;
        }
        
        public Vector3 TangentB {
            get => EnforceTangentMode(_tangentB);
            set => _tangentB = value;
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
            
            TangentA = new Vector3(x, 0, z);
        }

        public Vector3 GetTangent (TangentPoint tangentPoint) {
            switch (tangentPoint) {
                case TangentPoint.A:
                    return TangentA;
                case TangentPoint.B:
                    return TangentB;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tangentPoint), tangentPoint, null);
            }
        }

        public void SetTangent (TangentPoint tangentPoint, Vector3 position) {
            switch (tangentPoint) {
                case TangentPoint.A:
                    TangentA = position;
                    break;
                case TangentPoint.B:
                    TangentB = position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tangentPoint), tangentPoint, null);
            }
        }

        private Vector3 EnforceTangentMode (Vector3 tangent) {
            switch (_mode) {
                case CurveMode.Free:
                    return tangent;
                case CurveMode.StraightLine:
                    return Vector3.zero;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}