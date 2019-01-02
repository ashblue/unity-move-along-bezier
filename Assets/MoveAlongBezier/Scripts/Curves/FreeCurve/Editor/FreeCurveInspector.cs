using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(FreeCurve))]
    public class FreeCurveInspector : Editor {
        private const float HANDLE_SIZE = 0.04f;
        private const float PICK_SIZE = 0.06f;

        private FreeCurve _curve;
        private int _selectedIndex;
        private TangentPoint _selectedTangent;

        private void OnSceneGUI () {
            _curve = target as FreeCurve;
            
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
        
        // @NOTE Identical to NodeCurveInspector.cs file
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
        
        // @NOTE Identical to NodeCurveInspector.cs file
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