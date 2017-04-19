using UnityEngine;
using System.Collections;

namespace WeaponSelect {
    public class WSUnitGhost : MonoBehaviour {
        
        public WSUnitManager unit;

        public Sprite unitTypeSprite;
        public Sprite unitSpriteBorder;
        public Sprite weaponSprite;
        public Sprite weaponSpriteBorder;

        WSController controller;
        Camera mainCamera;

        void Start() {
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            controller = GameObject.Find("WSController").GetComponent<WSController>();
            transform.FindChild("UnitTypeSprite").GetComponent<SpriteRenderer>().sprite = unitTypeSprite;
            transform.FindChild("UnitSpriteBorder").GetComponent<SpriteRenderer>().sprite = unitSpriteBorder;
            transform.FindChild("WeaponSprite").GetComponent<SpriteRenderer>().sprite = weaponSprite;
            transform.FindChild("WeaponSpriteBorder").GetComponent<SpriteRenderer>().sprite = weaponSpriteBorder;
        }

        void Update() {
            if (Input.GetMouseButtonUp(0)) {
                controller.UnitGhostReleased(unit);
                Destroy(gameObject);
            }

            transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}