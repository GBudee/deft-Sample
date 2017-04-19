using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Deft;

namespace WeaponSelect {
    public class WSUnitManager : MonoBehaviour {
        private bool initialized = false;

        [SerializeField]
        GameObject unitGhost;

        // Renderers and general visual output
        SpriteRenderer UnitBackground;
        SpriteRenderer UnitBorder;
        SpriteRenderer BaseTypeSprite;
        SpriteRenderer BaseTypeBorder;
        SpriteRenderer WeaponSprite;
        SpriteRenderer WeaponSpriteBorder;
        Transform spriteContainerTransform;

        // 3 primary properties of a unit
        private UnitBaseType _unitType;
        private WeaponType _weaponType;
        private int _playerOwner;

        private Vector2 _visiblePosition;
        private bool _litBorder;
        private bool _active;

        WSUIHandler uiHandler;

        //Static sprite dictionaries
        public static IDictionary<UnitBaseType, Sprite> unitToSprite;
        public static IDictionary<UnitBaseType, Sprite> unitToSpriteBorder;
        public static IDictionary<WeaponType, Sprite> weaponToSprite;
        public static IDictionary<WeaponType, Sprite> weaponToSpriteBorder;

        #region Sprite Dictionary Setup Region, includes Awake() function
        // Unit base type sprites
        [SerializeField]
        Sprite Simple;
        [SerializeField]
        Sprite SimpleBorder;
        [SerializeField]
        Sprite Swift;
        [SerializeField]
        Sprite SwiftBorder;
        [SerializeField]
        Sprite Sturdy;
        [SerializeField]
        Sprite SturdyBorder;
        /*
        [SerializeField]
        Sprite Cavalry;
        [SerializeField]
        Sprite Monk;
        [SerializeField]
        Sprite Nightwing;
        [SerializeField]
        Sprite Ghost;
        [SerializeField]
        Sprite Flameshade;
        [SerializeField]
        Sprite Telekine;*/

        // Weapon sprites
        [SerializeField]
        Sprite NoWeapon;
        [SerializeField]
        Sprite Quarterstaff;
        [SerializeField]
        Sprite QuarterstaffBorder;
        [SerializeField]
        Sprite Spear;
        [SerializeField]
        Sprite SpearBorder;
        [SerializeField]
        Sprite Sword;
        [SerializeField]
        Sprite SwordBorder;
        [SerializeField]
        Sprite Axe;
        [SerializeField]
        Sprite AxeBorder;
        [SerializeField]
        Sprite Flail;
        [SerializeField]
        Sprite FlailBorder;
        [SerializeField]
        Sprite Dagger;
        [SerializeField]
        Sprite DaggerBorder;
        [SerializeField]
        Sprite Longbow;
        [SerializeField]
        Sprite LongbowBorder;
        [SerializeField]
        Sprite Crossbow;
        [SerializeField]
        Sprite CrossbowBorder;
        [SerializeField]
        Sprite Shield;
        [SerializeField]
        Sprite ShieldBorder;

        void Awake() {
            // Only happens once, generates static members
            if (unitToSprite == null) {
                unitToSprite = new Dictionary<UnitBaseType, Sprite>();
                unitToSpriteBorder = new Dictionary<UnitBaseType, Sprite>();
                weaponToSprite = new Dictionary<WeaponType, Sprite>();
                weaponToSpriteBorder = new Dictionary<WeaponType, Sprite>();

                unitToSprite.Add(UnitBaseType.Simple, Simple);
                unitToSprite.Add(UnitBaseType.Swift, Swift);
                unitToSprite.Add(UnitBaseType.Sturdy, Sturdy);
                /*
                unitToSprite.Add(UnitBaseType.Cavalry, Cavalry);
                unitToSprite.Add(UnitBaseType.Monk, Monk);
                unitToSprite.Add(UnitBaseType.Nightwing, Nightwing);
                unitToSprite.Add(UnitBaseType.Ghost, Ghost);
                unitToSprite.Add(UnitBaseType.Flameshade, Flameshade);
                unitToSprite.Add(UnitBaseType.Telekine, Telekine);*/

                unitToSpriteBorder.Add(UnitBaseType.Simple, SimpleBorder);
                unitToSpriteBorder.Add(UnitBaseType.Swift, SwiftBorder);
                unitToSpriteBorder.Add(UnitBaseType.Sturdy, SturdyBorder);

                weaponToSprite.Add(WeaponType.None, NoWeapon);
                weaponToSprite.Add(WeaponType.Quarterstaff, Quarterstaff);
                weaponToSprite.Add(WeaponType.Spear, Spear);
                weaponToSprite.Add(WeaponType.Sword, Sword);
                weaponToSprite.Add(WeaponType.Axe, Axe);
                weaponToSprite.Add(WeaponType.Flail, Flail);
                weaponToSprite.Add(WeaponType.Dagger, Dagger);
                weaponToSprite.Add(WeaponType.Longbow, Longbow);
                weaponToSprite.Add(WeaponType.Crossbow, Crossbow);
                weaponToSprite.Add(WeaponType.Shield, Shield);

                weaponToSpriteBorder.Add(WeaponType.None, NoWeapon);
                weaponToSpriteBorder.Add(WeaponType.Quarterstaff, QuarterstaffBorder);
                weaponToSpriteBorder.Add(WeaponType.Spear, SpearBorder);
                weaponToSpriteBorder.Add(WeaponType.Sword, SwordBorder);
                weaponToSpriteBorder.Add(WeaponType.Axe, AxeBorder);
                weaponToSpriteBorder.Add(WeaponType.Flail, FlailBorder);
                weaponToSpriteBorder.Add(WeaponType.Dagger, DaggerBorder);
                weaponToSpriteBorder.Add(WeaponType.Longbow, LongbowBorder);
                weaponToSpriteBorder.Add(WeaponType.Crossbow, CrossbowBorder);
                weaponToSpriteBorder.Add(WeaponType.Shield, ShieldBorder);
            }
        }
        #endregion

