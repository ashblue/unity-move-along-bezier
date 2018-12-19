using System;
using UnityEngine;

namespace CleverCrow.Curves {
    public class NodeCurveWalker : MonoBehaviour {
        public NodeCurve curve;
        public float duration = 5;
        public bool reverse;

        private float _progress;

        private void Update () {
            _progress += Time.deltaTime / duration;
            
            if (_progress > 1f) {
                _progress = 0f;
            }

            var progress = _progress;
            if (reverse) progress = Mathf.Abs(_progress - 1f);
            
            var pos = curve.GetPoint(progress);
            transform.position = pos;
        }
    }
}