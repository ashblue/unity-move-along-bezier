using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(NodeCurve))]
    public class NodeCurveInspector : Editor {
        private const float HANDLE_SIZE = 0.04f;
        private const float PICK_SIZE = 0.06f;

        private NodeCurve _curve;
        private int _selectedIndex;

        private int _newPointPosition;
        private List<Vector2> _newPointPositions;
        private string[] _newPointOptions;

        private void OnSceneGUI () {
            _curve = target as NodeCurve;
            if (!_curve.Ready) return;

            for (var i = 0; i < _curve.points.Count - 1; i += 1) {
                var startPoint = _curve.points[i];
                DrawPoint(startPoint.Position);
                var startTangent = DrawTangentPoint(i);
                
                var endPoint = _curve.points[i + 1];
                DrawPoint(endPoint.Position);
                var endTangent = DrawTangentPoint(i + 1);
                
                Handles.DrawBezier(startPoint.Position, endPoint.Position, startTangent, endTangent, Color.white, null, 2f);
            }
        }

        public override void OnInspectorGUI () {
            _curve = target as NodeCurve;
            
            EditorGUI.BeginChangeCheck();
            var startPoint = _curve.points[0];
            startPoint.transform = 
                EditorGUILayout.ObjectField("Start Point", startPoint.transform, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck()) {
                startPoint.SetRelativeTangent(_curve.points[1].Position);
                Undo.RecordObject(_curve, "Change start point");
                EditorUtility.SetDirty(_curve);
            }
            
            EditorGUI.BeginChangeCheck();
            var endPoint = _curve.points[_curve.points.Count - 1];
            endPoint.transform = 
                EditorGUILayout.ObjectField("End Point", endPoint.transform, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck()) {
                endPoint.SetRelativeTangent(_curve.points[_curve.points.Count - 2].Position);
                Undo.RecordObject(_curve, "Change end point");
                EditorUtility.SetDirty(_curve);
            }

            AddNewPoint();
            InspectorCurrentPoint();
        }

        private void AddNewPoint () {
            if (_newPointPositions == null || _newPointPositions.Count != _curve.points.Count) {
                _newPointPositions = new List<Vector2>();
                var options = new List<string>();
                for (var i = 0; i < _curve.points.Count; i++) {
                    _newPointPositions.Add(new Vector2(i, i + 1));
                    options.Add($"{i}, {i + 1}");
                }

                options.RemoveAt(options.Count - 1);
                _newPointOptions = options.ToArray();
            }

            EditorGUILayout.LabelField("Point Creator", EditorStyles.boldLabel);
            _newPointPosition = EditorGUILayout.Popup("Point Between", _newPointPosition, _newPointOptions);
            if (GUILayout.Button("Add Point")) {
                var start = _curve.points[_newPointPosition];
                var end = _curve.points[_newPointPosition + 1];
                var point = new CurvePoint {
                    Position = Vector3.Lerp(start.Position, end.Position, 0.5f),
                    transform = _curve.transform
                };

                _curve.points.Insert(_newPointPosition + 1, point);
                Undo.RecordObject(_curve, "Add Point");
                EditorUtility.SetDirty(_curve);
            }
        }

        private void InspectorCurrentPoint () {
            if (_selectedIndex >= _curve.points.Count) return;
            
            EditorGUILayout.LabelField("Current Point", EditorStyles.boldLabel);

            GUI.enabled = false;
            EditorGUILayout.Vector3Field("Current Position", _curve.points[_selectedIndex].Position);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            _curve.points[_selectedIndex].Mode =
                (CurveMode)EditorGUILayout.EnumPopup("Set Mode", _curve.points[_selectedIndex].Mode);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(_curve);
                Undo.RegisterCompleteObjectUndo(_curve, "Tangent type changed");
            }
            
            if (_curve.points[_selectedIndex].Mode != CurveMode.StraightLine) {
                EditorGUI.BeginChangeCheck();
                _curve.points[_selectedIndex].TangentA =
                    EditorGUILayout.Vector3Field("Current Tangent", _curve.points[_selectedIndex].TangentA);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(_curve);
                    Undo.RecordObject(_curve, "Change selected point");
                }
            }
        }

        private void DrawPoint (Vector3 startPoint) {
            Handles.color = Color.white;
            var startHandleSize = HandleUtility.GetHandleSize(startPoint) * HANDLE_SIZE;
            Handles.SphereHandleCap(0, startPoint, _curve.transform.rotation, startHandleSize, EventType.Repaint);
        }

        private Vector3 DrawTangentPoint (int index) {
            var point = _curve.points[index];
            var handle = point.transform.TransformPoint(_curve.points[index].TangentA);
            var size = HandleUtility.GetHandleSize(handle);
            var handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                point.transform.rotation : Quaternion.identity;
            
            Handles.color = Color.gray;
            Handles.DrawLine(point.Position, handle);
            
            Handles.color = Color.red;
            if (Handles.Button(handle, handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                _selectedIndex = index;
                Repaint();
            }
            
            if (_selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
                handle = Handles.DoPositionHandle(handle, handleRotation);
                
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_curve, "Move tangent point");
                    EditorUtility.SetDirty(_curve);
                    _curve.points[index].TangentA = point.transform.InverseTransformPoint(handle);
                }
            }

            return handle;
        }
    }
}