        void Start() {
            uiHandler = GameObject.Find("WSController").GetComponent<WSUIHandler>();
            spriteContainerTransform = transform.FindChild("SpriteHolder");
            UnitBackground = spriteContainerTransform.FindChild("Background").GetComponent<SpriteRenderer>();
            UnitBorder = spriteContainerTransform.FindChild("Border").GetComponent<SpriteRenderer>();
            UnitBorder.color = Config.Palette.border;
            BaseTypeSprite = spriteContainerTransform.FindChild("Base Type").GetComponent<SpriteRenderer>();
            BaseTypeSprite.color = Config.Palette.unitIconFill;
            BaseTypeBorder = BaseTypeSprite.transform.FindChild("Base Type Border").GetComponent<SpriteRenderer>();
            BaseTypeBorder.color = Config.Palette.border;
            WeaponSprite = spriteContainerTransform.FindChild("Weapon").GetComponent<SpriteRenderer>();
            WeaponSprite.color = Config.Palette.unitIconFill;
            WeaponSpriteBorder = WeaponSprite.transform.FindChild("Weapon Border").GetComponent<SpriteRenderer>();
            WeaponSpriteBorder.color = Config.Palette.border;

            initialized = true;
            UnitType = _unitType;
            WeaponType = _weaponType;
            PlayerOwner = _playerOwner;
            VisiblePosition = _visiblePosition;
            Active = _active;
            LitBorder = _litBorder;
        }

        // Primary unit property setters, called automatically in the unitController
        public UnitBaseType UnitType {
            get { return _unitType; }
            set {
                _unitType = value;
                if (initialized) {
                    BaseTypeSprite.sprite = unitToSprite[value];
                    BaseTypeBorder.sprite = unitToSpriteBorder[value];
                }
            }
        }

        public WeaponType WeaponType {
            get { return _weaponType; }
            set {
                _weaponType = value;
                if (initialized) {
                    WeaponSprite.sprite = weaponToSprite[value];
                    WeaponSpriteBorder.sprite = weaponToSpriteBorder[value];
                }
            }
        }

        public int PlayerOwner {
            get { return _playerOwner; }
            set {
                _playerOwner = value;
                if (initialized) {
                    UnitBackground.color = Config.Palette.PlayerColor(value);
                }
            }
        }

        public Vector2 VisiblePosition {
            get { return _visiblePosition; }
            set {
                _visiblePosition = value;

                transform.position = HexVectorUtil.worldPositionOfHexCoord(value);
            }
        }

        public bool Active {
            get { return _active; }
            set {
                _active = value;
                if (initialized) {
                    BaseTypeSprite.enabled = value;
                    BaseTypeBorder.enabled = value;
                    WeaponSprite.enabled = value;
                    WeaponSpriteBorder.enabled = value;
                }
            }
        }

        public void FaintBackground() {
            UnitBackground.color = new Color(UnitBackground.color.r, UnitBackground.color.g, UnitBackground.color.b, .5f);
        }

        public void FullBackground() {
            UnitBackground.color = new Color(UnitBackground.color.r, UnitBackground.color.g, UnitBackground.color.b, 1f);
        }

        #region Mouse Callbacks
        void OnMouseEnter() {
            if (!_active) {
                return;
            }

            uiHandler.UnitMouseEnter(this);
        }

        void OnMouseDown() {
            if (!_active) {
                return;
            }

            uiHandler.UnitMouseDown(this);
        }

        void OnMouseUpAsButton() {
            if (!_active) {
                return;
            }

            uiHandler.UnitMouseUpAsButton(this);
        }

        void OnMouseExit() {
            if (!_active) {
                return;
            }

            uiHandler.UnitMouseExit(this);
        }
        #endregion

        public void CreateUnitGhost() {
            GameObject ghost = Instantiate(unitGhost, transform.position, Quaternion.identity);
            WSUnitGhost ghostScript = ghost.GetComponent<WSUnitGhost>();
            ghostScript.unit = this;
            ghostScript.unitTypeSprite = BaseTypeSprite.sprite;
            ghostScript.unitSpriteBorder = BaseTypeBorder.sprite;
            ghostScript.weaponSprite = WeaponSprite.sprite;
            ghostScript.weaponSpriteBorder = WeaponSpriteBorder.sprite;
            spriteContainerTransform.gameObject.SetActive(false);
        }

        public void EnableUnitVisual() {
            spriteContainerTransform.gameObject.SetActive(true);
        }

        public bool LitBorder {
            get { return _litBorder; }
            set {
                _litBorder = value;
                if (initialized) {
                    if (_litBorder) {
                        UnitBorder.color = Config.Palette.unitIconFill;
                    }
                    else {
                        UnitBorder.color = Config.Palette.border;
                    }
                }
            }
        }
    }
}
