using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {
    //A Weapon script deals damage when a Mover moves a unit (including enemy units)
    //A Weapon may also need to describe its attack pattern for display purposes
    public interface IWeapon {

        List<AttackResult> CombatResults(List<HexEntry> recentPath);

        // Called by individuals against an enemy's recent path. True if hit, otherwise false. Passed unitController in order to check Player identity
        int CheckRetal(List<HexEntry> enemyRecentPath, IUnitController enemy);
        int CheckSupport(IUnitController target);
    }
}
