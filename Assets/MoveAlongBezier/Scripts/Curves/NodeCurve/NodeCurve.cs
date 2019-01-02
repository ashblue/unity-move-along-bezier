using System.Collections.Generic;
using UnityEngine;

namespace CleverCrow.Curves {
	public class NodeCurve : CurveBase {
		[Tooltip("Number of nav mesh points generated to follow. Increase the number for more accurate traversal")]
		public int samplePoints = 10;

		public bool Ready => Points[0].transform != null 
		                     && Points[Points.Count - 1].transform != null;
		
		private void Reset () {
			Points = new List<CurvePoint> {
				new CurvePoint {isEndPoint = true},
				new CurvePoint {isEndPoint = true}
			};
		}
	}
}
