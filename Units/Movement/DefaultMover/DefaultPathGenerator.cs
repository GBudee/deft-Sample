using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {
    class DefaultPathGenerator {

        public static List<List<HexEntry>> FindAllPaths(HexEntry current, int movesRemaining, IUnitController controller) {

            // We will fill this with paths we find
            List<List<HexEntry>> allPaths = new List<List<HexEntry>>();

            // This will be used after every run of the outer loop to process the most recently found paths
            List<List<HexEntry>> pathsFound = new List<List<HexEntry>>();
            List<List<HexEntry>> newPathsFound = new List<List<HexEntry>>();

            // Used to keep track of hexes we don't want our new paths to go back to. 
            HashSet<HexEntry> closed = new HashSet<HexEntry>();

            // Initially, we already know how to get to the origin
            List<HexEntry> originPath = new List<HexEntry>();
            originPath.Add(current);

            // Add the origin path and update allPaths
            pathsFound.Add(originPath);
            allPaths = allPaths.Concat(pathsFound).ToList();

            // We've considered all paths that go through the origin: close it.
            closed.Add(current);

            // At each step, consider paths of length i (not including the origin--that would be i + 1)
            for (var i = 1; i <= movesRemaining; i++) {

                // This is a set of the hexes that surround the perimeter of closed. 
                HashSet<HexEntry> borderHexes = FindBorderHexes(pathsFound);

                // Now we add them to closed.
                foreach (HexEntry hex in borderHexes) {
                    closed.Add(hex);
                }

                // After each call to AugmentPath, newPathsFound will have a new path in it.
                for (var j = 0; j < pathsFound.Count; j++) {
                    List<HexEntry> path = pathsFound[j];
                    AugmentPath(path, newPathsFound, closed, controller, movesRemaining);
                }

                // For the next round, make the new paths found the "old" paths found, and make a new newPathsFound
                allPaths = new List<List<HexEntry>>(allPaths.Concat(newPathsFound));

                pathsFound = newPathsFound;
                newPathsFound = new List<List<HexEntry>>();
            }

            return allPaths;
        }

        private static void AugmentPath(List<HexEntry> pathSoFar, List<List<HexEntry>> pathsFound,
                HashSet<HexEntry> closed, IUnitController controller, int movesRemaining) {

            // We want to take this partially-constructed path and extend it by one more hex.
            // From the last hex in the path, consider every neighboring hex.
            foreach (var neighbor in pathSoFar.Last().Neighbors.Values) {

                // Only further consider ones that are not in the closed set.
                if (!closed.Contains(neighbor)) {

                    // We've got a match! Create a new list to avoid modifying the old path
                    List<HexEntry> newPath = new List<HexEntry>(pathSoFar);
                    // Add the new hex
                    newPath.Add(neighbor);

                    // TODO: can we optimize this at all?
                    // Make sure it meets ZOC and terrain cost requirements
                    if (RespectsZoneOfControl(newPath, controller) && MeetsTerrainCostRequirement(newPath, controller, movesRemaining)) {
                        pathsFound.Add(newPath);
                    }
                }
            }
        }

        // To find border hexes, just find the last hex of every path we've found.
        private static HashSet<HexEntry> FindBorderHexes(List<List<HexEntry>> pathsFound) {
            HashSet<HexEntry> s = new HashSet<HexEntry>();
            foreach (List<HexEntry> path in pathsFound) {
                s.Add(path.Last());
            }
            return s;
        }

        // True if unit has enough Zone of Control moves to follow this path--false otherwise.
        private static bool RespectsZoneOfControl(List<HexEntry> path, IUnitController controller) {

            for (var i = 0; i < path.Count - 1; i++) {
                // Not correct for ZoCRemaining values not equal to 0 or 1
                if (IsEnemyNeighbor(path[i], controller.PlayerOwner)
                    && path.Count > i + 1 + controller.ZoCRemaining) {
                    return false;
                }
            }
            return true;
        }

        // True if hex has an enemy on a neighboring hex
        public static bool IsEnemyNeighbor(HexEntry hex, int owner) {
            foreach (HexEntry neighborHex in hex.Neighbors.Values) {
                if (neighborHex.Occupant != null
                    && neighborHex.Occupant.PlayerOwner != owner)
                    return true;
            }
            return false;
        }

        private static bool MeetsTerrainCostRequirement(List<HexEntry> path, IUnitController controller, int movesRemaining) {
            int sum = 0;
            UnitBaseType unitType = controller.UnitType;

            // Note: skip the origin of the path
            foreach (HexEntry hex in path.Skip(1)) {
                if (UnitBaseStats.TerrainCost(unitType, hex.Terrain) <= 0) {
                    return false;
                }
                if (hex.Occupant != null) {
                    return false;
                }
                sum += UnitBaseStats.TerrainCost(unitType, hex.Terrain);
            }
            return sum <= movesRemaining;
        }
    }
}
