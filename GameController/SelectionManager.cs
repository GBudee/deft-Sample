using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Deft {
    public class SelectionManager : MonoBehaviour {
        private UIHandler uiHandler;
        private GameManager gameManager;
        private ScenarioLoader scenarioLoader;

        //Private cache and function tools
        private OutcomeVisualizer outcomeVisualizer;
        private OutcomeExecutor outcomeExecutor;
        private OutcomeAnimator outcomeAnimator;
        private IDictionary<HexEntry, List<Outcome>> outcomeCache = new Dictionary<HexEntry, List<Outcome>>();// Save outcomes calculated for visualization

        //Gameplay-related values
        private IUnitController _unitSelected; // The currently selected unit
        private HashSet<HexEntry> selectionReachableHexes; // Which hexes can the currently selected unit move to this turn
        private List<HexEntry> enemyReachableHexes; // Which hexes can a hovered enemy unit move to on its next turn
        public int turnsOfPlay; // How many turns have passed in the skirmish so far

        //All units, accessed from here by any part of deft
        private IDictionary<int, List<IUnitController>> _unitsByPlayer; //Actively updated
        public IDictionary<int, List<IUnitController>> UnitsByPlayer {
            get { return _unitsByPlayer; }
        }

        //Cache of unit states so player can undo moves and game can serialize a turn's worth of outcomes
        List<BoardState> boardStateHistory;

        private List<IUnitController> unitsByUniqueID; //Contains every unit ever created in the skirmish; used for serialization
        private List<Outcome> storedEnemyMoves;
        private BoardState storedEnemyPremoveState;
        public Coroutine playbackAnimation;

        #region Initialization
        public void Initialize() {
            uiHandler = GetComponent<UIHandler>();
            gameManager = GetComponent<GameManager>();
            scenarioLoader = GetComponent<ScenarioLoader>();

            outcomeVisualizer = new OutcomeVisualizer(GetComponent<PathVisualizer>());
            outcomeExecutor = new OutcomeExecutor(this);
            outcomeAnimator = new OutcomeAnimator(this, uiHandler);

            selectionReachableHexes = new HashSet<HexEntry>();
            enemyReachableHexes = new List<HexEntry>();

            UpdateUnitList();

            uiHandler.SlideCameraToHexAvg(UnitsByPlayer[gameManager.ActivePlayer].Select(x => x.Position).ToList());

            turnsOfPlay = 0;
            scenarioLoader.SetTurnsRemainingText(Config.turnsBeforePlayer1Wins - turnsOfPlay + 1);

            gameManager.AITryAITurn();

            if (gameManager.NetEnemyTurn()) { // Netcode
                uiHandler.NotYourTurn();
            }
        }

        public void UpdateUnitList() {

            _unitsByPlayer = new Dictionary<int, List<IUnitController>>();
            unitsByUniqueID = new List<IUnitController>();

            foreach (int player in gameManager.playerDirectory.Keys) {
                _unitsByPlayer[player] = new List<IUnitController>();
            }

            foreach (HexEntry hex in scenarioLoader.HexGrid.Values) {
                if (hex.Occupant != null) {
                    _unitsByPlayer[hex.Occupant.PlayerOwner].Add(hex.Occupant);
                    unitsByUniqueID.Add(hex.Occupant);
                }
            }

            /*
            foreach (IUnitController unit in unitsByUniqueID) {
                if (unit.PlayerOwner == gameManager.ActivePlayer) {
                    unit.SpriteManager.MovePointDisplay(true);
                }
                else {
                    unit.SpriteManager.MovePointDisplay(false);
                }
            }*/

            _unitSelected = null;
            boardStateHistory = new List<BoardState>();
            boardStateHistory.Add(CreateBoardState(new List<Outcome>()));
            storedEnemyPremoveState = boardStateHistory[0];
            storedEnemyMoves = new List<Outcome>();
        }
        #endregion

        #region MoveHistory (Undos, saving turns to submit)
        private BoardState CreateBoardState(List<Outcome> movesSinceLastSave) {

            List<UnitState> currentUnitStates = new List<UnitState>();
            foreach (IUnitController unit in unitsByUniqueID) {
                currentUnitStates.Add(unit.UnitCurrentState);
            }

            return new BoardState(currentUnitStates, movesSinceLastSave); //Currently doesn't save previous move set
        }

        public void RevertBoardState(BoardState argState = null) {

            bool playbackNotUndo;
            BoardState stateBeingRevertedTo;

            if (argState != null) {
                stateBeingRevertedTo = argState;
                playbackNotUndo = false;
            }
            else if (boardStateHistory.Count > 1) {

                boardStateHistory.RemoveAt(boardStateHistory.Count - 1); // Drop the most recent move
                playbackNotUndo = false;
                stateBeingRevertedTo = boardStateHistory[boardStateHistory.Count - 1];
            }
            else {
                playbackNotUndo = true;
                stateBeingRevertedTo = storedEnemyPremoveState;
            }

            if (stateBeingRevertedTo == null) {
                return;
            }

            outcomeAnimator.RevertAnimation();
            foreach (HexEntry hex in scenarioLoader.HexGrid.Values) {
                hex.Occupant = null;
                hex.SimOccupant = null;
            }
            foreach (UnitState unitState in stateBeingRevertedTo.unitStates) { // Revert every unit to the new most recent move
                unitState.UnitController.UnitCurrentState = unitState;
                unitState.UnitController.OutcomeAnimator.RevertAnimation(unitState.UnitController);

                
                if (unitState.UnitController.Position != null && !UnitsByPlayer[unitState.UnitController.PlayerOwner].Contains(unitState.UnitController)) { // If unit is not present in unitslist but is alive
                    UnitsByPlayer[unitState.UnitController.PlayerOwner].Add(unitState.UnitController);         // Add it back in
                }
            }

            UnitSelected = _unitSelected;

            if (playbackNotUndo) { // If no boardstatehistory i.e. nothing to undo
                PlayBackStoredTurns();
            }
        }

        public void DestroyAllUnits() {

            UnitSelected = null;
            outcomeAnimator.RevertAnimation();
            foreach (IUnitController unit in unitsByUniqueID) {
                unit.OutcomeAnimator.RevertAnimation(unit);
                unit.Position = null;
                unit.SpriteManager.DestroyUnit();
            }
            outcomeVisualizer = new OutcomeVisualizer(GetComponent<PathVisualizer>());
            unitsByUniqueID = null;
            _unitsByPlayer = null;
        }

        public class BoardState {
            public readonly List<UnitState> unitStates;
            public readonly List<Outcome> movesSincePrevState;

            public BoardState(List<UnitState> unitStates, List<Outcome> movesSincePrevState) {
                this.unitStates = unitStates;
                this.movesSincePrevState = movesSincePrevState;
            }
        }
        #endregion

        public void AIExecuteSubMoves(List<Outcome> subMoves, IUnitController unit) {
            outcomeExecutor.ExecuteMoves(subMoves, unit);
        }

        public void ImplementAITurn(List<Outcome> movesAsList) {  // Probably not a robust implementation for multiplayer

            storedEnemyMoves = storedEnemyMoves.Concat(movesAsList).ToList();

            playbackAnimation = outcomeAnimator.Interpret(movesAsList, true); // Camera is set to follow

            UnitSelected = _unitSelected;
        }

        #region Netcode (Serialization Unit IDs and Turn Playback)
        public void NetStoreEnemyTurn(List<Outcome> movesAsList) {

            storedEnemyMoves = storedEnemyMoves.Concat(movesAsList).ToList();
            uiHandler.NetTurnReady();
        }

        public void PlayBackStoredTurns() {

            outcomeExecutor.ExecuteMoves(storedEnemyMoves);
            playbackAnimation = outcomeAnimator.Interpret(storedEnemyMoves, true); // Camera is set to follow

            boardStateHistory = new List<BoardState>();
            boardStateHistory.Add(CreateBoardState(new List<Outcome>()));

            UnitSelected = _unitSelected;
        }

        public void AnimationComplete(Coroutine animation) {
            if (playbackAnimation == animation) {
                uiHandler.TurnButtonDisplay(gameManager.ActivePlayer);
                playbackAnimation = null;
            }
        }

        public IUnitController GetUnitByID(int id) {
            return unitsByUniqueID[id];
        }
        public int GetIDByUnit(IUnitController unit) {
            return unitsByUniqueID.IndexOf(unit);
        }
        #endregion

        #region Player Interactions
        public IUnitController UnitSelected {
            get { return _unitSelected; }
            set {
                if (_unitSelected != null) {
                    HideSelectedUnitReachableHexes();
                    _unitSelected.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Selection, false);
                    outcomeCache.Clear();
                    outcomeVisualizer.Revert();
                }

                _unitSelected = value;

                if (_unitSelected != null) {
                    _unitSelected.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Selection, true);
                }

                ShowSelectedUnitReachableHexes();
            }
        }

        public void SelectUnit(IUnitController unitController) {
            if (unitController.PlayerOwner == gameManager.ActivePlayer && _unitSelected != unitController) {
                UnitSelected = unitController;
            }
        }

        public void TravelToHex(HexEntry hex) {
            if (_unitSelected != null) {

                if (hex.Occupant == null && selectionReachableHexes.Contains(hex)) {

                    List<Outcome> outcomes = outcomeCache[hex];

                    outcomeExecutor.ExecuteMoves(outcomes, _unitSelected); // Immediately update all internal values to reflect the outcomes of the move
                    boardStateHistory.Add(CreateBoardState(outcomes));

                    outcomeAnimator.Interpret(outcomes); // Launch coroutine to animate move outcomes sequentially

                    if (_unitSelected.HP <= 0) {
                        UnitSelected = null;
                    }
                    else {
                        UnitSelected = _unitSelected;
                    }
                }
                else {
                    UnitSelected = null;
                }
            }
            else {
                Debug.Log("No unit selected");
            }
        }

        public void ShowPathToHex(HexEntry hex) {
            if (_unitSelected != null) {
                List<Outcome> outcomes;
                
                if (outcomeCache.ContainsKey(hex)) {
                    outcomes = outcomeCache[hex];
                }
                else {
                    outcomes = _unitSelected.Mover.MoveOutcomes(hex);
                    outcomeCache.Add(hex, outcomes);
                }
                if (outcomes.Count > 0 && outcomes[outcomes.Count - 1].position == hex) {
                    _unitSelected.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Selection, true, hex);
                }
                else {
                    _unitSelected.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Selection, true);// Show at true unit location
                }
                outcomeVisualizer.Interpret(outcomes, _unitSelected.Position);
            }
        }

        public void HidePathToHex(HexEntry hex) {
            if (_unitSelected != null) {
                outcomeVisualizer.Revert();
            }
        }

        private void ShowSelectedUnitReachableHexes() {
            if (_unitSelected != null) {
                foreach (HexEntry hex in _unitSelected.Mover.CalculatePaths()) {
                    selectionReachableHexes.Add(hex); // Not merely cosmetic!
                    hex.SelectionHighlighted = true;
                }
            }
        }

        private void HideSelectedUnitReachableHexes() {
            foreach (HexEntry hex in selectionReachableHexes) {
                hex.SelectionHighlighted = false;
            }
            selectionReachableHexes.Clear();
        }

        public void ShowEnemyReachableHexes(IUnitController unit) {
            if (unit.PlayerOwner != gameManager.ActivePlayer) {
                foreach (HexEntry hex in unit.Mover.CalculatePaths()) {
                    enemyReachableHexes.Add(hex);
                    hex.EnemyHighlighted = true;
                }
            }
        }

        public void HideEnemyReachableHexes() {
            foreach (HexEntry hex in enemyReachableHexes) {
                hex.EnemyHighlighted = false;
            }
            enemyReachableHexes.Clear();
        }
        #endregion

        public void EndTurn() {
            if (playbackAnimation != null) {
                RevertBoardState(CreateBoardState(new List<Outcome>()));
                return;
            }

            UnitSelected = null; // Clear selection

            gameManager.NetTrySendMoves(boardStateHistory);

            foreach (IUnitController unit in _unitsByPlayer[gameManager.ActivePlayer]) {
                unit.TurnEndUpkeep();
            }

            PlayerType previousPlayer = gameManager.playerDirectory[gameManager.ActivePlayer];

            switch (gameManager.ActivePlayer) {
                case 1:
                    gameManager.ActivePlayer = 2;
                    break;

                case 2:
                    gameManager.ActivePlayer = 1;
                    break;
            }

            /*
            foreach(IUnitController unit in unitsByUniqueID) {
                if (unit.PlayerOwner == gameManager.ActivePlayer) {
                    unit.SpriteManager.MovePointDisplay(true);
                }
                else {
                    unit.SpriteManager.MovePointDisplay(false);
                }
            }*/

            if (gameManager.NetEnemyTurn()) { // If it's an online enemy's turn, show the panel
                uiHandler.NotYourTurn();
            }

            uiHandler.SlideCameraToHexAvg(UnitsByPlayer[gameManager.ActivePlayer].Select(x => x.Position).ToList());

            foreach (IUnitController unit in _unitsByPlayer[gameManager.ActivePlayer]) {
                unit.TurnStartUpkeep();
            }

            if (previousPlayer == PlayerType.Local) {
                uiHandler.TurnButtonDisplay(gameManager.ActivePlayer); // Only update after local turns

                if (gameManager.playerDirectory[gameManager.ActivePlayer] == PlayerType.Local) { // If previous and current player were local, save boardstatehistory as storedenemymoves
                    storedEnemyPremoveState = boardStateHistory[0];
                    storedEnemyMoves = new List<Outcome>();
                    foreach (BoardState boardState in boardStateHistory) {
                        storedEnemyMoves = storedEnemyMoves.Concat(boardState.movesSincePrevState).ToList();
                    }
                }

                boardStateHistory = new List<BoardState>(); // Clear undos -- turn end button is commitment!
                boardStateHistory.Add(CreateBoardState(new List<Outcome>())); // Prepare next player's potential undo (after upkeep)

                if (gameManager.playerDirectory[gameManager.ActivePlayer] != PlayerType.Local) {
                    storedEnemyMoves = new List<Outcome>(); // For new nonlocal moves, clear enemy move cache
                    storedEnemyPremoveState = boardStateHistory[0];
                }
            }
            else {
                boardStateHistory = new List<BoardState>(); // Clear undos -- turn end button is commitment!
                boardStateHistory.Add(CreateBoardState(new List<Outcome>())); // Prepare next player's potential undo (after upkeep)
            }

            if (gameManager.ActivePlayer == 1) {
                turnsOfPlay++;
                scenarioLoader.SetTurnsRemainingText(Config.turnsBeforePlayer1Wins - turnsOfPlay + 1);
            }
            if (UnitsByPlayer[1].Count == 0) {
                uiHandler.DisplayVictory(2);
            }
            if (UnitsByPlayer[2].Count == 0) {
                uiHandler.DisplayVictory(1);
            }
            else if (turnsOfPlay > Config.turnsBeforePlayer1Wins) {
                uiHandler.DisplayVictory(1);
            }
            else {
                gameManager.AITryAITurn();
            }
        }

        public void Player2Win() {
            uiHandler.DisplayVictory(2);
        }
    }
}