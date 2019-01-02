using System.Collections.Generic;
using UnityEngine;

namespace CleverCrow.Curves {
    public interface ICurve {
        int CurveCount { get; }
        List<CurvePoint> Points { get; }

        Vector3 GetPoint (float time);
    }
}