using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Deft {
    public class Shield : EmptyWeapon, IWeapon {

        public Shield(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) : base(unitController, gameManager, scenarioLoader, selectionManager) { }

        protected override List<AttackResult> CheckMelee(List<HexEntry> recentPath) {

            List<AttackResult> affectedCombats =
                CheckHit(recentPath)
                    .Where(x => x != null)
                    .Where(x => x.PlayerOwner != unitController.PlayerOwner)
                    .Where(x => x.SimHealth() > 0 && x.SimPosition.Terrain != Terrain.Pit)
                    .Select(x => {
                        List<Outcome> push = unitController.Mover.SimulatePush(x, x.SimPosition.BoardPos - unitController.SimPosition.BoardPos, 1);
                        if (push.Count >= 1) {
                            return new AttackResult(x, unitController, x.SimHealth(),
                                recentPath[recentPath.Count - 1], x.SimPosition, AttackType.Push, push);
                        }
                        int dmg = unitController.DamageOutput;
                        return new AttackResult(x, unitController, x.SimHealthAfterXDamage(dmg),
                            recentPath[recentPath.Count - 1], x.SimPosition, AttackType.Melee);
                    })
                    .ToList();

            return affectedCombats;
        }

        public override int CheckRetal(List<HexEntry> enemyRecentPath, IUnitController enemy) {
            return 0;
        }

        protected override List<IUnitController> CheckHit(List<HexEntry> recentPath) {
            if (recentPath.Count < 2) {
                return new List<IUnitController>();
            }

            HexEntry current = recentPath[recentPath.Count - 1];
            HexEntry last = recentPath[recentPath.Count - 2];

            Vector2 displacement = current.BoardPos - last.BoardPos;
            Vector2 targetPos1 = displacement + current.BoardPos;
            Vector2 targetPos2 = current.BoardPos + HexVectorUtil.RotateClockwise(displacement);
            Vector2 targetPos3 = current.BoardPos + HexVectorUtil.RotateCounterclockwise(displacement);

            List<IUnitController> results = new List<IUnitController>();
            if (scenarioLoader.HexGrid.ContainsKey(targetPos1)) {
                results.Add(scenarioLoader.HexGrid[targetPos1].SimOccupant);
            }
            if (scenarioLoader.HexGrid.ContainsKey(targetPos2)) {
                results.Add(scenarioLoader.HexGrid[targetPos2].SimOccupant);
            }
            if (scenarioLoader.HexGrid.ContainsKey(targetPos3)) {
                results.Add(scenarioLoader.HexGrid[targetPos3].SimOccupant);
            }
            return results;
        }
    }
}