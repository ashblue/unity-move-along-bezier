using UnityEngine;

namespace CleverCrow.Curves {
    public class CurvePoint {
        public Vector3 Position { get; }
        
        public CurvePoint (Vector3 point) {
            Position = point;
        }
    }
}