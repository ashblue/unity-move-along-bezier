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
	}
}
