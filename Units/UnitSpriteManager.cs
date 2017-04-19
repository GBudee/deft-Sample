using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Deft {
    public class UnitSpriteManager : MonoBehaviour {
        private bool initialized = false;

        // Health bar prefab
        [SerializeField]
        GameObject healthBarPrefab;
        [SerializeField]
        GameObject animationArrowPrefab;
        [SerializeField]
        MovePointDisplayer movePointDisplayer;

        // Renderers and general visual output
        SpriteRenderer UnitBackground;
        SpriteRenderer BaseTypeSprite;
        SpriteRenderer BaseTypeBorder;
        SpriteRenderer WeaponSprite;
        SpriteRenderer WeaponSpriteBorder;
        SpriteRenderer Border;
        RectTransform HealthBar;
        RectTransform HealthFill;
        PathVisualizer pathVisualizer;

        // 3 primary properties of a unit
        private UnitBaseType _unitType;
        private WeaponType _weaponType;
        private int _playerOwner;

        private HexEntry _visiblePosition;

        // Info/hp display variables
        private int _maxHealthDisplay;
        private int _currentHealthDisplay;
        private int _visualMovesRemaining;
        private bool isThisUnitSelected;
        private bool animatingMovement;
        private bool visualizingCombat;
        private bool animatingCombat;
        private bool cursorHovered;
        private bool altButtonDown;

        private UIHandler uiHandler;
        private IUnitController _unitController; // Only present so the SpriteManager can tell the UIHandler who it is, SpriteManager should never tell unitController what to do
        public IUnitController UnitController {
            set { _unitController = value; }
        }

        // Perlin shake variables
        Transform spriteContainerTransform;
        float shakeSeed;
        float startShakeTime;
        float currentShakeSize;
        bool shaking;

        // Bump animation variables
        UnitSpriteManager bumpTarget;
        float startBumpTime;
        bool bumping;

        // Flash fade variables
        bool flashFadeActive;

        //Static sprite dictionaries
        private static IDictionary<UnitBaseType, Sprite> unitToSprite;
        private static IDictionary<UnitBaseType, Sprite> unitToSpriteBorder;
        private static IDictionary<WeaponType, Sprite> weaponToSprite;
        private static IDictionary<WeaponType, Sprite> weaponToSpriteBorder;

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
            uiHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIHandler>();
            spriteContainerTransform = transform.FindChild("SpriteHolder");
            UnitBackground = spriteContainerTransform.FindChild("Background").GetComponent<SpriteRenderer>();
            BaseTypeSprite = spriteContainerTransform.FindChild("Base Type").GetComponent<SpriteRenderer>();
            BaseTypeSprite.color = Config.Palette.unitIconFill;
            BaseTypeBorder = BaseTypeSprite.transform.FindChild("Base Type Border").GetComponent<SpriteRenderer>();
            BaseTypeBorder.color = Config.Palette.border;
            WeaponSprite = spriteContainerTransform.FindChild("Weapon").GetComponent<SpriteRenderer>();
            WeaponSprite.color = Config.Palette.unitIconFill;
            WeaponSpriteBorder = WeaponSprite.transform.FindChild("Weapon Border").GetComponent<SpriteRenderer>();
            WeaponSpriteBorder.color = Config.Palette.border;
            Border = spriteContainerTransform.FindChild("Border").GetComponent<SpriteRenderer>();
            Border.color = Config.Palette.border;
            pathVisualizer = uiHandler.GetComponent<PathVisualizer>();

            shaking = false;
            shakeSeed = Random.Range(0.0f, 20000.0f);// So that units do not shake identically

            flashFadeActive = false;
            isThisUnitSelected = false;
            visualizingCombat = false;
            animatingCombat = false;
            cursorHovered = false;
            altButtonDown = false;

            GameObject healthBarObject = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity);
            healthBarObject.transform.SetParent(GameObject.Find("WorldCanvas").transform);
            healthBarObject.transform.localScale = Vector3.one;
            HealthBar = healthBarObject.GetComponent<RectTransform>();
            HealthFill = HealthBar.FindChild("HealthFill").GetComponent<RectTransform>();

            initialized = true;
            UnitType = _unitType;
            WeaponType = _weaponType;
            PlayerOwner = _playerOwner;
            VisiblePosition = _visiblePosition;
            MaxHealthDisplay = _maxHealthDisplay;
            CurrentHealthDisplay = _currentHealthDisplay;
            ShowExtraInfo(UnitInfoDisplaySource.CombatAnimation, false);//Technically renders it true if AlwaysDisplayHealthBars is true
        }

        // Primary unit property setters, called automatically in the unitController
        public UnitBaseType UnitType {
            set {
                _unitType = value;
                if (initialized) {
                    BaseTypeSprite.sprite = unitToSprite[value];
                    BaseTypeBorder.sprite = unitToSpriteBorder[value];
                }
            }
        }

        public WeaponType WeaponType {
            set {
                _weaponType = value;
                if (initialized) {
                    WeaponSprite.sprite = weaponToSprite[value];
                    WeaponSpriteBorder.sprite = weaponToSpriteBorder[value];
                }
            }
        }

        public int PlayerOwner {
            set {
                _playerOwner = value;

                if (initialized) {
                    UnitBackground.color = Config.Palette.PlayerColor(value);
                }
            }
        }

        public HexEntry VisiblePosition {
            set {
                if (value != null) {
                    _visiblePosition = value;

                    transform.position = HexVectorUtil.worldPositionOfHexCoord(value.BoardPos);

                    if (initialized) {
                        HealthBar.position = transform.position + Config.healthBarOffsetWorldCoords;
                    }
                }
            }
        }

        #region Health Bar Length
        public int MaxHealthDisplay {
            set {
                _maxHealthDisplay = value;

                if (initialized) {
                    HealthBar.sizeDelta = new Vector2(Config.healthBarWidth * _maxHealthDisplay, 4);
                }
            }
        }

        public int CurrentHealthDisplay {
            set {
                _currentHealthDisplay = value;

                if (initialized) {
                    HealthFill.sizeDelta = new Vector2(Config.healthBarWidth * _currentHealthDisplay, 4);
                    HealthFill.anchoredPosition = new Vector2(Config.healthBarWidth * (_currentHealthDisplay - _maxHealthDisplay) / 2, 0);
                }
            }
        }
        #endregion

        public enum UnitInfoDisplaySource { Selection, Movement, CombatVisualization, CombatAnimation, Cursor, Alt, Death }
        public void ShowExtraInfo(UnitInfoDisplaySource source, bool show, HexEntry hex = null) {
            switch (source) {
                case UnitInfoDisplaySource.Selection:
                    isThisUnitSelected = show;
                    break;
                case UnitInfoDisplaySource.Movement:
                    animatingMovement = show;
                    break;
                case UnitInfoDisplaySource.CombatVisualization:
                    visualizingCombat = show;
                    break;
                case UnitInfoDisplaySource.CombatAnimation:
                    animatingCombat = show;
                    break;
                case UnitInfoDisplaySource.Cursor:
                    cursorHovered = show;
                    break;
                case UnitInfoDisplaySource.Alt:
                    altButtonDown = show;
                    break;
                case UnitInfoDisplaySource.Death:
                    HealthBar.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
                    HealthFill.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
                    pathVisualizer.ClearRangeIndicator(_unitController);
                    movePointDisplayer.gameObject.SetActive(false);
                    return;
            }

            // Show health bar
            if (cursorHovered || animatingCombat || altButtonDown || Config.alwaysShowHealthBars) {
                HealthBar.GetComponent<UnityEngine.UI.Image>().color = Config.Palette.healthBar;
                HealthFill.GetComponent<UnityEngine.UI.Image>().color = Config.Palette.attack;
            }
            else {
                HealthBar.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
                HealthFill.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
            }

            // Show range indicator
            pathVisualizer.ClearRangeIndicator(_unitController);
            if (cursorHovered || visualizingCombat || animatingCombat || altButtonDown || isThisUnitSelected) {
                if (hex == null) {
                    hex = _unitController.Position;
                }
                if (_unitController.WeaponType == WeaponType.Longbow || _unitController.WeaponType == WeaponType.Crossbow) {
                    pathVisualizer.ShowRangeIndicator(_unitController, hex, 3);
                }
            }

            // Show move points
            if (cursorHovered || animatingMovement || altButtonDown || isThisUnitSelected) {
                movePointDisplayer.gameObject.SetActive(true);
            }
            else {
                movePointDisplayer.gameObject.SetActive(false);
            }
        }

        public int VisualMovesRemaining {
            get { return _visualMovesRemaining; }
            set {
                _visualMovesRemaining = value;
                movePointDisplayer.SetPointDisplay(_visualMovesRemaining);
            }
        }

        public void SetBorderColor(Color color) {
            Border.color = color;
            BaseTypeBorder.color = color;
            WeaponSpriteBorder.color = color;
        }

        public void FaintBackground() {
            UnitBackground.color = new Color(UnitBackground.color.r, UnitBackground.color.g, UnitBackground.color.b, .5f);
        }
        public void FullBackground() {
            UnitBackground.color = new Color(UnitBackground.color.r, UnitBackground.color.g, UnitBackground.color.b, 1);
        }


        #region Animations Region (Shake and FlashFade)
        void Update() {
            // Shake effect && bump effect
            if (shaking) {
                spriteContainerTransform.localPosition = new Vector3(
                    (Mathf.PerlinNoise(Time.unscaledTime * Config.baseShakeRate + shakeSeed, 0.0f) - 0.5f) * currentShakeSize,
                    (Mathf.PerlinNoise(1.0f, Time.unscaledTime * Config.baseShakeRate + shakeSeed) - 0.5f) * currentShakeSize,
                    0);
                if ((Time.unscaledTime - startShakeTime) > Config.shakeTime) {
                    shaking = false;
                }
            }
            else if (bumping) {
                Vector3 bumpLocation = bumpTarget.transform.position - transform.position;
                bumpLocation = new Vector3(bumpLocation.x, bumpLocation.y, 0);
                bumpLocation.Normalize();
                bumpLocation = bumpLocation * Config.bumpDistance;

                spriteContainerTransform.localPosition =
                    Vector3.Lerp(spriteContainerTransform.localPosition, bumpLocation, Config.unitPosLerpRate * Time.unscaledDeltaTime);

                if (Time.unscaledTime - startBumpTime > Config.bumpTimer) {
                    bumping = false;
                }
            }
            else { // Return to position
                spriteContainerTransform.localPosition =
                    Vector3.Lerp(spriteContainerTransform.localPosition, Vector3.zero, Config.unitPosLerpRate * Time.unscaledDeltaTime);
            }

            // Flash fade effect
            if (flashFadeActive) {
                UnitBackground.color = new Color(UnitBackground.color.r, UnitBackground.color.g, UnitBackground.color.b,
                    Mathf.Lerp(UnitBackground.color.a, 0, Config.flashFadeLerpRate * Time.unscaledDeltaTime));
                WeaponSprite.color = new Color(WeaponSprite.color.r, WeaponSprite.color.g, WeaponSprite.color.b,
                    Mathf.Lerp(WeaponSprite.color.a, 0, Config.flashFadeSlowLerpRate * Time.unscaledDeltaTime));
                BaseTypeSprite.color = new Color(BaseTypeSprite.color.r, BaseTypeSprite.color.g, BaseTypeSprite.color.b,
                    Mathf.Lerp(BaseTypeSprite.color.a, 0, Config.flashFadeSlowLerpRate * Time.unscaledDeltaTime));
            }
        }

        // Damage Receiving Animation
        public void Shake(float sizeModifier) {
            if (!shaking || (currentShakeSize > Config.baseShakeSize * sizeModifier)) {
                currentShakeSize = Config.baseShakeSize * sizeModifier;
                shaking = true;
                startShakeTime = Time.unscaledTime;
            }
        }

        // Bump attack animation
        public void Bump(UnitSpriteManager target) {
            startBumpTime = Time.unscaledTime;
            bumpTarget = target;
            bumping = true;
        }

        // Death Animation
        public void FlashFade(Color color) {
            UnitBackground.color = color;
            Border.color = Color.clear;
            BaseTypeBorder.color = Color.clear;
            WeaponSpriteBorder.color = Color.clear;
            flashFadeActive = true;
        }

        // Ranged Attack Animation
        public void ShootArrow(UnitSpriteManager target) {
            GameObject arrow = (GameObject)Instantiate(animationArrowPrefab, transform.position, Quaternion.identity);
            arrow.GetComponent<ShotController>().target = target;
        }
        #endregion

        // Remove all visual trace of unit
        public void DisableUnit() {/*
            UnitBackground.color = Color.clear;
            Border.color = Color.clear;
            BaseTypeSprite.color = Color.clear;
            BaseTypeBorder.color = Color.clear;
            WeaponSprite.color = Color.clear;
            WeaponSpriteBorder.color = Color.clear;
            GetComponent<PolygonCollider2D>().enabled = false;*/

            gameObject.SetActive(false); // Easiest way to disable visuals, duh
        }

        // A.k.a. EnableUnit()
        public void RevertUnitVisuals() {
            UnitBackground.color = Config.Palette.PlayerColor(_unitController.PlayerOwner);

            SetBorderColor(Config.Palette.border);
            BaseTypeSprite.color = Config.Palette.unitIconFill;
            WeaponSprite.color = Config.Palette.unitIconFill;
            GetComponent<PolygonCollider2D>().enabled = true;

            shaking = false;
            spriteContainerTransform.localPosition = Vector3.zero;

            flashFadeActive = false;
            visualizingCombat = false;
            animatingCombat = false;

            gameObject.SetActive(true);
        }

        public void DestroyUnit() { // With extreme prejudice
            pathVisualizer.ClearRangeIndicator(_unitController);
            Destroy(HealthBar.gameObject);
            Destroy(this.gameObject);
        }

        #region UIHandler Callbacks
        //Callback to uiHandler so it can know about unit clicks
        void OnMouseEnter() {
            if (uiHandler != null) {
                uiHandler.UnitMouseEnter(_unitController);
            }
        }
        void OnMouseExit() {
            if (uiHandler != null) {
                uiHandler.UnitMouseExit(_unitController);
            }
        }
        #endregion
    }
}
