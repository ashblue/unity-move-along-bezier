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

        private static Color[] _modeColors = {
            Color.white,
            Color.yellow,
            Color.cyan,
            Color.red
        };

        private void OnSceneGUI () {
            _spline = target as BezierSpline;
            _handleTransform = _spline.transform;
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                _handleTransform.rotation : Quaternion.identity;

            var p0 = ShowPoint(0);
            for (var i = 1; i < _spline.ControlPointCount; i += 3) {
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
            _spline = target as BezierSpline;

            if (_selectedIndex >= 0 && _selectedIndex < _spline.ControlPointCount) {
                DrawSelectedPointInspector();
            }
            
            if (GUILayout.Button("Add Curve")) {
                Undo.RecordObject(_spline, "Add Curve");
                _spline.AddCurve();
                EditorUtility.SetDirty(_spline);
            }
        }

        private void DrawSelectedPointInspector () {
            GUILayout.Label("Selected Point");
            
            EditorGUI.BeginChangeCheck();
            
            var point = EditorGUILayout.Vector3Field("Position", _spline.GetControlPoint(_selectedIndex));
            
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_spline, "Move Point");
                EditorUtility.SetDirty(_spline);
                _spline.SetControlPoint(_selectedIndex, point);
            }
            
            EditorGUI.BeginChangeCheck();
            
            BezierControlPointMode mode =
                (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", _spline.GetControlPointMode(_selectedIndex));

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_spline, "Change Point Mode");
                _spline.SetControlPointMode(_selectedIndex, mode);
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
            var point = _handleTransform.TransformPoint(_spline.GetControlPoint(index));
            var size = HandleUtility.GetHandleSize(point);
            var mode = _spline.GetControlPointMode(index);

            // Do not draw handles for straight lines
            if (mode == BezierControlPointMode.StraightLine && _spline.IsHandle(index)) {
                return point;
            }
            
            Handles.color = _modeColors[(int)mode];
            if (Handles.Button(point, _handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                _selectedIndex = index;
                Repaint();
            }

            if (_selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, _handleRotation);

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_spline, "Move Point");
                    EditorUtility.SetDirty(_spline);
                    _spline.SetControlPoint(index, _handleTransform.InverseTransformPoint(point));
                }
            }
            
            return point;
        }
    }
}