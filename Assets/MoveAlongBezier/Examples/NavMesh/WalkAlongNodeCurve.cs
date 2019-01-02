using System.Collections;
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

            StartCoroutine(FollowPath());
        }

        private void SetCurve (NodeCurve curve) {
            for (var i = 1; i <= curve.samplePoints; i++) {
                var time = i / (float) curve.samplePoints;
                _samples.Enqueue(curve.GetPoint(time));
            }
        }

        private IEnumerator FollowPath () {
            while (true) {
                if (agent.isOnOffMeshLink) {
                    yield return StartCoroutine(FollowOffMeshLink());
                    continue;
                }
                
                if (_curveIndex >= curves.Count && _samples.Count == 0) {
                    if (!(agent.remainingDistance < 0.1)) {
                        yield return null;
                        continue;
                    }
                    
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;

                    yield return null;
                    continue;
                }

                if (agent.hasPath == false || agent.remainingDistance < nextNodeDistance) {
                    if (_samples.Count == 0) {
                        SetCurve(curves[_curveIndex]);
                        _curveIndex++;
                    }
                
                    agent.SetDestination(_samples.Dequeue());
                }

                yield return null;
            }
        }

        private IEnumerator FollowOffMeshLink () {
            // Used to make sure the character is centered
            const float OFFSET_Y = 0.5f;

            // Should be adjusted at the curve level for tweaking
            const float DURATION = 2f;
            
            var curve = agent.currentOffMeshLinkData.offMeshLink.GetComponent<CurveBase>();

            // Approach jump point
            var jumpPoint = curve.GetPoint(0);
            jumpPoint.y += OFFSET_Y;
            while (Vector3.Distance(jumpPoint, transform.position) > 0.1) {
                transform.position = Vector3.MoveTowards(transform.position, jumpPoint, Time.deltaTime * 1f);
                yield return null;
            }

            // Jump
            var progress = 0f;
            while (agent.isOnOffMeshLink) {
                progress += Time.deltaTime / DURATION;
                
                var pos = curve.GetPoint(progress);
                pos.y += OFFSET_Y;
                transform.position = pos;
                
                if (progress > 1f) {
                    var landPoint = curve.GetPoint(1);
                    landPoint.y += OFFSET_Y;
                    transform.position = landPoint;
                    agent.CompleteOffMeshLink();
                }

                yield return null;
            }
        }
    }
}


