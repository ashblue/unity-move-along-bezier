using UnityEngine;

namespace CleverCrow.Curves {
    public static class Bezier {
        public static Vector3 GetPointQuadratic (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return oneMinusT * oneMinusT * p0
                   + 2f * oneMinusT * t * p1
                   + t * t * p2;
        }
        
        public static Vector3 GetFirstDerivativeQuadratic (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            return 2f * (1f - t) * (p1 - p0)
                   + 2f * t * (p2 - p1);
        }

        public static Vector3 GetPointCubic (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return oneMinusT * oneMinusT * oneMinusT * p0 +
                   3f * oneMinusT * oneMinusT * t * p1 +
                   3f * oneMinusT * t * t * p2 +
                   t * t * t * p3;
        }
        
        public static Vector3 GetFirstDerivativeCubic (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return 3f * oneMinusT * oneMinusT * (p1 - p0) +
                   6f * oneMinusT * t * (p2 - p1) +
                   3f * t * t * (p3 - p2);
        }
        
        public static Vector3 GetPointLerp (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            var pointA = Vector3.Lerp(p0, p1, t);
            var pointB = Vector3.Lerp(p1, p2, t);
            
            return Vector3.Lerp(pointA, pointB, t);
        }
    }
}