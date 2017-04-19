using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Deft;
using System.Collections.Generic;

namespace Tutorial {
    public class TutorialManager : MonoBehaviour {

        private GameManager gameManager;
        private ScenarioLoader scenarioLoader;
        private UIHandler uiHandler;
        private SelectionManager selectionManager;

        //Standard UI to be revealed
        [SerializeField]
        GameObject playerTurnButton;
        [SerializeField]
        GameObject undoButton;

        //Chapter select ui
        [SerializeField]
        GameObject settingsButton;
        [SerializeField]
        GameObject settingsPanel;
        [SerializeField]
        GameObject quitButton;

        //Panel Section
        [SerializeField]
        Image tooltipPanel1; // Page 1, movement
        [SerializeField]
        Image tooltipPanel2;
        [SerializeField]
        Image tooltipPanel3;
        [SerializeField]
        Image tooltipPanel3_1;
        [SerializeField]
        Image tooltipPanel4;
        [SerializeField]
        Image tooltipPanel5;
        [SerializeField]
        Image attackExplanationPanel; // Page 2, attacks
        [SerializeField]
        Image tooltipPanel6;
        [SerializeField]
        Image attackPatternPanel1;
        [SerializeField]
        Image tooltipPanel7;
        [SerializeField]
        Image tooltipPanel8;
        [SerializeField]
        Image tooltipPanel9;
        [SerializeField]
        Image attackPatternPanel2;
        [SerializeField]
        Image tooltipPanel10;
        [SerializeField]
        Image tooltipPanel11;
        [SerializeField]
        Image tooltipPanel12;
        [SerializeField]
        Image retalExplanationPanel;
        [SerializeField]
        Image tooltipPanel13;
        [SerializeField]
        Image tooltipPanel14;
        [SerializeField]
        Image tooltipPanel15;
        [SerializeField]
        Image tooltipPanel16;
        [SerializeField]
        Image tooltipPanel17;
        [SerializeField]
        Image attackPatternPanel3;
        [SerializeField]
        Image tooltipPanel18;
        [SerializeField]
        Image tooltipPanel19;
        [SerializeField]
        Image tooltipPanel20;
        [SerializeField]
        Image supportExplanationPanel;
        [SerializeField]
        Image tooltipPanel21;
        [SerializeField]
        Image tooltipPanel22;
        [SerializeField]
        Image tooltipPanel23;
        [SerializeField]
        Image tooltipPanel24;
        [SerializeField]
        Image pushExplanationPanel;
        [SerializeField]
        Image tooltipPanel25;
        [SerializeField]
        Image attackPatternPanel4;
        [SerializeField]
        Image tooltipPanel26;
        [SerializeField]
        Image terrainExplanationPanel;
        [SerializeField]
        Image tooltipPanel27;
        [SerializeField]
        Image tooltipPanel28;
        [SerializeField]
        Image tooltipPanel29;
        [SerializeField]
        Image tooltipPanel30;
        [SerializeField]
        Image tooltipPanelWebGLTerminus;

        //Alerts and effects
        [SerializeField]
        GameObject leftClickAlert;

        public int activeSection;
        private IUnitController unitOfInterest;
        private HexEntry hexOfInterest;

        GameObject alertObject;

        public void Initialize() {

            gameManager = GetComponent<GameManager>();
            scenarioLoader = GetComponent<ScenarioLoader>();
            uiHandler = GetComponent<UIHandler>();
            selectionManager = GetComponent<SelectionManager>();

            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                quitButton.SetActive(false);
                settingsButton.transform.position = quitButton.transform.position;
            }

            uiHandler.DisableInputs();
            
            activeSection = 0;

            AdvanceSection();
        }

