using NUnit.Framework;
using UnityEngine;

namespace CleverCrow.Curves.Editors {
    public class NodeCurveTest {
        private NodeCurve _curve;
        private Transform _startPoint;
        private Transform _endPoint;

        [SetUp]
        public void BeforeEach () {
            _curve = new GameObject("NodeCurve").AddComponent<NodeCurve>();
            
            _startPoint = new GameObject("StartPoint").transform;
            _startPoint.transform.position = Vector3.one;

            _endPoint = new GameObject("EndPoint").transform;
            _endPoint.transform.position = Vector3.down;
        }

        [TearDown]
        public void AfterEach () {
            Object.DestroyImmediate(_curve.gameObject);
            Object.DestroyImmediate(_startPoint.gameObject);
            Object.DestroyImmediate(_endPoint.gameObject);
        }

        public class GetStartPointMethod : NodeCurveTest {
            [Test]
            public void It_should_convert_point_to_a_CurvePoint_object () {
                var pointTransform = _curve.Vector3ToPoint(Vector3.one);
                
                Assert.AreEqual(Vector3.one, pointTransform.Position);
            }
        }

        public class StartPoint : NodeCurveTest {
            [Test]
            public void Returns_the_start_point () {
                _curve.start = _startPoint;
                
                Assert.AreEqual(_startPoint.transform.position, _curve.StartPoint);
            }
        }
        
        public class EndPoint : NodeCurveTest {
            [Test]
            public void Returns_the_start_point () {
                _curve.end = _endPoint;
                
                Assert.AreEqual(_endPoint.transform.position, _curve.EndPoint);
            }
        }
    }
}