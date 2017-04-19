using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Deft {
    public class EmptyWeapon : IWeapon {

        protected GameManager gameManager;
        protected ScenarioLoader scenarioLoader;
        protected SelectionManager selectionManager;
        protected IUnitController unitController;

        public EmptyWeapon(IUnitController unitController, GameManager gameManager, ScenarioLoader scenarioLoader, SelectionManager selectionManager) {
            this.unitController = unitController;
            this.gameManager = gameManager;
            this.scenarioLoader = scenarioLoader;
            this.selectionManager = selectionManager;
        }

        public virtual List<AttackResult> CombatResults(List<HexEntry> recentPath) {

            List<AttackResult> allCombat = new List<AttackResult>();

            //Firststrike retal
            foreach (int player in gameManager.playerDirectory.Keys) {
                foreach (IUnitController unit in selectionManager.UnitsByPlayer[player]) {
                    if (unit.WeaponType == WeaponType.Spear && unitController.WeaponType != WeaponType.Spear && unitController.WeaponType != WeaponType.Dagger) {
                        int damageReceived = unit.Weapon.CheckRetal(recentPath, unitController);
                        if (damageReceived > 0) {
                            allCombat.Add(new AttackResult(unitController, unit, unitController.SimHealthAfterXDamage(damageReceived),
                                unit.SimPosition, recentPath[recentPath.Count - 1], AttackType.Retal));
                        }
                    }
                }
            }

            //Melee
            if (unitController.DamageOutput > 0) {
                allCombat = allCombat.Concat(CheckMelee(recentPath)).ToList();
            }

            //Retal
            foreach (int player in gameManager.playerDirectory.Keys) {
                foreach (IUnitController unit in selectionManager.UnitsByPlayer[player].OrderBy(x => x.WeaponType)) {
                    if ((unit.WeaponType != WeaponType.Spear || unitController.WeaponType == WeaponType.Spear) && unitController.WeaponType != WeaponType.Dagger) {
                        int damageReceived = unit.Weapon.CheckRetal(recentPath, unitController);
                        if (damageReceived > 0) { // Check to not do unnecessary attacks - important in the instance of shot-limited weapons
                            allCombat.Add(new AttackResult(unitController, unit, unitController.SimHealthAfterXDamage(damageReceived),
                                unit.SimPosition, recentPath[recentPath.Count - 1], AttackType.Retal));
                        }
                    }
                }
            }

            //Support
            foreach (int player in gameManager.playerDirectory.Keys) {
                if (player != unitController.PlayerOwner) { // First check enemy units
                    foreach (IUnitController enemy in selectionManager.UnitsByPlayer[player].OrderBy(x => x.WeaponType)) {
                        foreach (AttackResult retalResult in allCombat.Where(x => x.attackType == AttackType.Retal).ToList()) {
                            int damageDealt = enemy.Weapon.CheckSupport(retalResult.target);
                            if (damageDealt > 0 && retalResult.target.SimHealth() > 0 && retalResult.target.SimPosition.Terrain != Terrain.Pit) { // Check to not do unnecessary attacks - important in the instance of shot-limited weapons
                                allCombat.Add(new AttackResult(retalResult.target, enemy, retalResult.target.SimHealthAfterXDamage(damageDealt),
                                    enemy.SimPosition, retalResult.target.SimPosition, AttackType.Support));
                            }
                        }
                    }
                }
                else {
                    foreach (IUnitController ally in selectionManager.UnitsByPlayer[player].OrderBy(x => x.WeaponType)) { // Ordering property means daggers attack first
                        foreach (AttackResult meleeResult in allCombat.Where(x => x.attackType == AttackType.Melee).ToList()) {
                            int damageDealt = ally.Weapon.CheckSupport(meleeResult.target);
                            if (damageDealt > 0 && meleeResult.target.SimHealth() > 0 && meleeResult.target.SimPosition.Terrain != Terrain.Pit) { // Check to not do unnecessary attacks - important in the instance of shot-limited weapons
                                allCombat.Add(new AttackResult(meleeResult.target, ally, meleeResult.target.SimHealthAfterXDamage(damageDealt),
                                    ally.SimPosition, meleeResult.target.SimPosition, AttackType.Support));
                            }
                        }
                    }
                }
            }

            return allCombat;
        }

        protected virtual List<AttackResult> CheckMelee(List<HexEntry> recentPath) {

            List<AttackResult> affectedCombats =
                CheckHit(recentPath)
                    .Where(x => x != null)
                    .Where(x => x.PlayerOwner != unitController.PlayerOwner)
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
        public virtual int CheckRetal(List<HexEntry> enemyRecentPath, IUnitController enemy) {

            List<IUnitController> affectedCombats =
                CheckHit(enemyRecentPath)
                .Where(x => x == unitController)
                .ToList();

            if (affectedCombats.Count > 0 && enemy.PlayerOwner != unitController.PlayerOwner && enemy.SimHealth() > 0 && enemy.SimPosition.Terrain != Terrain.Pit) {
                return unitController.DamageOutput;
            }
            else {
                return 0;
            }
        }

        public virtual int CheckSupport(IUnitController target) {
            return 0;
        }

        protected virtual List<IUnitController> CheckHit(List<HexEntry> recentPath) {
            return new List<IUnitController>();
        }
    }
}