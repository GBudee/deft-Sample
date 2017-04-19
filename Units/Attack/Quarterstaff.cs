using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Deft {
    public class Quarterstaff : EmptyWeapon, IWeapon {

        public Quarterstaff(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) : base(unitController, gameManager, scenarioLoader, selectionManager) { }

        public override List<AttackResult> CombatResults(List<HexEntry> recentPath) {

            List<AttackResult> allCombat = new List<AttackResult>();

            //Retal checks -- which are then input as melee
            foreach (int player in gameManager.playerDirectory.Keys) {
                foreach (IUnitController unit in selectionManager.UnitsByPlayer[player].OrderBy(x => x.WeaponType)) {
                    if (unit.WeaponType != WeaponType.Crossbow) {
                        int damageReceived = unit.Weapon.CheckRetal(recentPath, unitController);
                        if (damageReceived > 0 && unitController.SimHealth() > 0) { // Check to not do unnecessary attacks - important in the instance of shot-limited weapons
                            int dmg = unitController.DamageOutput;
                            allCombat.Add(new AttackResult(unit, unitController, unit.SimHealthAfterXDamage(dmg),
                                recentPath[recentPath.Count - 1], unit.SimPosition, AttackType.Melee));
                        }
                    }
                }
            }

            //Support
            foreach (int player in gameManager.playerDirectory.Keys) {
                if (player != unitController.PlayerOwner) { // Only check allied units (dagger isn't checked because the quarterstaff can't trigger retal attacks)
                    continue;
                }
                foreach (IUnitController ally in selectionManager.UnitsByPlayer[player].OrderBy(x => x.WeaponType)) {
                    foreach (AttackResult meleeResult in allCombat.Where(x => x.attackType == AttackType.Melee).ToList()) {
                        int damageDealt = ally.Weapon.CheckSupport(meleeResult.target);
                        if (damageDealt > 0 && meleeResult.target.SimHealth() > 0) { // Check to not do unnecessary attacks - important in the instance of shot-limited weapons
                            allCombat.Add(new AttackResult(meleeResult.target, ally, meleeResult.target.SimHealthAfterXDamage(damageDealt),
                                ally.SimPosition, meleeResult.target.SimPosition, AttackType.Support));
                        }
                    }
                }
            }

            //Retal from ranged weapons
            foreach (int player in gameManager.playerDirectory.Keys) {
                foreach (IUnitController unit in selectionManager.UnitsByPlayer[player].OrderBy(x => x.WeaponType)) {
                    if (unit.WeaponType == WeaponType.Crossbow) {
                        int damageReceived = unit.Weapon.CheckRetal(recentPath, unitController);
                        if (damageReceived > 0 && unitController.SimHealth() > 0) { // Check to not do unnecessary attacks - important in the instance of shot-limited weapons
                            allCombat.Add(new AttackResult(unitController, unit, unitController.SimHealthAfterXDamage(damageReceived),
                                unit.SimPosition, recentPath[recentPath.Count - 1], AttackType.Retal));
                        }
                    }
                }
            }

            return allCombat;
        }
    }
}