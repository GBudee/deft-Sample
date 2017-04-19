using UnityEngine;
using System.Collections;

namespace WeaponSelect {
    public enum UnitOrWeapon { Unit, Weapon }
    public class WSMenuOption : MonoBehaviour {

        [SerializeField]
        public UnitOrWeapon optionType;
        [SerializeField]
        public Deft.UnitBaseType unit;
        [SerializeField]
        public Deft.WeaponType weapon;

        [SerializeField]
        GameObject menuOptionGhost;
        GameObject ghost;

        WSController controller;
        WSUIHandler uiHandler;

        SpriteRenderer unitOrWeapon;
        SpriteRenderer unitOrWeaponBorder;

        // Use this for initialization
        void Start() {
            controller = GameObject.Find("WSController").GetComponent<WSController>();
            uiHandler = controller.GetComponent<WSUIHandler>();
            unitOrWeapon = transform.FindChild("UnitOrWeapon").GetComponent<SpriteRenderer>();
            unitOrWeaponBorder = transform.FindChild("UnitOrWeaponBorder").GetComponent<SpriteRenderer>();
        }

        void OnMouseDown() {
            uiHandler.MenuMouseDown(this);
        }
        void OnMouseUpAsButton() {
            uiHandler.MenuMouseUpAsButton(this);
        }
        void OnMouseEnter() {
            uiHandler.MenuMouseEnter(this);
        }
        void OnMouseExit() {
            uiHandler.MenuMouseExit(this);
        }

        public void CreateOptionGhost() {
            // Generate a menuoptionghost to drag onto a unit
            ghost = Instantiate(menuOptionGhost, transform.position, Quaternion.identity);
            WSOptionGhost ghostScript = ghost.GetComponent<WSOptionGhost>();
            ghostScript.optionType = optionType;
            ghostScript.unit = unit;
            ghostScript.weapon = weapon;
            ghostScript.typeSprite = unitOrWeapon.sprite;
            ghostScript.typeSpriteBorder = unitOrWeaponBorder.sprite;
        }

        public void FaintBackground() {
            unitOrWeapon.color = new Color(unitOrWeapon.color.r, unitOrWeapon.color.g, unitOrWeapon.color.b, .7f);
        }

        public void FullBackground() {
            unitOrWeapon.color = new Color(unitOrWeapon.color.r, unitOrWeapon.color.g, unitOrWeapon.color.b, 1);
        }
    }
}