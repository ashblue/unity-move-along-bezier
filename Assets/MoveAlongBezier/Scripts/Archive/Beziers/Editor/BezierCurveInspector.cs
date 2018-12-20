using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveInspector : Editor {
        private const int LINE_STEPS = 10;
        
        private BezierCurve _curve;
        private Transform _handleTransform;
        private Quaternion _handleRotation;

        private void OnSceneGUI () {
            _curve = target as BezierCurve;
            _handleTransform = _curve.transform;
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                _handleTransform.rotation : Quaternion.identity;
            
            var p0 = ShowPoint(0);
            var p1 = ShowPoint(1);
            var p2 = ShowPoint(2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            
            // Print the starting point
            var lineStart = _curve.GetPoint(0f);
            Handles.color = Color.green;
            Handles.DrawLine(lineStart, lineStart + _curve.GetDirection(0f));
            
            // Create the curve
            for (var i = 1; i <= LINE_STEPS; i++) {
                var lineEnd = _curve.GetPoint(i / (float) LINE_STEPS);
                
                // Print the direct line
                Handles.color = Color.white;
                Handles.DrawLine(lineStart, lineEnd);
                
                // Print the line curve projection
                Handles.color = Color.green;
                Handles.DrawLine(lineEnd, lineEnd + _curve.GetDirection(i / (float)LINE_STEPS));
                
                lineStart = lineEnd;
            }
        }
        
        private Vector3 ShowPoint (int index) {
            var point = _handleTransform.TransformPoint(_curve.points[index]);
            
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, _handleRotation);
            
            if (!EditorGUI.EndChangeCheck()) return point;
            
            Undo.RecordObject(_curve, "Move Point");
            EditorUtility.SetDirty(_curve);
            _curve.points[index] = _handleTransform.InverseTransformPoint(point);
            
            return point;
        }
    }
}