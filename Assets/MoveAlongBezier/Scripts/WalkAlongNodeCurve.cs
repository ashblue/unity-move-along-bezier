using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CleverCrow.Curves {
    public class WalkAlongNodeCurve : MonoBehaviour {
        public NavMeshAgent agent;
        public NodeCurve curve;
        public int sampleCount = 10;
        
        [Range(0, 1)]
        public float nextNodeDistance = 0.5f;

        private Queue<Vector3> _samples = new Queue<Vector3>();

        void Start () {
            transform.position = curve.points[0].GlobalPosition;
            
            for (var i = 1; i <= sampleCount; i++) {
                var time = i / (float)sampleCount;
                _samples.Enqueue(curve.GetPoint(time));
            }
        }

        void Update () {
            if (_samples.Count == 0) {
                if (agent.remainingDistance < 0.1) agent.isStopped = true;
                return;
            }

            if (agent.hasPath == false || agent.remainingDistance < nextNodeDistance) {
                agent.SetDestination(_samples.Dequeue());
            }
        }
    }
}


