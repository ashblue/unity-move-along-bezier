using System;
using UnityEngine;

namespace CleverCrow.Curves {
    [Serializable]
    public class CurvePoint {
        public Transform transform;
        public bool isEndPoint;
        
        [SerializeField]
        private Vector3 _tangentA = Vector3.left;
        
        [SerializeField]
        private Vector3 _tangentB = Vector3.right;

        [SerializeField]
        private CurveMode _mode;

        [SerializeField]
        private Vector3 _position;
        
        public Vector3 TangentA => _tangentA;
        public Vector3 TangentB => _tangentB;

        public CurveMode Mode {
            get => _mode;
            set => _mode = value;
        }

        public Vector3 Position {
            get => isEndPoint && transform != null 
                ? transform.position : _position;
            set => _position = value;
        }

        public Vector3 GlobalPosition {
            get {
                if (transform == null) return Vector3.zero;
                
                return isEndPoint ? transform.position : transform.TransformPoint(_position);
            }
        }

        public void SetRelativeTangent (Vector3 target) {
            var heading = target - Position;
            var x = Mathf.Clamp(Mathf.Round(heading.x), -1, 1);
            var z = Mathf.Clamp(Mathf.Round(heading.z), -1, 1);
            
            _tangentA = new Vector3(x, 0, z);
        }

        public Vector3 GetTangent (TangentPoint tangentPoint) {
            if (Mode == CurveMode.StraightLine) return Vector3.zero;
            
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
                    _tangentA = position;
                    break;
                case TangentPoint.B:
                    _tangentB = position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tangentPoint), tangentPoint, null);
            }
        }
        
        public void EnforceTangents (TangentPoint currentTangent) {
            if (currentTangent == TangentPoint.None) currentTangent = TangentPoint.A;
            
            var siblingTangent = currentTangent == TangentPoint.A ? TangentPoint.B : TangentPoint.A;
            var enforcedTangent = GetTangent(currentTangent) * -1;

            switch (_mode) {
                case CurveMode.Free:
                    break;
                case CurveMode.StraightLine:
                    // Allow tangent retrieval to override so we don't overwrite it
                    break;
                case CurveMode.Aligned:
                    SetTangent(siblingTangent, enforcedTangent.normalized * Vector3.Distance(Vector3.zero, GetTangent(siblingTangent)));
                    break;
                case CurveMode.Mirrored:
                    SetTangent(siblingTangent, enforcedTangent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}