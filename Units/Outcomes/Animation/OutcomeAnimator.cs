using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deft {
    public class OutcomeAnimator {
        //Outcome list, of which this is the Animation Interpreter
        List<Outcome> outcomeQueue;

        //Step animation local variables
        private Coroutine activeAnimation;
        private Coroutine deathAnimation;
        private bool cameraFollow;

        SelectionManager selectionManager;
        UIHandler uiHandler;

        public OutcomeAnimator(SelectionManager selectionManager, UIHandler uiHandler) {
            this.selectionManager = selectionManager;
            this.uiHandler = uiHandler;

            outcomeQueue = new List<Outcome>();
        }

        public Coroutine Interpret(List<Outcome> outcomes, bool cameraFollow = false) {

            this.cameraFollow = cameraFollow;

            // Append the incoming outcomes to the queue of outcomes to be animated
            foreach (Outcome outcome in outcomes) {
                outcomeQueue.Add(outcome);
            }

            if (activeAnimation == null && outcomeQueue.Count > 0) {
                activeAnimation = selectionManager.StartCoroutine(QueueAnimator());
            }

            return activeAnimation;

            //Debug.Log("Outcomes received by outcomeanimator: " + OutcomesToString(outcomes));

            //Debug.Log("Outcome queue: " + OutcomesToString(outcomeQueue));
        }

        IEnumerator QueueAnimator() {
            if (cameraFollow) {
                if (selectionManager.playbackAnimation == activeAnimation) { // If this is a newly created animation, and is of the playback type, changes the turn button display color
                    uiHandler.TurnButtonDisplay(outcomeQueue[0].activeUnit.PlayerOwner);
                }
                yield return new WaitForSeconds(Config.unitStepSpeed);
            }

            while (outcomeQueue.Count > 0) {
                Outcome currentOutcome = outcomeQueue[0];
                outcomeQueue.RemoveAt(0);

                currentOutcome.activeUnit.SpriteManager.VisiblePosition = currentOutcome.position; // Move one step on the grid
                if (cameraFollow) {
                    uiHandler.SlideCameraToHex(currentOutcome.position);
                }
                if (currentOutcome.spendingMoves && !cameraFollow) { // The camerafollow becomes active immediately even if it's a later part of the queue, so I know if an enemy move is about to play

                    currentOutcome.activeUnit.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Movement, true);

                    currentOutcome.activeUnit.SpriteManager.VisualMovesRemaining -=
                        UnitBaseStats.TerrainCost(currentOutcome.activeUnit.UnitType, currentOutcome.position.Terrain); // Deduct move points visually

                    if (outcomeQueue.Count > 0) {
                        if (outcomeQueue[0].activeUnit != currentOutcome.activeUnit) {

                            if (currentOutcome.activeUnit.ZoCRemaining == 0) {
                                currentOutcome.activeUnit.SpriteManager.VisualMovesRemaining = 0;
                            }
                        }
                    }
                    else {
                        if (currentOutcome.activeUnit.ZoCRemaining == 0) {
                            currentOutcome.activeUnit.SpriteManager.VisualMovesRemaining = 0;
                        }
                    }
                }

                foreach (AttackResult attackResult in currentOutcome.combat) {// Show hp on all units fighting this step
                    attackResult.target.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.CombatAnimation, true);
                    attackResult.source.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.CombatAnimation, true);
                }

                foreach (AttackResult attackResult in currentOutcome.combat) {// Animate combat
                    if (attackResult.source.WeaponType == WeaponType.Longbow || attackResult.source.WeaponType == WeaponType.Crossbow) {
                        attackResult.source.SpriteManager.ShootArrow(attackResult.target.SpriteManager);

                        yield return new WaitForSeconds(Config.shotTimer);
                    }
                    else {
                        attackResult.source.SpriteManager.Bump(attackResult.target.SpriteManager);

                        yield return new WaitForSeconds(Config.bumpTimer);
                    }

                    attackResult.target.SpriteManager.SetBorderColor(Config.Palette.attack);
                    attackResult.target.SpriteManager.Shake(4.0f);
                    attackResult.target.SpriteManager.CurrentHealthDisplay = attackResult.healthRemaining;

                    if (attackResult.pushMoves != null) {
                        //outcomeQueue = new Queue<Outcome>(outcomeQueue.Concat(attackResult.pushMoves)); // Not used so multiple units' animations can have simultaneity
                        yield return attackResult.target.OutcomeAnimator.Interpret(attackResult.pushMoves); // Animate push and any moves and attacks that go with it
                    }
                    else {
                        yield return new WaitForSeconds(Config.attackStepSpeed);
                    }

                    attackResult.target.SpriteManager.SetBorderColor(Config.Palette.border);
                    if (attackResult.healthRemaining <= 0 || attackResult.targetHex.Terrain == Terrain.Pit) { //Unit death animation
                        attackResult.target.OutcomeAnimator.AnimateDeath(attackResult.target.SpriteManager);
                    }

                }

                yield return new WaitForSeconds(Config.unitStepSpeed);

                currentOutcome.activeUnit.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Movement, false);

                foreach (AttackResult attackResult in currentOutcome.combat) {// Hide hp on all units that fought on this step
                    attackResult.target.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.CombatAnimation, false);
                    attackResult.source.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.CombatAnimation, false);
                }

            }

            selectionManager.AnimationComplete(activeAnimation);
            activeAnimation = null;
        }

        public void AnimateDeath(UnitSpriteManager spriteManager) {
            deathAnimation = selectionManager.StartCoroutine(DeathAnimation(spriteManager));
        }

        private IEnumerator DeathAnimation(UnitSpriteManager spriteManager) {
            spriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Death, false);
            spriteManager.FlashFade(Color.white);

            yield return new WaitForSeconds(Config.flashFadeDestroyTimer);

            spriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Death, false);
            spriteManager.DisableUnit();

            deathAnimation = null;
        }

        public void RevertAnimation(IUnitController unitController = null) {
            if (activeAnimation != null) {
                selectionManager.StopCoroutine(activeAnimation); //Don't animate any further
                selectionManager.AnimationComplete(activeAnimation);
                activeAnimation = null;

            }
            if (deathAnimation != null) {
                selectionManager.StopCoroutine(deathAnimation);
                deathAnimation = null;
            }
            outcomeQueue.Clear();

            if (unitController != null) {
                unitController.SpriteManager.VisiblePosition = unitController.Position;
                unitController.SpriteManager.VisualMovesRemaining = unitController.MovesRemaining;
                unitController.SpriteManager.RevertUnitVisuals();
                unitController.SpriteManager.CurrentHealthDisplay = unitController.HP;
                if (unitController.Position == null) {
                    unitController.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Death, false);
                    unitController.SpriteManager.DisableUnit();
                }
            }
        }

        /*
        public static string OutcomesToString(List<Outcome> path) {
            string s = "";
            foreach (Outcome he in path) {
                s += he.moveHex.BoardPos + ", ";
            }
            return s;
        }*/
    }
}