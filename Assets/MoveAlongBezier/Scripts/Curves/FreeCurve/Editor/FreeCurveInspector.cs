using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(FreeCurve))]
    public class FreeCurveInspector : CurveInspectorBase {
        public override void OnInspectorGUI () {
            _curve = target as CurveBase;

            AddPoint();
            
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            DrawDefaultInspector();
        }

        private void AddPoint () {
            if (!GUILayout.Button("Add Point")) return;
            
            var endPos = _curve.transform.InverseTransformPoint(_curve.Points[_curve.Points.Count - 1].GlobalPosition);
            var point = new CurvePoint {
                Position = endPos + (Vector3.right * 4),
                transform = _curve.transform
            };
            
            Undo.RecordObject(_curve, "Add Point");
            EditorUtility.SetDirty(_curve);
            _curve.Points.Add(point);
        }
    }
}