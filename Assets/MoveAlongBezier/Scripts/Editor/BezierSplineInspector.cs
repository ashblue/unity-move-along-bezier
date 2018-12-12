using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : Editor {
        private const int LINE_STEPS = 10;
        private const float DIRECTION_SCALE = 0.5f;
        private const int STEPS_PER_CURVE = 10;
        private const float HANDLE_SIZE = 0.04f;
        private const float PICK_SIZE = 0.06f;

        private int _selectedIndex = -1;
        private BezierSpline _spline;
        private Transform _handleTransform;
        private Quaternion _handleRotation;

        private void OnSceneGUI () {
            _spline = target as BezierSpline;
            _handleTransform = _spline.transform;
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                _handleTransform.rotation : Quaternion.identity;

            var p0 = ShowPoint(0);
            for (var i = 1; i < _spline.points.Length; i += 3) {
                var p1 = ShowPoint(i);
                var p2 = ShowPoint(i + 1);
                var p3 = ShowPoint(i + 2);
                
                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);
                
                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }

            ShowDirections();
        }

        public override void OnInspectorGUI () {
            DrawDefaultInspector();
            _spline = target as BezierSpline;
            
            if (GUILayout.Button("Add Curve")) {
                Undo.RecordObject(_spline, "Add Curve");
                _spline.AddCurve();
                EditorUtility.SetDirty(_spline);
            }
        }

        private void ShowDirections () {
            Handles.color = Color.green;
            var point = _spline.GetPoint(0f);
            Handles.DrawLine(point, point + _spline.GetDirection(0f) * DIRECTION_SCALE);
            
            var steps = STEPS_PER_CURVE * _spline.CurveCount;
            for (var i = 1; i <= steps; i++) {
                point = _spline.GetPoint(i / (float)steps);
                Handles.DrawLine(point, point + _spline.GetDirection(i / (float)steps) * DIRECTION_SCALE);
            }
        }

        private Vector3 ShowPoint (int index) {
            var point = _handleTransform.TransformPoint(_spline.points[index]);
            var size = HandleUtility.GetHandleSize(point);
            
            Handles.color = Color.white;
            if (Handles.Button(point, _handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                _selectedIndex = index;
            }

            if (_selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, _handleRotation);

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_spline, "Move Point");
                    EditorUtility.SetDirty(_spline);
                    _spline.points[index] = _handleTransform.InverseTransformPoint(point);
                }
            }
            
            return point;
        }
    }
}