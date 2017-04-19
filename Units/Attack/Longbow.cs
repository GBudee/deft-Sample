using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deft {
    class Longbow : EmptyWeapon, IWeapon {

        public Longbow(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) : base(unitController, gameManager, scenarioLoader, selectionManager) { }

        protected override List<AttackResult> CheckMelee(List<HexEntry> recentPath) {
            return new List<AttackResult>();
        }

        public override int CheckRetal(List<HexEntry> recentPath, IUnitController enemy) {
            return 0;
        }

        public override int CheckSupport(IUnitController target) {
            if (HexVectorUtil.AxialDistance(unitController.SimPosition.BoardPos, target.SimPosition.BoardPos) <= 3 && target != unitController) {
                return unitController.DamageOutput;
            }
            else {
                return 0;
            }
        }
    }
}
