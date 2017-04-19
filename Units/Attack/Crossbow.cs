using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deft {
    class Crossbow : EmptyWeapon, IWeapon {

        public Crossbow(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) : base(unitController, gameManager, scenarioLoader, selectionManager) { }

        protected override List<AttackResult> CheckMelee(List<HexEntry> recentPath) {
            return new List<AttackResult>();
        }

        public override int CheckRetal(List<HexEntry> recentPath, IUnitController enemy) {
            if (recentPath.Count < 2 || enemy.PlayerOwner == unitController.PlayerOwner) {
                return 0;
            }

            HexEntry current = recentPath[recentPath.Count - 1];
            HexEntry last = recentPath[recentPath.Count - 2];

            if (HexVectorUtil.AxialDistance(current.BoardPos, unitController.SimPosition.BoardPos) == 3) {
                if (HexVectorUtil.AxialDistance(last.BoardPos, unitController.SimPosition.BoardPos) > 3) {
                    return unitController.DamageOutput;
                }
            }

            return 0;
        }
    }
}
