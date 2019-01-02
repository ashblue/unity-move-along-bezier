using System;
using UnityEngine;

namespace CleverCrow.Curves {
    public enum CurveMode {
        Free,
        StraightLine,
        Aligned,
        Mirrored
    }

    public static class CurveModeExtensions {
        public static Color GetPointColor (this CurveMode curveMode) {
            switch (curveMode) {
                case CurveMode.Free:
                    return Color.cyan;
                case CurveMode.StraightLine:
                    return Color.red;
                case CurveMode.Aligned:
                    return Color.cyan;
                case CurveMode.Mirrored:
                    return Color.cyan;
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
                case CurveMode.Aligned:
                    return Color.yellow;
                case CurveMode.Mirrored:
                    return Color.blue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(curveMode), curveMode, null);
            }
        }
    }
}