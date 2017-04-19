using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deft {
    public class UIHandler : MonoBehaviour, IUIHandler {

        [SerializeField]
        UnityEngine.UI.Button playerTurnDisplay;
        [SerializeField]
        UnityEngine.UI.Button undoButtonDisplay;
        [SerializeField]
        UnityEngine.UI.Image popupPanel;

        GameManager gameManager;
        SelectionManager selectionManager;
        ScenarioLoader scenarioLoader;
        CameraHandler cameraHandler;

        // Store hex or unit which sent callbacks
        public HexEntry hoveredHex;
        public IUnitController hoveredUnit;

        //bool inputEnabled;
        public class InputEnabled {
            private UIHandler uiHandler;

            public bool hexHover;
            public bool hexClick;
            public bool unitHover;
            public bool unitClick;
            public bool undoMove;
            private bool _turnEnd;
            public bool TurnEnd {
                get { return _turnEnd; }
                set {
                    _turnEnd = value;
                    uiHandler.playerTurnDisplay.interactable = _turnEnd;
                }
            }

            public InputEnabled(bool hexHover, bool hexClick, bool unitHover, bool unitClick, bool undoMove, bool turnEnd, UIHandler uiHandler) {
                this.uiHandler = uiHandler;

                this.hexHover = hexHover;
                this.hexClick = hexClick;
                this.unitHover = unitHover;
                this.unitClick = unitClick;
                this.undoMove = undoMove;
                TurnEnd = turnEnd;
            }
        }
        public InputEnabled inputEnabled;
        public List<HexEntry> permittedHexes; // If null, not checked
        public List<IUnitController> permittedUnits; // If null, not checked

        public void Initialize() {
            gameManager = GetComponent<GameManager>();
            selectionManager = GetComponent<SelectionManager>();
            scenarioLoader = GetComponent<ScenarioLoader>();
            cameraHandler = GetComponent<CameraHandler>();

            EnableInputs();

            cameraHandler.Initialize(scenarioLoader.HexGrid.Values
                .Select(x => HexVectorUtil.worldPositionOfHexCoord(x.BoardPos)).ToList(), new Vector2(0, 0), new Vector2(1, 1));
        }

        public void EnableInputs() {
            inputEnabled = new InputEnabled(true, true, true, true, true, true, this);
        }

        public void DisableInputs() {
            inputEnabled = new InputEnabled(false, false, false, false, false, false, this);
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.LeftAlt)) {
                foreach (int player in gameManager.playerDirectory.Keys) {
                    foreach (IUnitController unit in selectionManager.UnitsByPlayer[player]) {
                        unit.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Alt, true);
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftAlt)) {
                foreach (int player in gameManager.playerDirectory.Keys) {
                    foreach (IUnitController unit in selectionManager.UnitsByPlayer[player]) {
                        unit.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Alt, false);
                    }
                }
            }


            if (Input.GetMouseButtonUp(0)) {
                selectionManager.HideEnemyReachableHexes();
                if (hoveredUnit != null) {
                    hoveredUnit.SpriteManager.FullBackground();
                }
            }

            if (Input.GetMouseButtonDown(0)) {

                if (inputEnabled.hexClick && hoveredHex != null) {
                    hoveredHex.HexManager.FullBackground();
                }
                if (inputEnabled.unitClick && hoveredUnit != null) {
                    selectionManager.ShowEnemyReachableHexes(hoveredUnit);
                    hoveredUnit.SpriteManager.FaintBackground();
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                if (inputEnabled.hexHover && hoveredHex != null) {
                    hoveredHex.HexManager.FaintBackground();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                TurnButtonClicked();
            }
            if (Input.GetKeyDown(KeyCode.Backspace)) {
                UndoButtonClicked();
            }
        }

        public void LeftClickedAsButton() {

            if (inputEnabled.hexClick && hoveredHex != null) {
                if (permittedHexes == null || permittedHexes.Contains(hoveredHex)) {
                    selectionManager.TravelToHex(hoveredHex);
                }
            }
            if (inputEnabled.unitClick && hoveredUnit != null) {
                if (permittedUnits == null || permittedUnits.Contains(hoveredUnit)) {
                    selectionManager.SelectUnit(hoveredUnit);
                }
            }
        }

        public void RightClickedAsButton() {
            selectionManager.UnitSelected = null;
        }

        #region Hex and Unit Mouse Hover Callbacks
        public void HexMouseEnter(HexEntry hex) {

            hoveredHex = hex;

            if (inputEnabled.hexHover) {
                hex.HexManager.FaintBackground();
                selectionManager.ShowPathToHex(hex);
            }
        }

        public void HexMouseExit(HexEntry hex) {

            if (hoveredHex == hex) {
                hoveredHex = null;
            }

            hex.HexManager.FullBackground();

            if (inputEnabled.hexHover) {
                selectionManager.HidePathToHex(hex);
            }
        }

        public void UnitMouseEnter(IUnitController unitController) {

            hoveredUnit = unitController;

            if (inputEnabled.unitHover) {
                unitController.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Cursor, true);
            }
        }

        public void UnitMouseExit(IUnitController unitController) {

            if (hoveredUnit == unitController) {
                hoveredUnit = null;
            }

            unitController.SpriteManager.FullBackground();
            unitController.SpriteManager.ShowExtraInfo(UnitSpriteManager.UnitInfoDisplaySource.Cursor, false);
        }
        #endregion

        public void SlideCameraToHex(HexEntry hex) {
            cameraHandler.LerpToHexCoords(hex.BoardPos);
        }

        public void SlideCameraToHexAvg(List<HexEntry> hexes) {
            if (hexes.Count < 1) {
                Debug.Log("Camera can't jump to no hexes");
                return;
            }
            Vector2 averageSpot = Vector2.zero;
            foreach (HexEntry hex in hexes) {
                averageSpot += hex.BoardPos;
            }
            averageSpot /= hexes.Count;

            cameraHandler.LerpToHexCoords(averageSpot);
        }

        public void UndoButtonClicked() {
            if (inputEnabled.undoMove) {
                selectionManager.RevertBoardState();
            }
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);//Prevents button from remaining highlighted after use
        }

        public void TurnButtonClicked() {
            if (inputEnabled.TurnEnd) {
                selectionManager.EndTurn();
            }
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);//Prevents button from remaining highlighted after use
        }

        public void TurnButtonDisplay(int player) {
            
            UnityEngine.UI.ColorBlock colorBlock = UnityEngine.UI.ColorBlock.defaultColorBlock;
            colorBlock.normalColor = Config.Palette.PlayerColor(player);
            colorBlock.disabledColor = Config.Palette.PlayerColor(player);
            colorBlock.highlightedColor = new Color(.722f, .722f, .722f);
            colorBlock.pressedColor = new Color(.447f, .447f, .447f);
            playerTurnDisplay.colors = colorBlock;
            playerTurnDisplay.transform.FindChild("PlayerTurnText").GetComponent<UnityEngine.UI.Text>().text = "Player " + player + " Turn";

            undoButtonDisplay.colors = colorBlock;
        }

        public void UndoButtonText(string text) {
            ((UnityEngine.UI.Text)undoButtonDisplay.transform.GetComponentInChildren(typeof(UnityEngine.UI.Text))).text = text;
        }

        public void NotYourTurn() {
            popupPanel.color = Config.Palette.PlayerColor(gameManager.ActivePlayer); /* new Color(Config.Palette.PlayerColor(gameManager.ActivePlayer).r,
                Config.Palette.PlayerColor(gameManager.ActivePlayer).g, Config.Palette.PlayerColor(gameManager.ActivePlayer).b, 0.5f);*/
            popupPanel.transform.FindChild("PopupText").GetComponent<UnityEngine.UI.Text>().text = "Player " + gameManager.ActivePlayer + "'s Turn";

            popupPanel.gameObject.SetActive(true);

            DisableInputs();
        }

        public void NetTurnReady() {

            Debug.Log("Net Turn Ready says uiHandler");
            NotYourTurn();
            popupPanel.transform.FindChild("AcceptTurnButton").gameObject.SetActive(true);
        }

        public void PanelAcceptClicked() {

            popupPanel.transform.FindChild("AcceptTurnButton").gameObject.SetActive(false);
            HidePanel();
            selectionManager.PlayBackStoredTurns();
        }

        private void HidePanel() {

            popupPanel.gameObject.SetActive(false);
            EnableInputs();
        }

        public void DisplayVictory(int player) {

            popupPanel.color = Config.Palette.PlayerColor(player);
            popupPanel.transform.FindChild("PopupText").GetComponent<UnityEngine.UI.Text>().text = "Player " + player + " Victory!";

            popupPanel.gameObject.SetActive(true);

            DisableInputs();
        }
    }
}