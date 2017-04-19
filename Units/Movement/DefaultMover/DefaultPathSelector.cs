using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {
    class DefaultPathSelector {
        public static List<Outcome> ChooseBestPath(List<List<HexEntry>> allPaths, HexEntry dest, IUnitController unitController,
                Func<List<HexEntry>, List<Outcome>> SimulatePath) {

            // Make sure that there actually is at least one path.
            var paths = allPaths.Where(x => x.Last() == dest);
            if (!paths.Any()) {
                return new List<Outcome>();
            }

            // Get the outcomes, then preferentially order them:
            // 1. descending # of enemies attacked
            // 2. ascending # of enemies retaliating
            // 3. ascending ZOC cost
            var bestOutcomes = paths
                .Select(x => SimulatePath(x))
                .OrderByDescending(x => (x.Last().position == dest) ? 1 : 0) // Does the unit reach the destination without dying or otherwise being prevented
                .ThenBy(x =>
                    x.Aggregate(0, (acc, y) => acc - y.combat
                     .Where(z => (z.attackType == AttackType.Melee || z.attackType == AttackType.Push) && z.target.PlayerOwner != z.source.PlayerOwner).ToList().Count // Maximize the number of times the unit attacks enemies
                     + y.combat.Where(z => (z.attackType == AttackType.Melee || z.attackType == AttackType.Push) && z.target.PlayerOwner == z.source.PlayerOwner).ToList().Count)) // Maximize the number of times the unit attacks allies
                .ThenBy(x =>
                    x.Aggregate(0, (acc, y) => acc + y.combat
                     .Where(z => z.attackType == AttackType.Retal).ToList().Count)) // Minimize the number of times the unit receives retal
                .ThenBy(x => zoneOfControlExpense(x)) // Try not to consume the unit's movement by getting ZoCed
                .ThenBy(x => movePointExpense(x, unitController)); // Try to minimize mp consumption from terrain
                    
            // The best one is now at the top
            return bestOutcomes.First();
        }

        // Calculates how many ZOC moves are consumed for a given path
        private static int zoneOfControlExpense(List<Outcome> outcomes) {
            int sum = 0;
            for (var i = 1; i < outcomes.Count; i++) {
                if (IsEnemyNeighbor(outcomes[i].position, outcomes[i].activeUnit.PlayerOwner)) { 
                    sum += 1;
                }
            }
            return sum;
        }

        private static int movePointExpense(List<Outcome> outcomes, IUnitController unitController) {
            int sum = 0;
            foreach (Outcome outcome in outcomes) {
                sum += UnitBaseStats.TerrainCost(unitController.UnitType, outcome.position.Terrain);
            }
            return sum;
        }

        //ZoC utility function
        public static bool IsEnemyNeighbor(HexEntry hex, int owner) {
            foreach (HexEntry neighborHex in hex.Neighbors.Values) {
                if (neighborHex.Occupant != null
                    && neighborHex.Occupant.PlayerOwner != owner)
                    return true;
            }
            return false;
        }
    }
}
