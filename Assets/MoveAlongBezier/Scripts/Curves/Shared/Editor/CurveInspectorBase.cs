using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    public abstract class CurveInspectorBase : Editor {
        private const float HANDLE_SIZE = 0.04f;
        private const float PICK_SIZE = 0.06f;
        
        protected CurveBase _curve;
        protected int _selectedIndex;
        protected TangentPoint _selectedTangent;
        
        public void OnSceneGUI () {
            _curve = target as CurveBase;
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
        
        protected void InspectorCurrentPoint () {
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
                && _curve.CurveCount > 1
                && GUILayout.Button("Delete Point")) {
                
                EditorUtility.SetDirty(_curve);
                Undo.RecordObject(_curve, "Delete point");
                _curve.Points.RemoveAt(_selectedIndex);
            }
        }
    }
}