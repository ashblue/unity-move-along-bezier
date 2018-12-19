using UnityEngine;

namespace CleverCrow.Curves {
    public class SplineWalker : MonoBehaviour {
        public BezierSpline spline;
        public float duration = 5;
        public bool lookForward;

        private float _progress;

        private void Update () {
            _progress += Time.deltaTime / duration;
            
            if (_progress > 1f) {
                _progress = 0f;
            }

            var pos = spline.GetPoint(_progress);
            transform.localPosition = pos;
            if (lookForward) {
                transform.LookAt(pos + spline.GetDirection(_progress));
            }
        }
    }
}