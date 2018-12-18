using System;
using UnityEngine;

namespace CleverCrow.Curves {
    public enum CurveMode {
        Free,
        StraightLine
    }

    public static class CurveModeExtensions {
        public static Color GetPointColor (this CurveMode curveMode) {
            switch (curveMode) {
                case CurveMode.Free:
                    return Color.cyan;
                case CurveMode.StraightLine:
                    return Color.red;
                default:
                    throw new ArgumentOutOfRangeException(nameof(curveMode), curveMode, null);
            }
        }
        
        public static Color GetTangentColor (this CurveMode curveMode) {
            switch (curveMode) {
                case CurveMode.Free:
                    return Color.green;
                case CurveMode.StraightLine:
                    return Color.red;
                default:
                    throw new ArgumentOutOfRangeException(nameof(curveMode), curveMode, null);
            }
        }
    }
}