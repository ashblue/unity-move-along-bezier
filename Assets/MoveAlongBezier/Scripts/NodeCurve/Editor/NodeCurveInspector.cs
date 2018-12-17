using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(NodeCurve))]
    public class NodeCurveInspector : Editor {
        private const float HANDLE_SIZE = 0.04f;
        private const float PICK_SIZE = 0.06f;

        private NodeCurve _curve;
        private int _selectedIndex;

        private void OnSceneGUI () {
            _curve = target as NodeCurve;
            if (!_curve.Ready) return;

            for (var i = 0; i < _curve.points.Count; i += 2) {
                var startPoint = _curve.points[i];
                DrawEndPoint(startPoint.Position);
                var startTangent = DrawTangentPoint(startPoint.transform, i);
                
                var endPoint = _curve.points[i + 1];
                DrawEndPoint(endPoint.Position);
                var endTangent = DrawTangentPoint(endPoint.transform, i + 1);
                
                Handles.DrawBezier(startPoint.Position, endPoint.Position, startTangent, endTangent, Color.white, null, 2f);
            }
        }

        public override void OnInspectorGUI () {
            _curve = target as NodeCurve;
            
            EditorGUI.BeginChangeCheck();
            _curve.points[0].transform = 
                EditorGUILayout.ObjectField("Start Point", _curve.points[0].transform, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_curve, "Change start point");
                EditorUtility.SetDirty(_curve);
            }
            
            EditorGUI.BeginChangeCheck();
            var lastIndex = _curve.points.Count - 1;
            _curve.points[lastIndex].transform = 
                EditorGUILayout.ObjectField("Start Point", _curve.points[lastIndex].transform, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_curve, "Change end point");
                EditorUtility.SetDirty(_curve);
            }

            if (_selectedIndex < _curve.points.Count) {
                EditorGUILayout.LabelField("Current Point", EditorStyles.boldLabel);

                GUI.enabled = false;
                EditorGUILayout.Vector3Field("Current Position", _curve.points[_selectedIndex].Position);
                GUI.enabled = true;
                
                EditorGUI.BeginChangeCheck();
                _curve.points[_selectedIndex].tangent = 
                    EditorGUILayout.Vector3Field("Current Tangent", _curve.points[_selectedIndex].tangent);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_curve, "Change selected point");
                    EditorUtility.SetDirty(_curve);
                }
            }
        }

        private void DrawEndPoint (Vector3 startPoint) {
            Handles.color = Color.white;
            var startHandleSize = HandleUtility.GetHandleSize(startPoint) * HANDLE_SIZE;
            Handles.SphereHandleCap(0, startPoint, _curve.transform.rotation, startHandleSize, EventType.Repaint);
        }

        private Vector3 DrawTangentPoint (Transform origin, int index) {
            var handle = origin.TransformPoint(_curve.points[index].tangent);
            var size = HandleUtility.GetHandleSize(handle);
            var handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                origin.rotation : Quaternion.identity;
            
            Handles.color = Color.gray;
            Handles.DrawLine(origin.position, handle);
            
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
                    _curve.points[index].tangent = origin.InverseTransformPoint(handle);
                }
            }

            return handle;
        }
    }
}