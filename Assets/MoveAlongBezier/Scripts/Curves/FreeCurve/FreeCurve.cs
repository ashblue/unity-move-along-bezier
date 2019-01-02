using System.Collections.Generic;
using UnityEngine;

namespace CleverCrow.Curves {
    public class FreeCurve : CurveBase {
        private void Reset () {
            Points = new List<CurvePoint> {
                new CurvePoint {
                    transform = transform,
                    Position =  Vector3.left
                },
                new CurvePoint {
                    transform = transform,
                    Position = Vector3.right
                }
            };
        }
    }
}