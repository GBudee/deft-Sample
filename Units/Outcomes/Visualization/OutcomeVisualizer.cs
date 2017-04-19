using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {
    public class OutcomeVisualizer {
        List<HexEntry> redHexes;
        PathVisualizer pathVisualizer;
        List<IUnitController> rangeIndicators;

        public OutcomeVisualizer(PathVisualizer pathVisualizer) {
            this.pathVisualizer = pathVisualizer;

            redHexes = new List<HexEntry>();
            rangeIndicators = new List<IUnitController>();
        }

        public void Interpret(List<Outcome> outcomes, HexEntry footstepStart = null) {

            if (footstepStart != null) {
                pathVisualizer.SetPath(outcomes, footstepStart);
            }

            foreach (Outcome outcome in outcomes) {
                foreach (AttackResult attackResult in outcome.combat) {

                    if (attackResult.attackType != AttackType.Push) {
                        attackResult.targetHex.HexManager.BorderColor = Config.Palette.attack;
                    }
                    else {
                        attackResult.targetHex.HexManager.BorderColor = Config.Palette.pushAttack;
                        attackResult.targetHex.HexManager.gameObject.transform.position = new Vector3(attackResult.targetHex.HexManager.gameObject.transform.position.x,
                            attackResult.targetHex.HexManager.gameObject.transform.position.y, 0); // Make sure push attack borders go behind damaging attack borders
                    }
                    redHexes.Add(attackResult.targetHex);

                    pathVisualizer.CreateHitArrow(attackResult.sourceHex, attackResult.targetHex);

                    attackResult.source.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.CombatVisualization, true);
                    rangeIndicators.Add(attackResult.source);

                    if (attackResult.source.PlayerOwner == attackResult.target.PlayerOwner) {
                        pathVisualizer.CreateFriendlyFireWarning(outcome.position);
                    }

                    if (attackResult.pushMoves != null) {
                        Interpret(attackResult.pushMoves);
                    }
                }
            }
        }

        public void Revert() {
            pathVisualizer.ClearPath();
            pathVisualizer.ClearArrows();
            pathVisualizer.ClearFriendlyFireWarnings();
            foreach (HexEntry hex in redHexes) {
                hex.HexManager.BorderColor = Color.clear;
                hex.HexManager.gameObject.transform.position = new Vector3(hex.HexManager.gameObject.transform.position.x,
                    hex.HexManager.gameObject.transform.position.y, 1); // Make sure push attack borders go behind damaging attack borders
            }
            redHexes = new List<HexEntry>();

            foreach (IUnitController unit in rangeIndicators) {
                unit.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.CombatVisualization, false);
            }
        }
    }
}
