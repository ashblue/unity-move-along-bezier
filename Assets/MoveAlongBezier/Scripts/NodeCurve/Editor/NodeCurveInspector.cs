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
        private TangentPoint _selectedTangent;

        private int _newPointPosition;
        private List<Vector2> _newPointPositions;
        private string[] _newPointOptions;
        
        private void OnSceneGUI () {
            _curve = target as NodeCurve;
            if (!_curve.Ready) return;

            for (var i = 0; i < _curve.points.Count - 1; i += 1) {
                var startPoint = _curve.points[i];
                if (i == 0) DrawPoint(i, startPoint);
                var startTangent = DrawTangentPoint(i, TangentPoint.B);
                
                var endPoint = _curve.points[i + 1];
                DrawPoint(i + 1, endPoint);
                var endTangent = DrawTangentPoint(i + 1, TangentPoint.A);
                
                Handles.DrawBezier(startPoint.GlobalPosition, endPoint.GlobalPosition, startTangent, endTangent, Color.white, null, 2f);
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
            
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            DrawDefaultInspector();
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
                var start = _curve.transform.InverseTransformPoint(_curve.points[_newPointPosition].GlobalPosition);
                var end = _curve.transform.InverseTransformPoint(_curve.points[_newPointPosition + 1].GlobalPosition);
                var point = new CurvePoint {
                    Position = Vector3.Lerp(start, end, 0.5f),
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
            EditorGUILayout.Vector3Field("Point Position", _curve.points[_selectedIndex].Position);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            _curve.points[_selectedIndex].Mode =
                (CurveMode)EditorGUILayout.EnumPopup("Set Mode", _curve.points[_selectedIndex].Mode);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(_curve);
                Undo.RegisterCompleteObjectUndo(_curve, "Tangent type changed");
            }
            
            if (_selectedTangent != TangentPoint.None 
                && _curve.points[_selectedIndex].Mode != CurveMode.StraightLine) {
                EditorGUI.BeginChangeCheck();
                var newTangent = EditorGUILayout.Vector3Field("Selected Tangent",
                    _curve.points[_selectedIndex].GetTangent(_selectedTangent));
                _curve.points[_selectedIndex].SetTangent(_selectedTangent, newTangent);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(_curve);
                    Undo.RecordObject(_curve, "Change selected point");
                }
            }
        }

        private void DrawPoint (int index, CurvePoint point) {
            Handles.color = point.Mode.GetPointColor();
            var handle = point.GlobalPosition;
            var size = HandleUtility.GetHandleSize(handle);
            var handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                point.transform.rotation : Quaternion.identity;

            if (Handles.Button(handle, point.transform.rotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                _selectedIndex = index;
                _selectedTangent = TangentPoint.None;
                Repaint();
            }
            
            if (_selectedIndex == index && _selectedTangent == TangentPoint.None) {
                EditorGUI.BeginChangeCheck();
                handle = Handles.DoPositionHandle(handle, handleRotation);
                
                if (EditorGUI.EndChangeCheck()) {
                    point.Position = point.transform.InverseTransformPoint(handle);
                    Undo.RecordObject(_curve, "Move point position");
                    EditorUtility.SetDirty(_curve);
                }
            }
        }

        private Vector3 DrawTangentPoint (int index, TangentPoint tangentPoint) {
            var point = _curve.points[index];
            var handle = point.GetTangent(tangentPoint) + point.GlobalPosition;
            var size = HandleUtility.GetHandleSize(handle);
            var handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                point.transform.rotation : Quaternion.identity;
            
            Handles.color = Color.gray;
            Handles.DrawLine(point.GlobalPosition, handle);
            
            Handles.color = point.Mode.GetTangentColor();
            if (Handles.Button(handle, handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                _selectedIndex = index;
                _selectedTangent = tangentPoint;
                Repaint();
            }
            
            if (_selectedIndex == index && _selectedTangent == tangentPoint) {
                EditorGUI.BeginChangeCheck();
                handle = Handles.DoPositionHandle(handle, handleRotation);
                
                if (EditorGUI.EndChangeCheck()) {
                    point.SetTangent(tangentPoint, handle - point.GlobalPosition);
                    Undo.RecordObject(_curve, "Move tangent point");
                    EditorUtility.SetDirty(_curve);
                }
            }

            return handle;
        }
    }
}