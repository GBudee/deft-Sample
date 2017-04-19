using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Deft {
    public class DefaultAI : IComputerPlayer {

        SelectionManager selectionManager;
        ScenarioLoader scenarioLoader;

        public DefaultAI(SelectionManager selectionManager, ScenarioLoader scenarioLoader) {
            this.selectionManager = selectionManager;
            this.scenarioLoader = scenarioLoader;

            MapDistancesToGoal();
        }

        private void MapDistancesToGoal() {

            HexEntry goalHex = null;

            foreach (HexEntry hex in scenarioLoader.HexGrid.Values) {
                if (hex.Terrain == Terrain.Goal) {
                    goalHex = hex;
                }

                hex.aiDistanceToGoal = int.MaxValue;
            }

            if (goalHex == null) {
                Debug.Log("No goal");
                return;
            }

            foreach (HexEntry hex in scenarioLoader.HexGrid.Values) {
                if (hex != goalHex) {
                    hex.aiDistanceToGoal = (int)HexVectorUtil.AxialDistance(goalHex.BoardPos, hex.BoardPos);
                }
                else {
                    hex.aiDistanceToGoal = -10; // Used because the distance is a direct weighting factor
                }
            }
        }

        public List<Outcome> GenerateTurn(int player) {

            List<Outcome> result = new List<Outcome>();

            PopulateHexDistancesToEnemies(player);

            List<IUnitController> myUnits = selectionManager.UnitsByPlayer[player];
            myUnits = myUnits.OrderBy(x => WeaponUsageOrder(x.WeaponType)).ToList();


            bool moved = true;
            while (moved) {
                moved = false;

                foreach (IUnitController unit in myUnits) {
                    if (unit.HP <= 0) { // Units sometimes lose health due to an allied flail mid-movement
                        continue;
                    }

                    List<Outcome> bestHexOutcomes = new List<Outcome>();
                    int bestHexWeight = int.MaxValue;
                    foreach (HexEntry hex in unit.Mover.CalculatePaths()) {

                        List<Outcome> hexOutcomes = unit.Mover.MoveOutcomes(hex);

                        int hexWeight = HexSpacingWeight(unit.UnitType, unit.WeaponType, hex);
                        if (player == 2) {
                            hexWeight += hex.aiDistanceToGoal;
                        }
                        hexWeight += OutcomeDamageWeight(player, unit.Mover.MoveOutcomes(hex));

                        if (hexWeight < bestHexWeight || (hexWeight == bestHexWeight && hexOutcomes.Count < bestHexOutcomes.Count)) { // Can I change this to reflect mp used instead?
                            bestHexOutcomes = hexOutcomes;
                            bestHexWeight = hexWeight;
                        }
                    }

                    selectionManager.AIExecuteSubMoves(bestHexOutcomes, unit);

                    result = result.Concat(bestHexOutcomes).ToList();

                    if (bestHexOutcomes.Count > 0) {
                        moved = true;
                    }
                }
            }

            return result;
        }

        // Lower is earlier
        private int WeaponUsageOrder(WeaponType weaponType) {
            if (weaponType == WeaponType.Dagger) {
                return 1;
            }
            if (weaponType == WeaponType.Longbow) {
                return 2;
            }
            if (weaponType == WeaponType.Flail) {
                return 3;
            }
            if (weaponType == WeaponType.Sword) {
                return 4;
            }
            if (weaponType == WeaponType.Spear) {
                return 5;
            }
            if (weaponType == WeaponType.Shield) {
                return 6;
            }
            return 7;
        }

        // Lower is better
        private int HexSpacingWeight(UnitBaseType unitType, WeaponType weaponType, HexEntry hex) {

            int result = 0;

            // Per-weapon spacing weights
            if (weaponType == WeaponType.Spear) {
                if (hex.AIDistanceToEnemy == 1) {
                    result = +2;
                }
                if (hex.AIDistanceToEnemy == UnitBaseStats.MoveSpeed(unitType) + 1) {
                    result = -2;
                }
            }
            if (weaponType == WeaponType.Sword) {
                if (hex.AIDistanceToEnemy == 1) {
                    result = -1;
                }
                if (hex.AIDistanceToEnemy == 2) {
                    result = +1;
                }
                if (hex.AIDistanceToEnemy == UnitBaseStats.MoveSpeed(unitType) + 1) {
                    result = +2;
                }
            }
            if (weaponType == WeaponType.Flail) {
                result = -1 * hex.aiAdjacentEnemies;
            }
            if (weaponType == WeaponType.Longbow) {
                if (hex.AIDistanceToEnemy == 2) {
                    result = -4;
                }
                if (hex.AIDistanceToEnemy == 3) {
                    result = -8;
                }
            }
            if (weaponType == WeaponType.Dagger) {
                result = -2 * hex.aiAdjacentEnemies;
            }
            if (weaponType == WeaponType.Shield) {
                if (hex.AIDistanceToEnemy == 1) {
                    result = +3;
                }
            }

            //Generally encourage movement toward the enemy if out of range
            if (hex.AIDistanceToEnemy > UnitBaseStats.MoveSpeed(unitType) + 1) {
                result += hex.AIDistanceToEnemy - UnitBaseStats.MoveSpeed(unitType) - 1;
            }

            return result;
        }

        private int OutcomeDamageWeight(int player, List<Outcome> outcomes) {

            int result = 0;

            foreach (Outcome outcome in outcomes) {
                foreach (AttackResult attackResult in outcome.combat) {
                    if (attackResult.attackType == AttackType.Push) {
                        result += OutcomeDamageWeight(player, attackResult.pushMoves);
                    }
                    else if (attackResult.target.PlayerOwner == player) {
                        result += 5;
                    }
                    else {
                        result -= 5;
                    }
                }
            }

            return result;
        }

        private void PopulateHexDistancesToEnemies(int player) {

            // Reset hex distances
            foreach (HexEntry hex in scenarioLoader.HexGrid.Values) {
                hex.AIDistanceToEnemy = int.MaxValue;
                hex.aiAdjacentEnemies = 0;
            }

            foreach (int p in selectionManager.UnitsByPlayer.Keys) {
                if (p == player) { // Only enemy units
                    continue;
                }

                foreach (IUnitController unit in selectionManager.UnitsByPlayer[p]) {
                    foreach (HexEntry hex in scenarioLoader.HexGrid.Values) {
                        int distanceToUnit = (int)HexVectorUtil.AxialDistance(unit.Position.BoardPos, hex.BoardPos);
                        if (distanceToUnit < hex.AIDistanceToEnemy) {
                            hex.AIDistanceToEnemy = distanceToUnit; // Update nearest enemy distance
                        }
                        if (distanceToUnit == 1) {
                            hex.aiAdjacentEnemies++; // Update adjacent enemies
                        }
                    }
                }
            }
        }

        public static List<WeaponSelect.WSController.WSUnitDescriptor> DefaultUnitPicker(List<Vector2> unitPositions, int player) {
            List<WeaponSelect.WSController.WSUnitDescriptor> output = new List<WeaponSelect.WSController.WSUnitDescriptor>();

            List<Vector2> randomizedPositions = unitPositions.OrderBy(x => Random.Range(0f, 1f)).ToList();

            for (int i = 0; i < unitPositions.Count; i++) {
                
                int reducedIterator = (i) % 7;

                bool r1 = Random.Range(0, 2) == 1;
                bool r2 = Random.Range(0, 2) == 1;
                if (reducedIterator == 0) {
                    output.Add(new WeaponSelect.WSController.WSUnitDescriptor(randomizedPositions[i],
                        UnitBaseType.Simple, r2 ? WeaponType.Spear : WeaponType.Sword, player));
                }
                else if (reducedIterator == 1) {
                    output.Add(new WeaponSelect.WSController.WSUnitDescriptor(randomizedPositions[i],
                        r1 ? UnitBaseType.Sturdy : UnitBaseType.Simple, r2 ? WeaponType.Longbow : WeaponType.Dagger, player));
                }
                else if (reducedIterator == 2) {
                    output.Add(new WeaponSelect.WSController.WSUnitDescriptor(randomizedPositions[i],
                        r1 ? UnitBaseType.Swift : UnitBaseType.Simple, WeaponType.Flail, player));
                }
                else if (reducedIterator == 3) {
                    output.Add(new WeaponSelect.WSController.WSUnitDescriptor(randomizedPositions[i],
                        r1 ? UnitBaseType.Swift : UnitBaseType.Simple, WeaponType.Shield, player));
                }
                else if (reducedIterator == 4) {
                    output.Add(new WeaponSelect.WSController.WSUnitDescriptor(randomizedPositions[i],
                        UnitBaseType.Simple, r2 ? WeaponType.Longbow : WeaponType.Flail, player));
                }
                else if (reducedIterator == 5) {
                    output.Add(new WeaponSelect.WSController.WSUnitDescriptor(randomizedPositions[i],
                        r1 ? UnitBaseType.Sturdy : UnitBaseType.Simple, r2 ? WeaponType.Sword : WeaponType.Flail, player));
                }
                else if (reducedIterator == 6) {
                    output.Add(new WeaponSelect.WSController.WSUnitDescriptor(randomizedPositions[i],
                        r1 ? UnitBaseType.Swift : UnitBaseType.Simple, r2 ? WeaponType.Spear : WeaponType.Sword, player));
                }
            }

            return output;
        }
    }
}