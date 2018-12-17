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

            DrawEndPoint(_curve.StartPoint);
            var startTangent = DrawTangentPoint(_curve.start, 0);

            DrawEndPoint(_curve.EndPoint);
            var endTangent = DrawTangentPoint(_curve.end, 1);
            
            Handles.DrawBezier(_curve.StartPoint, _curve.EndPoint, startTangent, endTangent, Color.white, null, 2f);
        }

        private void DrawEndPoint (Vector3 startPoint) {
            Handles.color = Color.white;
            var startHandleSize = HandleUtility.GetHandleSize(startPoint) * HANDLE_SIZE;
            Handles.SphereHandleCap(0, startPoint, _curve.transform.rotation, startHandleSize, EventType.Repaint);
        }

        private Vector3 DrawTangentPoint (Transform handle, int index) {
            var point = handle.TransformPoint(_curve.tangents[index]);
            var size = HandleUtility.GetHandleSize(point);
            var handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                handle.rotation : Quaternion.identity;
            
            Handles.color = Color.gray;
            Handles.DrawLine(handle.position, point);
            
            Handles.color = Color.red;
            if (Handles.Button(point, handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap)) {
                _selectedIndex = index;
                Repaint();
            }
            
            if (_selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_curve, "Move tangent point");
                    EditorUtility.SetDirty(_curve);
                    _curve.tangents[index] = handle.InverseTransformPoint(point);
                }
            }

            return point;
        }
    }
}