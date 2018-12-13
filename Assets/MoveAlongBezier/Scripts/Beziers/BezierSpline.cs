﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CleverCrow.Curves {
    public class BezierSpline : MonoBehaviour {
        [SerializeField] 
        private BezierControlPointMode[] _modes;
        
        [SerializeField]
        private Vector3[] _points;

        public int ControlPointCount => _points.Length;

        public int CurveCount => (_points.Length - 1) / 3;

        public Vector3 GetControlPoint (int index) {
            return _points[index];
        }

        public void SetControlPoint (int index, Vector3 point) {
            if (index % 3 == 0) {
                var delta = point - _points[index];
                if (index > 0) {
                    _points[index - 1] += delta;
                }

                if (index + 1 < _points.Length) {
                    _points[index + 1] += delta;
                }
            }
            
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
            EnforceMode(_points.Length - 4);
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
            
            // Enforce straight line mode on ends
            if (modeIndex == 0 && mode == BezierControlPointMode.StraightLine) {
                _points[1] = _points[0];
            } else if (modeIndex == _modes.Length - 1 && mode == BezierControlPointMode.StraightLine) {
                _points[_points.Length - 2] = _points[_points.Length - 1];
            }
            
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
            } else if (mode == BezierControlPointMode.StraightLine) {
                _points[fixedIndex] = middle;
                _points[enforcedIndex] = middle;
                return;
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

        private List<int> GetAssociatedPointIndices (int pointIndex) {
            if (pointIndex == 0 || pointIndex == 1) {
                return new List<int> {0, 1};
            }

            if (pointIndex == _points.Length - 1 || pointIndex == _points.Length - 2) {
                return new List<int> {_points.Length - 1, _points.Length - 2};
            }

            var modeIndex = (pointIndex + 1) / 3;
            var middleIndex = modeIndex * 3;

            return new List<int> {middleIndex - 1, middleIndex, middleIndex + 1};
        }

        public bool IsHandle (int index) {
            var modeIndex = (index + 1) / 3;
            var middleIndex = modeIndex * 3;

            if (index == 1 || index == _points.Length - 2) {
                return true;
            }

            return index != middleIndex;
        }

        public void DeletePoint (int pointIndex) {
            var pointIndices = GetAssociatedPointIndices(pointIndex);
            var modeIndex = GetAssociatedModeIndex(pointIndex);

            // Account for deleting end points
            if (pointIndices.Contains(0)) {
                pointIndices.Add(2);
            } else if (pointIndices.Contains(_points.Length - 1)) {
                pointIndices.Add(_points.Length - 3);
            }
            
            _points = _points.Where((source, index) => !pointIndices.Contains(index)).ToArray();
            _modes = _modes.Where((source, index) => modeIndex != index).ToArray();
        }

        private int GetAssociatedModeIndex (int pointIndex) {
            return (pointIndex + 1) / 3;
        }
    }
}
