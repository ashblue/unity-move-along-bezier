using System;
using UnityEngine;

namespace CleverCrow.Curves {
    public class BezierSpline : MonoBehaviour {
        [SerializeField] 
        private BezierControlPointMode[] _modes;
        
        [SerializeField]
        private Vector3[] _points;

        public int ControlPointCount {
            get { return _points.Length; }
        }
        
        public int CurveCount {
            get { return (_points.Length - 1) / 3; }
        }

        public Vector3 GetControlPoint (int index) {
            return _points[index];
        }

        public void SetControlPoint (int index, Vector3 point) {
            _points[index] = point;
            EnforceMode(index);
        }

        public Vector3 GetPoint (float t) {
            int i;
            if (t >= 1f) {
                t = 1f;
                i = _points.Length - 4;
            } else {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            
            return transform.TransformPoint(Bezier.GetPointCubic(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t));
        }

        public Vector3 GetVelocity (float t) {
            int i;
            if (t >= 1f) {
                t = 1f;
                i = _points.Length - 4;
            } else {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            
            return transform.TransformPoint(Bezier.GetFirstDerivativeCubic(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t)) -
                   transform.position;
        }
        
        public Vector3 GetDirection (float t) {
            return GetVelocity(t).normalized;
        }

        public void AddCurve () {
            var point = _points[_points.Length - 1];
            Array.Resize(ref _points, _points.Length + 3);
            
            point.x += 1f;
            _points[_points.Length - 3] = point;
            point.x += 1f;
            _points[_points.Length - 2] = point;
            point.x += 1f;
            _points[_points.Length - 1] = point;
            
            Array.Resize(ref _modes, _modes.Length + 1);
            _modes[_modes.Length - 1] = _modes[_modes.Length - 2];
        }

        public BezierControlPointMode GetControlPointMode (int index) {
            return _modes[(index + 1) / 3];
        }

        public void SetControlPointMode (int index, BezierControlPointMode mode) {
            _modes[(index + 1) / 3] = mode;
            EnforceMode(index);
        }
        
        private void EnforceMode (int index) {
            var modeIndex = (index + 1) / 3;
            var mode = _modes[modeIndex];
            
            if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == _modes.Length - 1) {
                return;
            }

            var middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex) {
                fixedIndex = middleIndex - 1;
                enforcedIndex = middleIndex + 1;
            } else {
                fixedIndex = middleIndex + 1;
                enforcedIndex = middleIndex - 1;
            }

            var middle = _points[middleIndex];
            var enforcedTangent = middle - _points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned) {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, _points[enforcedIndex]);
            }
            _points[enforcedIndex] = middle + enforcedTangent;
        }

        public void Reset () {
            _points = new[] {
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(4f, 0f, 0f) 
            };

            _modes = new[] {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free
            };
        }
    }
}
