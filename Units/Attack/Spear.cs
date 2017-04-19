using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Deft {
    public class Spear : EmptyWeapon, IWeapon {

        public Spear(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) : base(unitController, gameManager, scenarioLoader, selectionManager) { }

        // First-strike property is defined within EmptyWeapon

        protected override List<IUnitController> CheckHit(List<HexEntry> recentPath) {
            if (recentPath.Count < 2) {
                return new List<IUnitController>();
            }

            HexEntry current = recentPath[recentPath.Count - 1];
            HexEntry last = recentPath[recentPath.Count - 2];

            Vector2 displacement = current.BoardPos - last.BoardPos;
            Vector2 targetPos = displacement + current.BoardPos;

            if (!scenarioLoader.HexGrid.ContainsKey(targetPos)) {
                return new List<IUnitController>();
            }
            else {
                return new List<IUnitController>() { scenarioLoader.HexGrid[targetPos].SimOccupant };
            }
        }
    }
}