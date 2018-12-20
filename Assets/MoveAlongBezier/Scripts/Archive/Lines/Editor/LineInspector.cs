using UnityEditor;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    [CustomEditor(typeof(Line))]
    public class LineInspector : Editor {
        private void OnSceneGUI () {
            var line = target as Line;
            var handleTransform = line.transform;
            var handleRotation = Tools.pivotRotation == PivotRotation.Local ? 
                handleTransform.rotation : Quaternion.identity;
            
            var p0 = handleTransform.TransformPoint(line.p0);
            var p1 = handleTransform.TransformPoint(line.p1);

            Handles.color = Color.white;
            Handles.DrawLine(p0, p1);
            
            SetPointZero(p0, handleRotation, line, handleTransform);
            SetPointOne(p1, handleRotation, line, handleTransform);
        }

        private static void SetPointOne (Vector3 p1, Quaternion handleRotation, Line line, Transform handleTransform) {
            EditorGUI.BeginChangeCheck();
            p1 = Handles.DoPositionHandle(p1, handleRotation);
            
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(line, "Move Point");
                EditorUtility.SetDirty(line);
                line.p1 = handleTransform.InverseTransformPoint(p1);
            }
        }

        private static void SetPointZero (Vector3 p0, Quaternion handleRotation, Line line, Transform handleTransform) {
            EditorGUI.BeginChangeCheck();
            p0 = Handles.DoPositionHandle(p0, handleRotation);
            
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(line, "Move Point");
                EditorUtility.SetDirty(line);
                line.p0 = handleTransform.InverseTransformPoint(p0);
            }
        }
    }
}