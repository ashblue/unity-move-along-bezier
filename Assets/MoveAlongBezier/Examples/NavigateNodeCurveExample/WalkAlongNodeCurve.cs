using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace CleverCrow.Curves {
    public class WalkAlongNodeCurve : MonoBehaviour {
        private int _curveIndex;
        private Queue<Vector3> _samples = new Queue<Vector3>();

        public NavMeshAgent agent;
        public List<NodeCurve> curves;
        
        [Range(0, 1)]
        public float nextNodeDistance = 0.5f;


        void Start () {
            transform.position = curves[0].Points[0].GlobalPosition;
            SetCurve(curves[_curveIndex]);
            _curveIndex++;
        }

        private void SetCurve (NodeCurve curve) {
            for (var i = 1; i <= curve.samplePoints; i++) {
                var time = i / (float) curve.samplePoints;
                _samples.Enqueue(curve.GetPoint(time));
            }
        }

        void Update () {
            if (_curveIndex >= curves.Count && _samples.Count == 0) {
                if (!(agent.remainingDistance < 0.1)) return;
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                return;
            }

            if (agent.hasPath == false || agent.remainingDistance < nextNodeDistance) {
                if (_samples.Count == 0) {
                    SetCurve(curves[_curveIndex]);
                    _curveIndex++;
                }
                
                agent.SetDestination(_samples.Dequeue());
            }
        }
    }
}


