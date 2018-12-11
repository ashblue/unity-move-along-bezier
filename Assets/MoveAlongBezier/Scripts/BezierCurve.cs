using UnityEngine;

namespace CleverCrow.Curves {
    public class BezierCurve : MonoBehaviour {
        public Vector3[] points;

        public Vector3 GetPoint (float t) {
            return transform.TransformPoint(Bezier.GetPointQuadratic(points[0], points[1], points[2], t));
        }

        public Vector3 GetVelocity (float t) {
            return transform.TransformPoint(Bezier.GetFirstDerivativeQuadratic(points[0], points[1], points[2], t)) -
                   transform.position;
        }

        public void Reset () {
            points = new[] {
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f)
            };
        }

        public Vector3 GetDirection (float t) {
            return GetVelocity(t).normalized;
        }
    }
}
