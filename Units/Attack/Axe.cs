using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Deft {
    public class Axe : EmptyWeapon, IWeapon {

        public Axe(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) : base(unitController, gameManager, scenarioLoader, selectionManager) { }
        
        protected override List<IUnitController> CheckHit(List<HexEntry> recentPath) {
            if (recentPath.Count < 2) {
                return new List<IUnitController>();
            }

            HexEntry current = recentPath[recentPath.Count - 1];
            HexEntry last = recentPath[recentPath.Count - 2];

            Vector2 displacement = current.BoardPos - last.BoardPos;
            Vector2 targetPos1 = current.BoardPos + HexVectorUtil.RotateClockwise(displacement);
            Vector2 targetPos2 = current.BoardPos + HexVectorUtil.RotateCounterclockwise(displacement);

            List<IUnitController> results = new List<IUnitController>();
            if (scenarioLoader.HexGrid.ContainsKey(targetPos1)) {
                results.Add(scenarioLoader.HexGrid[targetPos1].SimOccupant);
            }
            if (scenarioLoader.HexGrid.ContainsKey(targetPos2)) {
                results.Add(scenarioLoader.HexGrid[targetPos2].SimOccupant);
            }
            return results;

        }
    }
}