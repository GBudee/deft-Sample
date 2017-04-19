using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Deft {
    public class Flail : EmptyWeapon, IWeapon {

        public Flail(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) : base(unitController, gameManager, scenarioLoader, selectionManager) { }

        protected override List<AttackResult> CheckMelee(List<HexEntry> recentPath) {

            List<AttackResult> affectedCombats =
                CheckHit(recentPath)
                    .Where(x => x != null)
                    .Where(x => x != unitController) //.Where(x => x.PlayerOwner != unitController.PlayerOwner) -- Flail hits allies
                    .Where(x => x.SimHealth() > 0 && x.SimPosition.Terrain != Terrain.Pit)
                    .Select(x => {
                        if (x.WeaponType == WeaponType.Shield) {
                            List<Outcome> push = unitController.Mover.SimulatePush(x, x.SimPosition.BoardPos - unitController.SimPosition.BoardPos, 1);
                            if (push.Count >= 1) {
                                return new AttackResult(x, unitController, x.SimHealth(),
                                    recentPath[recentPath.Count - 1], x.SimPosition, AttackType.Push, push);
                            }
                        }
                        int dmg = unitController.DamageOutput;
                        return new AttackResult(x, unitController, x.SimHealthAfterXDamage(dmg),
                            recentPath[recentPath.Count - 1], x.SimPosition, AttackType.Melee);
                    })
                    .ToList();

            return affectedCombats;
        }

        // gets called by other units
        public override int CheckRetal(List<HexEntry> enemyRecentPath, IUnitController enemy) {

            List<IUnitController> affectedCombats =
                CheckHit(enemyRecentPath)
                .Where(x => x == unitController)
                .ToList();

            if (affectedCombats.Count > 0 && enemy != unitController && enemy.SimHealth() > 0 && enemy.SimPosition.Terrain != Terrain.Pit) { // && enemy.PlayerOwner != unitController.PlayerOwner) { -- Flail hits allies
                return unitController.DamageOutput;
            }
            else {
                return 0;
            }
        }

        protected override List<IUnitController> CheckHit(List<HexEntry> recentPath) {
            if (recentPath.Count < 2) {
                return new List<IUnitController>();
            }

            HexEntry current = recentPath[recentPath.Count - 1];
            HexEntry last = recentPath[recentPath.Count - 2];

            Vector2 displacement = current.BoardPos - last.BoardPos;
            List<Vector2> targetPositions = new List<Vector2>();
            targetPositions.Add(current.BoardPos + displacement);
            targetPositions.Add(current.BoardPos + HexVectorUtil.RotateClockwise(displacement));
            targetPositions.Add(current.BoardPos + HexVectorUtil.RotateClockwise(HexVectorUtil.RotateClockwise(displacement)));
            targetPositions.Add(current.BoardPos + HexVectorUtil.RotateCounterclockwise(displacement));
            targetPositions.Add(current.BoardPos + HexVectorUtil.RotateCounterclockwise(HexVectorUtil.RotateCounterclockwise(displacement)));

            /* // For some unknowable reason this implementation doesn't work right 
             * // The unit tries to attack itself I guess (includes current position)?
             * // But the behavior it creates depends on where you hover your mouse prior to clicking
            List<Vector2> targetPositions = new List<Vector2>();
            foreach (Bearing b in Enum.GetValues(typeof(Bearing))) {
                targetPositions.Add(current.BoardPos + HexVectorUtil.NeighborOffsetFromBearing(b));
            }*/

            List<IUnitController> results = new List<IUnitController>();
            foreach (Vector2 targetPos in targetPositions) {
                if (scenarioLoader.HexGrid.ContainsKey(targetPos)) {
                    results.Add(scenarioLoader.HexGrid[targetPos].SimOccupant);
                }
            }
            return results;
        }
    }
}