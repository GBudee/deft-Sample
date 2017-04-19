using UnityEngine;
using System.Collections;

namespace WeaponSelect {
    public class WSOptionGhost : MonoBehaviour {
        
        public UnitOrWeapon optionType;
        public Deft.UnitBaseType unit;
        public Deft.WeaponType weapon;

        public Sprite typeSprite;
        public Sprite typeSpriteBorder;

        WSController controller;
        Camera mainCamera;

        void Start() {
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            controller = GameObject.Find("WSController").GetComponent<WSController>();
            transform.FindChild("TypeSprite").GetComponent<SpriteRenderer>().sprite = typeSprite;
            transform.FindChild("TypeSpriteBorder").GetComponent<SpriteRenderer>().sprite = typeSpriteBorder;
        }

        void Update() {
            if (Input.GetMouseButtonUp(0)) {
                controller.OptionGhostReleased(optionType, unit, weapon);
                Destroy(gameObject);
            }
            
            transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}