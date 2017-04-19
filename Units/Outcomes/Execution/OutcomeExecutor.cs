using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deft {
    public class OutcomeExecutor {

        SelectionManager selectionManager;

        public OutcomeExecutor(SelectionManager selectionManager) {
            this.selectionManager = selectionManager;
        }

        // Update the actual game state after a commitment to a certain move
        public void ExecuteMoves(List<Outcome> outcomes, IUnitController unitSpendingMoves = null) {
            
            foreach (Outcome outcome in outcomes){

                if (outcome.activeUnit == unitSpendingMoves) {
                    //Deduct zone of control and movement costs before moving onto a hex
                    outcome.activeUnit.MovesRemaining -= UnitBaseStats.TerrainCost(outcome.activeUnit.UnitType, outcome.position.Terrain);
                    if (DefaultPathGenerator.IsEnemyNeighbor(outcome.activeUnit.Position, outcome.activeUnit.PlayerOwner)){
                        outcome.activeUnit.ZoCRemaining -= 1;
                        if (outcome.activeUnit.ZoCRemaining < 0) {
                            throw new System.Exception("This should not have happened");
                        }
                        else if (outcome.activeUnit.ZoCRemaining == 0) {
                            outcome.activeUnit.MovesRemaining = 0;
                        }
                    }
                }

                // Move to the next hex, tell it we're occupying it, and add hex to our recent path
                outcome.activeUnit.Position = outcome.position;

                foreach (AttackResult attackResult in outcome.combat) {
                    //Debug.Log("Execute attack on enemy at " + outcome.activeUnit.CurrentHex);
                    attackResult.source.ExecuteAttack();
                    attackResult.target.HP = attackResult.healthRemaining;
                    if (attackResult.healthRemaining <= 0) {
                        attackResult.target.Position = null;
                        selectionManager.UnitsByPlayer[attackResult.target.PlayerOwner].Remove(attackResult.target);
                    }
                    if (attackResult.pushMoves != null) {
                        ExecuteMoves(attackResult.pushMoves); // When executing forced movement, don't deduct ZoC or move points
                    }
                }

                if (outcome.activeUnit.Position != null && outcome.activeUnit.Position.Terrain == Terrain.Pit) {
                    outcome.activeUnit.Position = null;
                    selectionManager.UnitsByPlayer[outcome.activeUnit.PlayerOwner].Remove(outcome.activeUnit);
                }
            }
        }

        
        /*
        // Do accounting with ZoneOfControlMovesRemaining (Should be improved significantly)
        private void ZoneOfControlDeduction(List<Outcome> outcomes, HexEntry currentHex, IUnitController unitSpendingMoves) {

            for (var i = 0; i < outcomes.Count - 1; i++) {
                if (outcomes[i].activeUnit == unitSpendingMoves && DefaultPathGenerator.IsEnemyNeighbor(outcomes[i].position, unitSpendingMoves.PlayerOwner)) {
                    unitSpendingMoves.ZoCRemaining -= 1;
                }
            }

            if (unitSpendingMoves.ZoCRemaining < 0) {
                throw new System.Exception("This should not have happened");
            }
            else if (unitSpendingMoves.ZoCRemaining == 0) {
                outcomes[0].activeUnit.MovesRemaining = 0;
            }
        }*/
    }
}