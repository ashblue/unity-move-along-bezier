using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(NodeCurve))]
    public class NodeCurveInspector : Editor {
        private void OnSceneGUI () {
            var curve = target as NodeCurve;

            if (!curve.Ready) return;

            Handles.color = Color.green;
            Handles.SphereHandleCap(0, curve.StartPoint, curve.transform.rotation, 0.4f, EventType.Repaint);
            Handles.SphereHandleCap(0, curve.EndPoint, curve.transform.rotation, 0.4f, EventType.Repaint);

            Handles.color = Color.cyan;
            Handles.DrawLine(curve.StartPoint, curve.EndPoint);
        }
    }
}