﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace CleverCrow.Curves {
	public class NodeCurve : MonoBehaviour {
		public List<CurvePoint> points;

		public bool Ready => points[0].transform != null 
		                     && points[1].transform != null;

		private void Reset () {
			points = new List<CurvePoint> {
				new CurvePoint(),
				new CurvePoint()
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
