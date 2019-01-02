using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CleverCrow.Curves {
    public class CurveBase : MonoBehaviour, ICurve {
        [FormerlySerializedAs("points")]
        [SerializeField]
        private List<CurvePoint> _points;
        
        public int CurveCount => _points.Count - 1;
        public List<CurvePoint> Points {
            get => _points;
            protected set => _points = value;
        }
        
        /// <summary>
        /// Retrieve progress as a 0 - 1 value
        /// </summary>
        /// <param name="time">0 - 1 value</param>
        /// <returns></returns>
        public Vector3 GetPoint (float time) {
            int i;
            if (time >= 1f) {
                time = 1;
                i = CurveCount - 1;
            } else {
                time = Mathf.Clamp01(time) * CurveCount;
                i = (int)time;
                time -= i;
            }

            return Bezier.GetPointCubic(
                _points[i].GlobalPosition, _points[i].GlobalPosition + _points[i].TangentB, 
                _points[i + 1].GlobalPosition + _points[i + 1].TangentA, _points[i + 1].GlobalPosition,
                time);
        }
        
        private void OnDrawGizmos () {
#if UNITY_EDITOR
            // If this object is selected, don't draw our gizmos
            if (Array.IndexOf(UnityEditor.Selection.gameObjects, gameObject) >= 0) {
                return;
            }
#endif
			
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(Points[0].Position, Points[Points.Count - 1].Position);
        }
    }
}