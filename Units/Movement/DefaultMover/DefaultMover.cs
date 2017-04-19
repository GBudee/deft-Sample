using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {
    class DefaultMover : IMover {
        GameManager gameManager;
        SelectionManager selectionManager;
        ScenarioLoader scenarioLoader;
        IUnitController unitController;

        // Internal to the class
        private List<List<HexEntry>> allPaths;

        public DefaultMover(IUnitController unitController, GameManager gameManager, SelectionManager selectionManager, ScenarioLoader scenarioLoader) {
            this.gameManager = gameManager;
            this.unitController = unitController;
            this.selectionManager = selectionManager;
            this.scenarioLoader = scenarioLoader;
        }

        // Simulates execution of an actual move and produces a list of Outcomes
        public List<Outcome> MoveOutcomes(HexEntry dest) {
            return DefaultPathSelector.ChooseBestPath(allPaths, dest, unitController, SimulatePath);
        }

        // We need to simulate a path's outcomes to avoid making changes to the unit's actual state.
        private List<Outcome> SimulatePath(List<HexEntry> path) {
            List<Outcome> outcomes = new List<Outcome>();

            for (int i = 1; i < path.Count; i++) {
                if (path[i].SimOccupant != null) { //&& path[i].SimOccupant != unitController) { // Necessary because if a unit is pushed midpath, the pathgenerator doesn't understand that
                    break;
                }
                unitController.SimPosition = path[i];

                // Keep track of data to feed the Outcome constructor
                List<AttackResult> combat = unitController.Weapon.CombatResults(unitController.SimRecentPath); // Contains all simulatepush calls

                outcomes.Add(new Outcome(unitController, path[i], true, combat));

                if (unitController.SimHealth() <= 0) {
                    break;
                }
            }

            foreach (HexEntry hex in scenarioLoader.HexGrid.Values) {
                hex.SimOccupant = null;
            }
            foreach (int player in gameManager.playerDirectory.Keys) {
                foreach (IUnitController unit in selectionManager.UnitsByPlayer[player]) {
                    unit.ResetSimulationState();
                }
            }

            return outcomes;
        }


        public List<Outcome> SimulatePush(IUnitController target, Vector2 pushDirection, int pushAmount) {
            List<Outcome> outcomes = new List<Outcome>();

            //outcomes.Add(new Outcome(target, target.SimPosition)); // Add current hex for interpreter convenience

            if (pushAmount > 0) {
                for (int i = 1; i <= pushAmount; i++) {
                    if (!scenarioLoader.HexGrid.ContainsKey(pushDirection + target.SimPosition.BoardPos)) {
                        break;
                    }
                    HexEntry nextHex = scenarioLoader.HexGrid[pushDirection + target.SimPosition.BoardPos];
                    if (nextHex.SimOccupant != null) {
                        break;
                    }
                    if (UnitBaseStats.TerrainCost(target.UnitType, nextHex.Terrain) == 0) {
                        break;
                    }
                    target.SimPosition = nextHex;
                    List<AttackResult> combat = target.Weapon.CombatResults(target.SimRecentPath);
                    outcomes.Add(new Outcome(target, nextHex, false, combat));
                }
            }

            return outcomes;
        }

        // Used by SelectionManager to get a list of reachable hexes for a given unit. Used for UI cues.
        public List<HexEntry> CalculatePaths() {
            allPaths = DefaultPathGenerator.FindAllPaths(unitController.Position, unitController.MovesRemaining, unitController);

            List<HexEntry> reachableHexes = allPaths
                .Aggregate(new List<HexEntry>(), (acc, x) => new List<HexEntry>(acc.Union(x)))
                .ToList();

            return reachableHexes;
        }

        /*
        private static string PathToString(List<HexEntry> path) {
            string s = "";
            foreach (HexEntry he in path) {
                s += he.BoardPos + ", ";
            }
            return s;
        }
        private static string OutcomesToString(List<Outcome> outcomes) {
            string s = "";
            foreach (Outcome he in outcomes) {
                s += he.moveHex.BoardPos + ", ";
            }
            return s;
        }*/
    }
}

