using System;
using UnityEngine;

namespace Deft {
    public enum Bearing { E, NNE, NNW, W, SSW, SSE }

    static class HexVectorUtil {
        // Gives the displacement vector that gets a Hex to its neighbor in a given bearing
        public static Vector2 NeighborOffsetFromBearing(Bearing b) {
            switch (b) {
                case Bearing.E:
                    return new Vector2(1, 0);
                case Bearing.NNE:
                    return new Vector2(1, -1);
                case Bearing.NNW:
                    return new Vector2(0, -1);
                case Bearing.W:
                    return new Vector2(-1, 0);
                case Bearing.SSW:
                    return new Vector2(-1, 1);
                case Bearing.SSE:
                    return new Vector2(0, 1);
            }
            // never reached, but you didn't know that, did you C# compiler?
            return new Vector2(0, 0);
        }

        //Returns null if not neighbors
        public static Bearing? NeighborsToBearing(HexEntry start, HexEntry end) {
            foreach (Bearing b in Enum.GetValues(typeof(Bearing))) {
                if (start.Neighbors.ContainsKey(b)) {
                    if (start.Neighbors[b] == end) {
                        return b;
                    }
                }
            }
            return null;
        }

        public static Bearing BearingClockwise(Bearing b) {
            switch (b) {
                case Bearing.E:
                    return Bearing.SSE;
                case Bearing.NNE:
                    return Bearing.E;
                case Bearing.NNW:
                    return Bearing.NNE;
                case Bearing.W:
                    return Bearing.NNW;
                case Bearing.SSW:
                    return Bearing.W;
                case Bearing.SSE:
                default:
                    return Bearing.SSW;
            }
        }

        public static Bearing BearingCounterClockwise(Bearing b) {
            switch (b) {
                case Bearing.E:
                    return Bearing.NNE;
                case Bearing.NNE:
                    return Bearing.NNW;
                case Bearing.NNW:
                    return Bearing.W;
                case Bearing.W:
                    return Bearing.SSW;
                case Bearing.SSW:
                    return Bearing.SSE;
                case Bearing.SSE:
                default:
                    return Bearing.E;
            }
        }

        public static Vector3 AxialToCubic(Vector2 v) {
            return new Vector3(v.x, -v.x - v.y, v.y);
        }

        public static Vector2 CubicToAxial(Vector3 v) {
            return new Vector2(v.x, v.z);
        }

        // Assume a and b are in (q, r) (axial) coordinates
        public static float AxialDistance(Vector2 a, Vector2 b) {
            Vector3 ac = AxialToCubic(a);
            Vector3 bc = AxialToCubic(b);
            return CubicDistance(ac, bc);
        }

        public static float CubicDistance(Vector3 a, Vector3 b) {
            return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2;
        }

        public static Vector2 RotateClockwise(Vector2 v) {
            Vector3 vC = AxialToCubic(v);
            return CubicToAxial(new Vector3(-vC.z, -vC.x, -vC.y));
        }

        public static Vector2 RotateCounterclockwise(Vector2 v) {
            Vector3 vC = AxialToCubic(v);
            return CubicToAxial(new Vector3(-vC.y, -vC.z, -vC.x));
        }

        public static Vector2 worldPositionOfHexCoord(Vector2 hexCoord) {
            return new Vector2(
                (Config.hexSize * Mathf.Sqrt(3) * (hexCoord.x + hexCoord.y / 2)) + Config.worldCoordOffsetOfGridZero.x,
                (-Config.hexSize * 3.0f / 2.0f * hexCoord.y) + Config.worldCoordOffsetOfGridZero.y);
        }

    }
}
