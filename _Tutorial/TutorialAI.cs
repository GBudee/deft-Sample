using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deft;
using UnityEngine;

namespace Tutorial {
    public class TutorialAI : IComputerPlayer {

        TutorialManager tutorialManager;
        SelectionManager selectionManager;
        ScenarioLoader scenarioLoader;

        public TutorialAI(TutorialManager tutorialManager, SelectionManager selectionManager, ScenarioLoader scenarioLoader) {
            this.tutorialManager = tutorialManager;
            this.selectionManager = selectionManager;
            this.scenarioLoader = scenarioLoader;
        }

        public List<Outcome> GenerateTurn(int player) {

            List<Vector2> vectorList = new List<Vector2>();

            if (tutorialManager.activeSection == 5) {

                IUnitController unitBeingMoved = selectionManager.UnitsByPlayer[2][0];
                vectorList.Add(new Vector2(-2, 0));
                vectorList.Add(new Vector2(-1, -1));
                vectorList.Add(new Vector2(0, -1));
                List<Outcome> outcomeList = OutcomesFromVectors(vectorList, unitBeingMoved);
                selectionManager.AIExecuteSubMoves(outcomeList, unitBeingMoved);
                return outcomeList;
            }
            else {
                Debug.Log("Ended turn at wrong time--no ai move exists for this part of the tutorial");
            }
            return new List<Outcome>();
        }

        private List<Outcome> OutcomesFromVectors(List<Vector2> vectorList, IUnitController unit) {

            List<Outcome> outcomeList = new List<Outcome>();

            foreach (Vector2 boardPos in vectorList) {
                outcomeList.Add(new Outcome(unit, scenarioLoader.HexGrid[boardPos], true));
            }

            return outcomeList;
        }
    }
}