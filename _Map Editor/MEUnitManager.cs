using UnityEngine;
using System.Collections;

namespace MapEditor {
    public class MEUnitManager : MonoBehaviour {
        private bool initialized = false;

        MEInputManager meInputManager;
        SpriteRenderer background;
        public MEHexEntry myHex;

        private int _player;
        public int Player {
            get { return _player; }
            set {
                _player = value;

                if (initialized) {
                    background.color = Deft.Config.Palette.PlayerColor(_player);
                }
            }
        }

        void Start() {
            meInputManager = GameObject.Find("MEGameController").GetComponent<MEInputManager>();
            background = transform.FindChild("SpriteHolder").FindChild("Background").GetComponent<SpriteRenderer>();

            initialized = true;
            Player = _player;
        }

        void OnMouseOver() {
            if (Input.GetMouseButtonDown(1)) {
                meInputManager.OnUnitDown(myHex);
            }
        }
    }
}