using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {
    class UnitFactory : MonoBehaviour {
        [SerializeField]
        GameObject unitPrefab;

        static GameManager gameManager;
        static ScenarioLoader scenarioLoader;
        static SelectionManager selectionManager;
        static UIHandler uiHandler;

        public IUnitController Create(int playerOwner, UnitBaseType type, WeaponType weapon, HexEntry startPosition) {

            if (gameManager == null) {
                gameManager = GetComponent<GameManager>();
                scenarioLoader = GetComponent<ScenarioLoader>();
                selectionManager = GetComponent<SelectionManager>();
                uiHandler = GetComponent<UIHandler>();
            }

            GameObject unit = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity);

            IUnitController unitController = new DefaultUnitController(type,weapon,playerOwner);
            UnitSpriteManager unitSpriteManager = unit.GetComponent<UnitSpriteManager>();
            unitSpriteManager.UnitController = unitController;
            IMover mover = new DefaultMover(unitController, gameManager, selectionManager, scenarioLoader);
            OutcomeAnimator outcomeAnimator = new OutcomeAnimator(selectionManager, uiHandler);
            unitController.Initialize(mover, outcomeAnimator, unitSpriteManager, startPosition);
            
            switch (weapon) {
                case WeaponType.None:
                    unitController.Weapon = new EmptyWeapon(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Spear:
                    unitController.Weapon = new Spear(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Sword:
                    unitController.Weapon = new Sword(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Axe:
                    unitController.Weapon = new Axe(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Quarterstaff:
                    unitController.Weapon = new Quarterstaff(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Flail:
                    unitController.Weapon = new Flail(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Dagger:
                    unitController.Weapon = new Dagger(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Longbow:
                    unitController.Weapon = new Longbow(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Crossbow:
                    unitController.Weapon = new Crossbow(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
                case WeaponType.Shield:
                    unitController.Weapon = new Shield(unitController, gameManager, scenarioLoader, selectionManager);
                    break;
            }

            return unitController;
        }
    }
}
