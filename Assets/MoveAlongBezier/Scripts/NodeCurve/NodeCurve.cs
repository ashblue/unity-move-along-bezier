using UnityEngine;

namespace CleverCrow.Curves {
	public class NodeCurve : MonoBehaviour {
		public Transform start;
		public Transform end;

		public Vector3 StartPoint => start.transform.position;
		public Vector3 EndPoint => end.transform.position;
		public bool Ready => start != null && end != null;

		public CurvePoint Vector3ToPoint (Vector3 point) {
			return new CurvePoint(point);
		}
	}
}
