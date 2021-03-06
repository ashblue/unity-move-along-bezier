using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(NodeCurve))]
    public class NodeCurveInspector : CurveInspectorBase {
        private int _newPointPosition;
        private List<Vector2> _newPointPositions;
        private string[] _newPointOptions;

        public override void OnInspectorGUI () {
            _curve = target as CurveBase;
                        
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
    }
}