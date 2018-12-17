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

        public class StartPoint : NodeCurveTest {
            [Test]
            public void It_should_create_a_default_start_point () {
                Assert.IsNotNull(_curve.points[0]);
            }
        }
        
        public class EndPoint : NodeCurveTest {
            [Test]
            public void It_should_create_a_default_end_point () {
                Assert.IsNotNull(_curve.points[1]);
            }
        }
    }
}