        void Update() {

            // Conditionals which advance the section
            if (activeSection == 1) {
                if (selectionManager.UnitSelected == unitOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 2) {
                if (uiHandler.hoveredHex == hexOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 3) {
                if (unitOfInterest.Position == hexOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 4) {
                if (unitOfInterest.Position == hexOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 5) {
                if (selectionManager.turnsOfPlay == 1) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 8) {
                if (selectionManager.UnitSelected == unitOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 9) {
                if (unitOfInterest.Position == hexOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 12) {
                if (uiHandler.permittedHexes.Contains(unitOfInterest.Position)) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 14) {
                if (selectionManager.UnitSelected == unitOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 15) {
                if (unitOfInterest.HP == 8) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 18) {
                if (unitOfInterest.HP == 12) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 19) {
                if (selectionManager.UnitsByPlayer[1].Count == 1 && selectionManager.UnitsByPlayer[2].Count == 1) {
                    if (selectionManager.UnitsByPlayer[1][0].HP == 12 && selectionManager.UnitsByPlayer[2][0].HP == 8) {
                        AdvanceSection();
                    }
                }
            }
            else if (activeSection == 21) {
                if (selectionManager.UnitSelected == unitOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 22) {
                if (unitOfInterest.Position == hexOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 23) {
                if (selectionManager.UnitSelected == unitOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 24) {
                if (unitOfInterest.Position == hexOfInterest) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 26) {
                if (selectionManager.UnitsByPlayer[1].Count == 2) {
                    if (selectionManager.UnitsByPlayer[1][0].HP == 12 && selectionManager.UnitsByPlayer[1][1].HP == 12 && unitOfInterest.HP <= 0) {
                        AdvanceSection();
                    }
                }
            }
            else if (activeSection == 29) {
                if (unitOfInterest.HP == 4) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 31) {
                if (unitOfInterest.HP <= 0) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 34) {
                if (unitOfInterest.HP < 12) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 37) {
                if (unitOfInterest.HP < 12) {
                    AdvanceSection();
                }
            }
            else if (activeSection == 39) {
                if (unitOfInterest.Position == null) {
                    AdvanceSection();
                }
            }
        }

        public void AdvanceSection() {
            activeSection++;
            EnterSection();
        }

        private void EnterSection() {
            // Movement explanation page
            if (activeSection == 1) { // Chapter 1
                tooltipPanel1.gameObject.SetActive(true);
                
                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;
                uiHandler.inputEnabled.undoMove = false; // Not necessary,
                uiHandler.inputEnabled.TurnEnd = false;  // present for readability

                uiHandler.permittedHexes = null;
                uiHandler.permittedUnits = null;

                List<WeaponSelect.WSController.WSUnitDescriptor> startingUnits = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                startingUnits.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(3, -3), UnitBaseType.Simple, WeaponType.Spear, 1));
                startingUnits.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(-3, 1), UnitBaseType.Simple, WeaponType.None, 2));
                gameManager.ReloadUnits(startingUnits);
                unitOfInterest = selectionManager.UnitsByPlayer[1][0];

                DirectPlayerAttention(unitOfInterest.Position, UIAlertType.LeftClick);
            }
            else if (activeSection == 2) {
                tooltipPanel1.gameObject.SetActive(false);
                tooltipPanel2.gameObject.SetActive(true);

                hexOfInterest = scenarioLoader.HexGrid[new Vector2(1, -1)];
                uiHandler.permittedHexes = new List<HexEntry>();
                uiHandler.permittedHexes.Add(hexOfInterest);

                Destroy(alertObject);
                DirectPlayerAttention(hexOfInterest, UIAlertType.Hover);
            }
            else if (activeSection == 3) {
                tooltipPanel3.gameObject.SetActive(true);
                tooltipPanel2.color = Config.Palette.tooltipDark;
                
                Destroy(alertObject);
                DirectPlayerAttention(hexOfInterest, UIAlertType.LeftClick);
            }
            else if (activeSection == 4) {
                hexOfInterest = scenarioLoader.HexGrid[new Vector2(0, 1)];
                uiHandler.permittedHexes = new List<HexEntry>();
                uiHandler.permittedHexes.Add(hexOfInterest);

                tooltipPanel3_1.gameObject.SetActive(true);
                tooltipPanel3.color = Config.Palette.tooltipDark;

                Destroy(alertObject);
                DirectPlayerAttention(hexOfInterest, UIAlertType.LeftClick);
            }
            else if (activeSection == 5) {
                tooltipPanel4.gameObject.SetActive(true);
                tooltipPanel3_1.color = Config.Palette.tooltipDark;

                playerTurnButton.SetActive(true);
                uiHandler.inputEnabled.TurnEnd = true;

                Destroy(alertObject);
                DirectPlayerAttention(new Vector2(.78f, 4.75f), UIAlertType.LeftClick); // Over the player turn button
            }
            else if (activeSection == 6) {
                tooltipPanel2.gameObject.SetActive(false);
                tooltipPanel3.gameObject.SetActive(false);
                tooltipPanel3_1.gameObject.SetActive(false);
                tooltipPanel4.gameObject.SetActive(false);
                tooltipPanel5.gameObject.SetActive(true);

                uiHandler.inputEnabled.hexClick = false;
                uiHandler.inputEnabled.unitClick = false;
                uiHandler.inputEnabled.TurnEnd = false;

                selectionManager.turnsOfPlay = 0;

                Destroy(alertObject);
            }
            // Attack explanation page
            else if (activeSection == 7) { // Chapter 2
                tooltipPanel5.gameObject.SetActive(false);
                attackExplanationPanel.gameObject.SetActive(true);

                uiHandler.DisableInputs();

                uiHandler.permittedHexes = null;
                uiHandler.permittedUnits = null;

                List<WeaponSelect.WSController.WSUnitDescriptor> startingUnits = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                startingUnits.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 1), UnitBaseType.Simple, WeaponType.Spear, 1));
                startingUnits.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, -1), UnitBaseType.Simple, WeaponType.None, 2));
                gameManager.ReloadUnits(startingUnits);
                unitOfInterest = selectionManager.UnitsByPlayer[1][0];
            }
            else if (activeSection == 8) {
                attackExplanationPanel.gameObject.SetActive(false);
                tooltipPanel6.gameObject.SetActive(true);
                attackPatternPanel1.gameObject.SetActive(true);

                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;

                hexOfInterest = scenarioLoader.HexGrid[new Vector2(0, 0)];
                uiHandler.permittedHexes = new List<HexEntry>();
                uiHandler.permittedHexes.Add(hexOfInterest);

                DirectPlayerAttention(unitOfInterest.Position, UIAlertType.LeftClick);
            }
            else if (activeSection == 9) {
                Destroy(alertObject);
                DirectPlayerAttention(hexOfInterest, UIAlertType.LeftClick);
            }
            else if (activeSection == 10) {
                tooltipPanel7.gameObject.SetActive(true);
                tooltipPanel6.color = Config.Palette.tooltipDark;

                Destroy(alertObject);
            }
            else if (activeSection == 11) {
                tooltipPanel8.gameObject.SetActive(true);
                tooltipPanel7.color = Config.Palette.tooltipDark;
            }
            else if (activeSection == 12) {
                attackPatternPanel2.gameObject.SetActive(true);
                tooltipPanel9.gameObject.SetActive(true);
                tooltipPanel8.color = Config.Palette.tooltipDark;

                uiHandler.permittedHexes = new List<HexEntry>();
                uiHandler.permittedHexes.Add(scenarioLoader.HexGrid[new Vector2(-1, 0)]);
                uiHandler.permittedHexes.Add(scenarioLoader.HexGrid[new Vector2(1, -1)]);

                unitOfInterest.WeaponType = WeaponType.Sword;
                unitOfInterest.Weapon = new Sword(unitOfInterest, gameManager, scenarioLoader, selectionManager);
                selectionManager.UnitSelected = selectionManager.UnitSelected;
            }
            else if (activeSection == 13) {
                tooltipPanel6.gameObject.SetActive(false);
                tooltipPanel7.gameObject.SetActive(false);
                tooltipPanel8.gameObject.SetActive(false);
                tooltipPanel9.gameObject.SetActive(false);
                tooltipPanel10.gameObject.SetActive(true);

                uiHandler.permittedHexes = null;
            }
            else if (activeSection == 14) { // Give enemy spear -- Chapter 3
                tooltipPanel10.gameObject.SetActive(false);
                tooltipPanel11.gameObject.SetActive(true);
                attackPatternPanel1.gameObject.SetActive(true);
                attackPatternPanel2.gameObject.SetActive(true);

                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;
                uiHandler.inputEnabled.undoMove = false;
                uiHandler.inputEnabled.TurnEnd = false;

                List<WeaponSelect.WSController.WSUnitDescriptor> spearvsSword = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                spearvsSword.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 1), UnitBaseType.Simple, WeaponType.Sword, 1));
                spearvsSword.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, -1), UnitBaseType.Simple, WeaponType.Spear, 2));
                gameManager.ReloadUnits(spearvsSword);
                unitOfInterest = selectionManager.UnitsByPlayer[1][0];
                //selectionManager.UnitSelected = unitOfInterest; -- Doesn't work: newly created units require a frame to set up
                
                hexOfInterest = scenarioLoader.HexGrid[new Vector2(0, 0)];
                uiHandler.permittedHexes = new List<HexEntry>();
                uiHandler.permittedHexes.Add(hexOfInterest);
                uiHandler.permittedUnits = null;

                DirectPlayerAttention(unitOfInterest.Position, UIAlertType.LeftClick);
            }
            else if (activeSection == 15) {
                Destroy(alertObject);
                DirectPlayerAttention(hexOfInterest, UIAlertType.LeftClick);
            }
            else if (activeSection == 16) {
                tooltipPanel12.gameObject.SetActive(true);
                tooltipPanel11.color = Config.Palette.tooltipDark;

                Destroy(alertObject);
            }
            else if (activeSection == 17) {
                tooltipPanel11.gameObject.SetActive(false);
                tooltipPanel12.gameObject.SetActive(false);
                attackPatternPanel1.gameObject.SetActive(false);
                attackPatternPanel2.gameObject.SetActive(false);
                retalExplanationPanel.gameObject.SetActive(true);

                uiHandler.DisableInputs();
            }
            else if (activeSection == 18) {
                retalExplanationPanel.gameObject.SetActive(false);
                attackPatternPanel1.gameObject.SetActive(true);
                attackPatternPanel2.gameObject.SetActive(true);
                tooltipPanel13.gameObject.SetActive(true);
                undoButton.SetActive(true);
                
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;
                uiHandler.inputEnabled.undoMove = true;

                // Show alert over undo button (resolution-agnostic)
                Rect undoButtonRect = undoButton.GetComponent<RectTransform>().rect;
                Vector2 positionOfUndoButton = undoButton.GetComponent<RectTransform>().TransformPoint(new Vector3(undoButtonRect.center.x - 30, undoButtonRect.center.y));
                DirectPlayerAttention(positionOfUndoButton, UIAlertType.FlippedX);
            }
            else if (activeSection == 19) {
                tooltipPanel14.gameObject.SetActive(true);
                tooltipPanel13.color = Config.Palette.tooltipDark;

                uiHandler.permittedHexes = null; // All hexes now permitted
                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;

                Destroy(alertObject);
            }
            else if (activeSection == 20) {
                tooltipPanel15.gameObject.SetActive(true);
                tooltipPanel14.color = Config.Palette.tooltipDark;
            }
            //Flail intro
            else if (activeSection == 21) { // Chapter 4
                tooltipPanel13.gameObject.SetActive(false);
                tooltipPanel14.gameObject.SetActive(false);
                tooltipPanel15.gameObject.SetActive(false);
                tooltipPanel16.gameObject.SetActive(true);
                attackPatternPanel1.gameObject.SetActive(true);
                attackPatternPanel2.gameObject.SetActive(true);
                attackPatternPanel3.gameObject.SetActive(true);

                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;
                uiHandler.inputEnabled.undoMove = false;
                uiHandler.inputEnabled.TurnEnd = false;

                //uiHandler.inputEnabled.undoMove = false;

                List<WeaponSelect.WSController.WSUnitDescriptor> flailvsAlly = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                flailvsAlly.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 1), UnitBaseType.Simple, WeaponType.Flail, 1));
                flailvsAlly.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(-2, 2), UnitBaseType.Simple, WeaponType.Spear, 1));
                flailvsAlly.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(1, -1), UnitBaseType.Simple, WeaponType.None, 2));
                gameManager.ReloadUnits(flailvsAlly);

                unitOfInterest = selectionManager.UnitsByPlayer[1][1];
                uiHandler.permittedUnits = new List<IUnitController>();
                uiHandler.permittedUnits.Add(unitOfInterest);

                hexOfInterest = scenarioLoader.HexGrid[new Vector2(0, -2)];
                uiHandler.permittedHexes = new List<HexEntry>();
                uiHandler.permittedHexes.Add(hexOfInterest);

                DirectPlayerAttention(unitOfInterest.Position, UIAlertType.LeftClick);
            }
            else if (activeSection == 22) {
                Destroy(alertObject);
                DirectPlayerAttention(hexOfInterest, UIAlertType.LeftClick);
            }
            else if (activeSection == 23) {
                tooltipPanel17.gameObject.SetActive(true);
                tooltipPanel16.color = Config.Palette.tooltipDark;

                unitOfInterest = selectionManager.UnitsByPlayer[1][0];
                uiHandler.permittedUnits = new List<IUnitController>();
                uiHandler.permittedUnits.Add(unitOfInterest);
                selectionManager.UnitSelected = null;

                hexOfInterest = scenarioLoader.HexGrid[new Vector2(0, -1)];
                uiHandler.permittedHexes = new List<HexEntry>();
                uiHandler.permittedHexes.Add(hexOfInterest);

                Destroy(alertObject);
                DirectPlayerAttention(unitOfInterest.Position, UIAlertType.LeftClick);
            }
            else if (activeSection == 24) {
                Destroy(alertObject);
                DirectPlayerAttention(hexOfInterest, UIAlertType.FlippedX);
            }
            else if (activeSection == 25) {
                tooltipPanel18.gameObject.SetActive(true);
                tooltipPanel17.color = Config.Palette.tooltipDark;

                uiHandler.inputEnabled.hexHover = false;
                uiHandler.inputEnabled.hexClick = false;

                Destroy(alertObject);
            }
            //Flail challenge
            else if (activeSection == 26) {
                tooltipPanel19.gameObject.SetActive(true);
                tooltipPanel18.color = Config.Palette.tooltipDark;

                uiHandler.permittedHexes = null;
                uiHandler.permittedUnits = null;
                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.undoMove = true;

                List<WeaponSelect.WSController.WSUnitDescriptor> flailChallenge = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                flailChallenge.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 1), UnitBaseType.Simple, WeaponType.Spear, 1));
                flailChallenge.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(2, -1), UnitBaseType.Simple, WeaponType.Flail, 1));
                flailChallenge.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, -1), UnitBaseType.Simple, WeaponType.Sword, 2));
                gameManager.ReloadUnits(flailChallenge);
                unitOfInterest = selectionManager.UnitsByPlayer[2][0];
            }
            else if (activeSection == 27) {
                tooltipPanel20.gameObject.SetActive(true);
                tooltipPanel19.color = Config.Palette.tooltipDark;
            }
            // Support explanation
            else if (activeSection == 28) { // Chapter 5
                attackPatternPanel1.gameObject.SetActive(false);
                attackPatternPanel2.gameObject.SetActive(false);
                attackPatternPanel3.gameObject.SetActive(false);
                tooltipPanel16.gameObject.SetActive(false);
                tooltipPanel17.gameObject.SetActive(false);
                tooltipPanel18.gameObject.SetActive(false);
                tooltipPanel19.gameObject.SetActive(false);
                tooltipPanel20.gameObject.SetActive(false);
                supportExplanationPanel.gameObject.SetActive(true);

                uiHandler.DisableInputs();
                uiHandler.permittedHexes = null;
                uiHandler.permittedUnits = null;

                List<WeaponSelect.WSController.WSUnitDescriptor> longbowDemo = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                longbowDemo.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(2, -1), UnitBaseType.Simple, WeaponType.Spear, 1));
                longbowDemo.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(-3, 2), UnitBaseType.Simple, WeaponType.Longbow, 1));
                longbowDemo.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, -1), UnitBaseType.Simple, WeaponType.Sword, 2));
                gameManager.ReloadUnits(longbowDemo);
                unitOfInterest = selectionManager.UnitsByPlayer[2][0];
            }
            else if (activeSection == 29) {
                supportExplanationPanel.gameObject.SetActive(false);
                attackPatternPanel1.gameObject.SetActive(true);
                attackPatternPanel2.gameObject.SetActive(true);
                attackPatternPanel3.gameObject.SetActive(true);
                tooltipPanel21.gameObject.SetActive(true);

                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;
                uiHandler.inputEnabled.undoMove = true;
            }
            else if (activeSection == 30) {
                tooltipPanel22.gameObject.SetActive(true);
                tooltipPanel21.color = Config.Palette.tooltipDark;
            }
            else if (activeSection == 31) {
                tooltipPanel23.gameObject.SetActive(true);
                tooltipPanel22.color = Config.Palette.tooltipDark;

                List<WeaponSelect.WSController.WSUnitDescriptor> flailFighters = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                flailFighters.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(2, -1), UnitBaseType.Simple, WeaponType.Spear, 1));
                flailFighters.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 3), UnitBaseType.Simple, WeaponType.Longbow, 1));
                flailFighters.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 1), UnitBaseType.Simple, WeaponType.Dagger, 1));
                flailFighters.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, -1), UnitBaseType.Simple, WeaponType.Flail, 2));
                gameManager.ReloadUnits(flailFighters);
                unitOfInterest = selectionManager.UnitsByPlayer[2][0];
            }
            else if (activeSection == 32) {
                tooltipPanel24.gameObject.SetActive(true);
                tooltipPanel23.color = Config.Palette.tooltipDark;
            }
            // Push explanation
            else if (activeSection == 33) { // Chapter 6
                attackPatternPanel1.gameObject.SetActive(false);
                attackPatternPanel2.gameObject.SetActive(false);
                attackPatternPanel3.gameObject.SetActive(false);
                tooltipPanel21.gameObject.SetActive(false);
                tooltipPanel22.gameObject.SetActive(false);
                tooltipPanel23.gameObject.SetActive(false);
                tooltipPanel24.gameObject.SetActive(false);
                pushExplanationPanel.gameObject.SetActive(true);

                uiHandler.DisableInputs();
                uiHandler.permittedHexes = null;
                uiHandler.permittedUnits = null;
            }
            else if (activeSection == 34) {
                pushExplanationPanel.gameObject.SetActive(false);
                attackPatternPanel1.gameObject.SetActive(true);
                attackPatternPanel2.gameObject.SetActive(true);
                attackPatternPanel3.gameObject.SetActive(true);
                attackPatternPanel4.gameObject.SetActive(true);
                tooltipPanel25.gameObject.SetActive(true);

                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;
                uiHandler.inputEnabled.undoMove = true;

                List<WeaponSelect.WSController.WSUnitDescriptor> shieldPushSimple = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                shieldPushSimple.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(-3, 3), UnitBaseType.Simple, WeaponType.Shield, 1));
                shieldPushSimple.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, -3), UnitBaseType.Simple, WeaponType.Spear, 2));
                shieldPushSimple.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, -1), UnitBaseType.Simple, WeaponType.Flail, 2));
                gameManager.ReloadUnits(shieldPushSimple);
                unitOfInterest = selectionManager.UnitsByPlayer[2][0];
            }
            else if (activeSection == 35) {
                tooltipPanel26.gameObject.SetActive(true);
                tooltipPanel25.color = Config.Palette.tooltipDark;
            }
            else if (activeSection == 36) {
                tooltipPanel21.gameObject.SetActive(false);
                tooltipPanel22.gameObject.SetActive(false);
                tooltipPanel23.gameObject.SetActive(false);
                tooltipPanel24.gameObject.SetActive(false);
                tooltipPanel25.gameObject.SetActive(false);
                tooltipPanel26.gameObject.SetActive(false);
                terrainExplanationPanel.gameObject.SetActive(true);

                uiHandler.DisableInputs();
            }
            else if (activeSection == 37) {
                terrainExplanationPanel.gameObject.SetActive(false);
                attackPatternPanel1.gameObject.SetActive(true);
                attackPatternPanel2.gameObject.SetActive(true);
                attackPatternPanel3.gameObject.SetActive(true);
                attackPatternPanel4.gameObject.SetActive(true);
                tooltipPanel27.gameObject.SetActive(true);

                uiHandler.inputEnabled.hexHover = true;
                uiHandler.inputEnabled.hexClick = true;
                uiHandler.inputEnabled.unitHover = true;
                uiHandler.inputEnabled.unitClick = true;
                uiHandler.inputEnabled.undoMove = true;

                List<WeaponSelect.WSController.WSUnitDescriptor> shieldPusher = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                shieldPusher.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(2, -2), UnitBaseType.Simple, WeaponType.Spear, 1));
                shieldPusher.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 0), UnitBaseType.Simple, WeaponType.Shield, 2));
                gameManager.ReloadUnits(shieldPusher);
                unitOfInterest = selectionManager.UnitsByPlayer[2][0];

                scenarioLoader.HexGrid[new Vector2(-2, 2)].Terrain = Deft.Terrain.Forest;
            }
            else if (activeSection == 38) {
                tooltipPanel28.gameObject.SetActive(true);
                tooltipPanel27.color = Config.Palette.tooltipDark;
            }
            else if (activeSection == 39) {
                tooltipPanel27.gameObject.SetActive(false);
                tooltipPanel28.gameObject.SetActive(false);
                tooltipPanel29.gameObject.SetActive(true);

                List<WeaponSelect.WSController.WSUnitDescriptor> shieldPusherSupreme = new List<WeaponSelect.WSController.WSUnitDescriptor>();
                shieldPusherSupreme.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 2), UnitBaseType.Simple, WeaponType.Shield, 1));
                shieldPusherSupreme.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(0, 1), UnitBaseType.Simple, WeaponType.Sword, 1));
                shieldPusherSupreme.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(1, -1), UnitBaseType.Simple, WeaponType.Shield, 2));
                shieldPusherSupreme.Add(new WeaponSelect.WSController.WSUnitDescriptor(new Vector2(-1, 0), UnitBaseType.Simple, WeaponType.Spear, 2));
                gameManager.ReloadUnits(shieldPusherSupreme);
                unitOfInterest = selectionManager.UnitsByPlayer[2][0];

                scenarioLoader.HexGrid[new Vector2(-2, 2)].Terrain = Deft.Terrain.Normal;
                scenarioLoader.HexGrid[new Vector2(1, -2)].Terrain = Deft.Terrain.Forest;
                scenarioLoader.HexGrid[new Vector2(1, 0)].Terrain = Deft.Terrain.Pit;
            }
            else if (activeSection == 40) {
                if (Application.platform != RuntimePlatform.WebGLPlayer) {
                    tooltipPanel30.gameObject.SetActive(true);
                }
                else {
                    tooltipPanelWebGLTerminus.gameObject.SetActive(true);
                }

                tooltipPanel29.color = Config.Palette.tooltipDark;
            }
        }

        private void ExitChapter() {
            if (alertObject != null) {
                Destroy(alertObject);
            }

            attackPatternPanel1.gameObject.SetActive(false);
            attackPatternPanel2.gameObject.SetActive(false);
            attackPatternPanel3.gameObject.SetActive(false);
            attackPatternPanel4.gameObject.SetActive(false);

            if (1 <= activeSection && activeSection < 7) { // Chapter 1
                tooltipPanel1.gameObject.SetActive(false);
                tooltipPanel2.gameObject.SetActive(false);
                tooltipPanel3.gameObject.SetActive(false);
                tooltipPanel3_1.gameObject.SetActive(false);
                tooltipPanel4.gameObject.SetActive(false);
                tooltipPanel5.gameObject.SetActive(false);

                tooltipPanel1.color = Config.Palette.tooltipColor;
                tooltipPanel2.color = Config.Palette.tooltipColor;
                tooltipPanel3.color = Config.Palette.tooltipColor;
                tooltipPanel3_1.color = Config.Palette.tooltipColor;
                tooltipPanel4.color = Config.Palette.tooltipColor;
                tooltipPanel5.color = Config.Palette.tooltipColor;

                selectionManager.turnsOfPlay = 0;
            }
            if (7 <= activeSection && activeSection < 14) { // Chapter 2
                tooltipPanel6.gameObject.SetActive(false);
                tooltipPanel7.gameObject.SetActive(false);
                tooltipPanel8.gameObject.SetActive(false);
                tooltipPanel9.gameObject.SetActive(false);
                tooltipPanel10.gameObject.SetActive(false);

                tooltipPanel6.color = Config.Palette.tooltipColor;
                tooltipPanel7.color = Config.Palette.tooltipColor;
                tooltipPanel8.color = Config.Palette.tooltipColor;
                tooltipPanel9.color = Config.Palette.tooltipColor;
                tooltipPanel10.color = Config.Palette.tooltipColor;

                attackExplanationPanel.gameObject.SetActive(false);
            }
            if (14 <= activeSection && activeSection < 21) { // Chapter 3
                tooltipPanel11.gameObject.SetActive(false);
                tooltipPanel12.gameObject.SetActive(false);
                tooltipPanel13.gameObject.SetActive(false);
                tooltipPanel14.gameObject.SetActive(false);
                tooltipPanel15.gameObject.SetActive(false);

                tooltipPanel11.color = Config.Palette.tooltipColor;
                tooltipPanel12.color = Config.Palette.tooltipColor;
                tooltipPanel13.color = Config.Palette.tooltipColor;
                tooltipPanel14.color = Config.Palette.tooltipColor;
                tooltipPanel15.color = Config.Palette.tooltipColor;

                retalExplanationPanel.gameObject.SetActive(false);
            }
            if (21 <= activeSection && activeSection < 28) { // Chapter 4
                tooltipPanel16.gameObject.SetActive(false);
                tooltipPanel17.gameObject.SetActive(false);
                tooltipPanel18.gameObject.SetActive(false);
                tooltipPanel19.gameObject.SetActive(false);
                tooltipPanel20.gameObject.SetActive(false);

                tooltipPanel16.color = Config.Palette.tooltipColor;
                tooltipPanel17.color = Config.Palette.tooltipColor;
                tooltipPanel18.color = Config.Palette.tooltipColor;
                tooltipPanel19.color = Config.Palette.tooltipColor;
                tooltipPanel20.color = Config.Palette.tooltipColor;
            }
            if (28 <= activeSection && activeSection < 33) { // Chapter 5
                tooltipPanel21.gameObject.SetActive(false);
                tooltipPanel22.gameObject.SetActive(false);
                tooltipPanel23.gameObject.SetActive(false);
                tooltipPanel24.gameObject.SetActive(false);

                tooltipPanel21.color = Config.Palette.tooltipColor;
                tooltipPanel22.color = Config.Palette.tooltipColor;
                tooltipPanel23.color = Config.Palette.tooltipColor;
                tooltipPanel24.color = Config.Palette.tooltipColor;

                supportExplanationPanel.gameObject.SetActive(false);
            }
            if (33 <= activeSection) { // Chapter 6
                tooltipPanel25.gameObject.SetActive(false);
                tooltipPanel26.gameObject.SetActive(false);
                tooltipPanel27.gameObject.SetActive(false);
                tooltipPanel28.gameObject.SetActive(false);
                tooltipPanel29.gameObject.SetActive(false);
                tooltipPanel30.gameObject.SetActive(false);

                tooltipPanel25.color = Config.Palette.tooltipColor;
                tooltipPanel26.color = Config.Palette.tooltipColor;
                tooltipPanel27.color = Config.Palette.tooltipColor;
                tooltipPanel28.color = Config.Palette.tooltipColor;
                tooltipPanel29.color = Config.Palette.tooltipColor;
                tooltipPanel30.color = Config.Palette.tooltipColor;

                pushExplanationPanel.gameObject.SetActive(false);
                terrainExplanationPanel.gameObject.SetActive(false);

                scenarioLoader.HexGrid[new Vector2(-2, 2)].Terrain = Deft.Terrain.Normal;
                scenarioLoader.HexGrid[new Vector2(1, -2)].Terrain = Deft.Terrain.Normal;
                scenarioLoader.HexGrid[new Vector2(1, 0)].Terrain = Deft.Terrain.Normal;
            }
        }

        public void ChapterSelect(int chapter) {
            ExitChapter();

            if (chapter == 1) {
                activeSection = 1;
            }
            if (chapter == 2) {
                activeSection = 7;
            }
            if (chapter == 3) {
                activeSection = 14;
            }
            if (chapter == 4) {
                activeSection = 21;
            }
            if (chapter == 5) {
                activeSection = 28;
            }
            if (chapter == 6) {
                activeSection = 33;
            }

            EnterSection();
        }

        private enum UIAlertType { LeftClick, Hover, FlippedX }
        private void DirectPlayerAttention(HexEntry hex, UIAlertType alertType) {
            Vector2 targetPos = HexVectorUtil.worldPositionOfHexCoord(hex.BoardPos);
            alertObject = Instantiate(leftClickAlert, new Vector2(targetPos.x + (alertType==UIAlertType.FlippedX ? -0.14f : 0.14f), targetPos.y - 0.14f), Quaternion.identity);

            if (alertType == UIAlertType.Hover) {
                alertObject.transform.GetChild(1).gameObject.SetActive(false);
            }
            if (alertType == UIAlertType.FlippedX) {
                alertObject.transform.localScale = new Vector3(-alertObject.transform.localScale.x,
                    alertObject.transform.localScale.y, alertObject.transform.localScale.z);
            }
        }
        private void DirectPlayerAttention(Vector2 worldPos, UIAlertType alertType) {

            alertObject = Instantiate(leftClickAlert, worldPos, Quaternion.identity);

            if (alertType == UIAlertType.Hover) {
                alertObject.transform.GetChild(1).gameObject.SetActive(false);
            }
            if (alertType == UIAlertType.FlippedX) {
                alertObject.transform.localScale = new Vector3(-alertObject.transform.localScale.x,
                    alertObject.transform.localScale.y, alertObject.transform.localScale.z);
            }
        }

        public void AfterTutorialButton() {
            Deft.Vaults.playerDirectory = new Dictionary<int, Deft.PlayerType>();
            Deft.Vaults.playerDirectory.Add(1, Deft.PlayerType.Local);
            Deft.Vaults.playerDirectory.Add(2, Deft.PlayerType.AI);
            Deft.Vaults.mapName = "invertedMapBig";
            Deft.Vaults.wsUnitList = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Outfitting");
        }

        public void ContinueButton() {
            AdvanceSection();
        }
        
        public void OpenChapterSelect() {
            settingsButton.SetActive(false);
            settingsPanel.SetActive(true);

            if (alertObject != null) {
                alertObject.SetActive(false);
            }
        }
        public void CloseChapterSelect() {
            settingsButton.SetActive(true);
            settingsPanel.SetActive(false);

            if (alertObject != null) {
                alertObject.SetActive(true);
            }
        }
    }
}

