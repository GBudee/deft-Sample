using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {

    public enum AttackType { Melee, Support, Retal, Push }

    public class AttackResult {
        public readonly IUnitController target;
        public readonly IUnitController source;
        public readonly int healthRemaining;
        public readonly HexEntry sourceHex;
        public readonly HexEntry targetHex;
        public readonly AttackType attackType;
        public readonly List<Outcome> pushMoves;

        public AttackResult(IUnitController target, IUnitController source, int healthRemaining,
                HexEntry sourceHex, HexEntry targetHex, AttackType attackType, List<Outcome> pushMoves = null) {

            this.target = target;
            this.source = source;
            this.healthRemaining = healthRemaining;
            this.sourceHex = sourceHex;
            this.targetHex = targetHex;
            this.attackType = attackType;
            this.pushMoves = pushMoves;
        }

        public AttackResult(SelectionManager selectionManager, ScenarioLoader scenarioLoader, SZAttackResult toCopy) {
            target = selectionManager.GetUnitByID(toCopy.target);
            source = selectionManager.GetUnitByID(toCopy.source);
            healthRemaining = toCopy.healthRemaining;
            sourceHex = scenarioLoader.GetHexByID(toCopy.sourceHex);
            targetHex = scenarioLoader.GetHexByID(toCopy.targetHex);
            attackType = toCopy.attackType;

            if (toCopy.pushMoves == null) {
                pushMoves = null;
            }
            else {
                pushMoves = new List<Outcome>();
                foreach (SZOutcome outcome in toCopy.pushMoves) {
                    pushMoves.Add(new Outcome(selectionManager, scenarioLoader, outcome));
                }
            }
        }
    }

    [Serializable]
    public class SZAttackResult {
        public readonly int target;
        public readonly int source;
        public readonly int healthRemaining;
        public readonly int sourceHex;
        public readonly int targetHex;
        public readonly AttackType attackType;
        public readonly List<SZOutcome> pushMoves;

        public SZAttackResult(SelectionManager selectionManager, ScenarioLoader scenarioLoader, AttackResult toCopy) {
            target = selectionManager.GetIDByUnit(toCopy.target);
            source = selectionManager.GetIDByUnit(toCopy.source);
            healthRemaining = toCopy.healthRemaining;
            sourceHex = scenarioLoader.GetIDByHex(toCopy.sourceHex);
            targetHex = scenarioLoader.GetIDByHex(toCopy.targetHex);
            attackType = toCopy.attackType;
            

            if (toCopy.pushMoves == null) {
                pushMoves = null;
            }
            else {
                pushMoves = new List<SZOutcome>();
                foreach (Outcome outcome in toCopy.pushMoves) {
                    pushMoves.Add(new SZOutcome(selectionManager, scenarioLoader, outcome));
                }
            }
        }
    }

    public class Outcome {
        public readonly IUnitController activeUnit;
        public readonly HexEntry position;
        public readonly bool spendingMoves;
        public readonly List<AttackResult> combat;

        public Outcome(IUnitController activeUnit, HexEntry position, bool spendingMoves, List<AttackResult> combat = null) {
            this.activeUnit = activeUnit;
            this.position = position;
            this.spendingMoves = spendingMoves;

            if (combat == null) {
                this.combat = new List<AttackResult>();
            }
            else {
                this.combat = combat;
            }
        }

        public Outcome(SelectionManager selectionManager, ScenarioLoader scenarioLoader, SZOutcome toCopy) {
            activeUnit = selectionManager.GetUnitByID(toCopy.activeUnit);
            position = scenarioLoader.GetHexByID(toCopy.position);
            spendingMoves = toCopy.spendingMoves;

            combat = new List<AttackResult>();
            foreach (SZAttackResult attackResult in toCopy.combat) {
                combat.Add(new AttackResult(selectionManager, scenarioLoader, attackResult));
            }
        }
    }

    [Serializable]
    public class SZOutcome {
        public readonly int activeUnit;
        public readonly int position;
        public readonly bool spendingMoves;
        public readonly List<SZAttackResult> combat;

        public SZOutcome(SelectionManager selectionManager, ScenarioLoader scenarioLoader, Outcome toCopy) {
            activeUnit = selectionManager.GetIDByUnit(toCopy.activeUnit);
            position = scenarioLoader.GetIDByHex(toCopy.position);
            spendingMoves = toCopy.spendingMoves;

            combat = new List<SZAttackResult>();
            foreach (AttackResult attackResult in toCopy.combat) {
                combat.Add(new SZAttackResult(selectionManager, scenarioLoader, attackResult));
            }
        }
    }
}