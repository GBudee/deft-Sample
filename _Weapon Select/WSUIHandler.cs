using UnityEngine;
using System.Collections;
using Deft;
using System.Linq;
using System;

namespace WeaponSelect {
    public class WSUIHandler : MonoBehaviour, IUIHandler {

        WSController wsController;
        CameraHandler cameraHandler;

        //<Camera vars>
        private GameObject mainCamera;
        private int screenWidth;
        private int screenHeight;

        private float cameraXLeftBound;
        private float cameraXRightBound;
        private float cameraYBottomBound;
        private float cameraYTopBound;
        //</Camera vars>

        public bool inputEnabled;

        private WSUnitManager hoveredUnit;
        private WSUnitManager unitBeingDragged;

        private WSMenuOption clickedOnThisMenuOption;
        private WSMenuOption menuBeingDragged;

        public void Initialize() {

            wsController = GetComponent<WSController>();
            cameraHandler = GetComponent<CameraHandler>();

            inputEnabled = true;

            cameraHandler.Initialize(wsController.HexGrid.Values
                .Select(s => new Vector2(s.transform.position.x, s.transform.position.y)).ToList(), new Vector2(0, 0), new Vector2(1, .8f));
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                TurnButtonClicked();
            }
        }

        public void TurnButtonClicked() {
            if (!inputEnabled) {
                return;
            }

            wsController.EndTurn();
        }

        public void CenterCameraOnUnits(int player) {

            Vector2 averageSpot = Vector2.zero;
            foreach (WSUnitManager unit in wsController.Units[player]) {
                averageSpot += unit.VisiblePosition;
            }
            averageSpot /= wsController.Units[player].Count;

            cameraHandler.SnapToHexCoords(averageSpot);
        }

        #region Unit Mouse Callbacks
        public void UnitMouseEnter(WSUnitManager unit) {
            if (!inputEnabled) {
                return;
            }

            unit.FaintBackground();
            wsController.UnitHover(unit);
        }

        public void UnitMouseExit(WSUnitManager unit) {
            unit.FullBackground();

            if (!inputEnabled) {
                return;
            }

            wsController.UnitEndHover(unit);

            if (Input.GetMouseButton(0) && unitBeingDragged == unit) {
                unit.CreateUnitGhost();
            }
            unitBeingDragged = null;
        }

        public void UnitMouseDown(WSUnitManager unit) {
            unit.FullBackground();

            if (!inputEnabled) {
                return;
            }

            unitBeingDragged = unit;
            cameraHandler.dragPanEnabled = false;
        }

        public void UnitMouseUpAsButton(WSUnitManager unit) {
            if (!inputEnabled) {
                return;
            }

            unit.FaintBackground();

            wsController.SelectUnit(unit);
            unitBeingDragged = null;
        }
        #endregion

        #region Menu Mouse Callbacks
        public void MenuMouseEnter(WSMenuOption option) {
            if (!inputEnabled) {
                return;
            }

            option.FaintBackground();

            wsController.MenuOptionHover(option.optionType, option.unit, option.weapon);
        }

        public void MenuMouseExit(WSMenuOption option) {
            option.FullBackground();

            wsController.MenuOptionEndHover();

            if (!inputEnabled) {
                return;
            }

            if (Input.GetMouseButton(0) && option == clickedOnThisMenuOption) {
                //if (GameObject.Find("WSOptionGhost(Clone)") == null) { // If there are currently no other optionghosts
                    option.CreateOptionGhost();
                //}
            }
            clickedOnThisMenuOption = null;
        }

        public void MenuMouseDown(WSMenuOption option) {
            if (!inputEnabled) {
                return;
            }

            clickedOnThisMenuOption = option;

            option.FullBackground();
        }

        public void MenuMouseUpAsButton(WSMenuOption option) {
            if (!inputEnabled) {
                return;
            }

            option.FaintBackground();

            wsController.MenuOptionClicked(option.optionType, option.unit, option.weapon);
        }
        #endregion
        
        public void LeftClickedAsButton() {
            // Not used currently
        }

        public void RightClickedAsButton() {
            wsController.OptionGhostReleased(UnitOrWeapon.Weapon, UnitBaseType.Simple, WeaponType.None);
        }
    }
}