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

            for (var i = 0; i < _curve.Points.Count - 1; i += 1) {
                var startPoint = _curve.Points[i];
                if (i == 0) DrawPoint(i, startPoint);
                var startTangent = DrawTangentPoint(i, TangentPoint.B);
                
                var endPoint = _curve.Points[i + 1];
                DrawPoint(i + 1, endPoint);
                var endTangent = DrawTangentPoint(i + 1, TangentPoint.A);
                
                Handles.DrawBezier(startPoint.GlobalPosition, endPoint.GlobalPosition, startTangent, endTangent, Color.white, null, 2f);
            }
        }

        public override void OnInspectorGUI () {
            _curve = target as NodeCurve;
                        
            EditorGUI.BeginChangeCheck();
            var startPoint = _curve.Points[0];
            startPoint.transform = 
                EditorGUILayout.ObjectField("Start Point", startPoint.transform, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_curve, "Change start point");
                EditorUtility.SetDirty(_curve);
                startPoint.SetRelativeTangent(_curve.Points[1].Position);
            }
            
            EditorGUI.BeginChangeCheck();
            var endPoint = _curve.Points[_curve.Points.Count - 1];
            endPoint.transform = 
                EditorGUILayout.ObjectField("End Point", endPoint.transform, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_curve, "Change end point");
                EditorUtility.SetDirty(_curve);
                endPoint.SetRelativeTangent(_curve.Points[_curve.Points.Count - 2].Position);
            }

            AddNewPoint();
            InspectorCurrentPoint();
            
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            DrawDefaultInspector();
        }

        private void AddNewPoint () {
            if (_newPointPositions == null || _newPointPositions.Count != _curve.Points.Count) {
                _newPointPositions = new List<Vector2>();
                var options = new List<string>();
                for (var i = 0; i < _curve.Points.Count; i++) {
                    _newPointPositions.Add(new Vector2(i, i + 1));
                    options.Add($"{i}, {i + 1}");
                }

                options.RemoveAt(options.Count - 1);
                _newPointOptions = options.ToArray();
            }

            EditorGUILayout.LabelField("Point Creator", EditorStyles.boldLabel);
            _newPointPosition = EditorGUILayout.Popup("Point Between", _newPointPosition, _newPointOptions);
            if (GUILayout.Button("Add Point")) {
                var start = _curve.transform.InverseTransformPoint(_curve.Points[_newPointPosition].GlobalPosition);
                var end = _curve.transform.InverseTransformPoint(_curve.Points[_newPointPosition + 1].GlobalPosition);
                var point = new CurvePoint {
                    Position = Vector3.Lerp(start, end, 0.5f),
                    transform = _curve.transform
                };

                Undo.RecordObject(_curve, "Add Point");
                EditorUtility.SetDirty(_curve);
                _curve.Points.Insert(_newPointPosition + 1, point);
            }
        }

        private void InspectorCurrentPoint () {
            if (_selectedIndex >= _curve.Points.Count) return;

            var point = _curve.Points[_selectedIndex];
            EditorGUILayout.LabelField("Current Point", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var pointPosition = EditorGUILayout.Vector3Field("Point Position", point.Position);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(_curve);
                Undo.RecordObject(_curve, "Move selected point");
                point.Position = pointPosition;
            }
            
            EditorGUI.BeginChangeCheck();
            var mode = (CurveMode)EditorGUILayout.EnumPopup("Set Mode", point.Mode);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(_curve);
                Undo.RecordObject(_curve, "Changed tangent type");
                point.EnforceTangents(_selectedTangent);
                point.Mode = mode;
            }
            
            if (_selectedTangent != TangentPoint.None 
                && point.Mode != CurveMode.StraightLine) {
                EditorGUI.BeginChangeCheck();
                var newTangent = EditorGUILayout.Vector3Field("Selected Tangent", point.GetTangent(_selectedTangent));

                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(_curve);
                    Undo.RecordObject(_curve, "Move selected tangent");
                    point.SetTangent(_selectedTangent, newTangent);
                    point.EnforceTangents(_selectedTangent);
                }
            }

            if (_selectedTangent == TangentPoint.None 
                && !_curve.Points[_selectedIndex].isEndPoint
                && GUILayout.Button("Delete Point")) {
                
                EditorUtility.SetDirty(_curve);
                Undo.RecordObject(_curve, "Delete point");
                _curve.Points.RemoveAt(_selectedIndex);
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
                    Undo.RecordObject(_curve, "Move point position");
                    EditorUtility.SetDirty(_curve);
                    point.Position = point.transform.InverseTransformPoint(handle);
                }
            }
        }

        private Vector3 DrawTangentPoint (int index, TangentPoint tangentPoint) {
            var point = _curve.Points[index];
            var handle = point.GetTangent(tangentPoint) + point.GlobalPosition;
            var size = HandleUtility.GetHandleSize(handle);
            var handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                point.transform.rotation : Quaternion.identity;
            
            Handles.color = Color.gray;
            Handles.DrawLine(point.GlobalPosition, handle);
            
            Handles.color = point.Mode.GetTangentColor();
            if (point.Mode != CurveMode.StraightLine
                && Handles.Button(handle, handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                _selectedIndex = index;
                _selectedTangent = tangentPoint;
                Repaint();
            }
            
            if (_selectedIndex == index && _selectedTangent == tangentPoint) {
                EditorGUI.BeginChangeCheck();
                handle = Handles.DoPositionHandle(handle, handleRotation);

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_curve, "Move tangent point");
                    EditorUtility.SetDirty(_curve);
                    point.SetTangent(tangentPoint, handle - point.GlobalPosition);
                    point.EnforceTangents(tangentPoint);
                }
            }

            return handle;
        }
    }
}