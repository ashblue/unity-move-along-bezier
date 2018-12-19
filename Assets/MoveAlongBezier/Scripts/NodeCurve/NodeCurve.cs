using System;
using System.Collections.Generic;
using UnityEngine;

namespace CleverCrow.Curves {
	public class NodeCurve : MonoBehaviour {
		public List<CurvePoint> points;

		public int CurveCount => points.Count - 1;
		public bool Ready => points[0].transform != null 
		                     && points[points.Count - 1].transform != null;

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
				points[i].GlobalPosition, points[i].GlobalPosition + points[i].TangentB, 
				points[i + 1].GlobalPosition + points[i + 1].TangentA, points[i + 1].GlobalPosition,
				time);
		}
		
		private void Reset () {
			points = new List<CurvePoint> {
				new CurvePoint {isEndPoint = true},
				new CurvePoint {isEndPoint = true}
			};
		}

		private void OnDrawGizmos () {
#if UNITY_EDITOR
			// If this object is selected, don't draw our gizmos
			if (Array.IndexOf(UnityEditor.Selection.gameObjects, gameObject) >= 0) {
				return;
			}
#endif
			
			Gizmos.color = Color.gray;
			Gizmos.DrawLine(points[0].Position, points[points.Count - 1].Position);
		}
	}
}
