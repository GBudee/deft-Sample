using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Deft;

namespace MapEditor {
    public class MEHexManager : MonoBehaviour {


        // Terrain Sprites
        [SerializeField]
        Sprite NormalTerrain;
        [SerializeField]
        Sprite RoadTerrain;
        [SerializeField]
        Sprite ForestTerrain;
        [SerializeField]
        Sprite HillsTerrain;
        [SerializeField]
        Sprite PitTerrain;
        [SerializeField]
        Sprite WallTerrain;
        [SerializeField]
        Sprite GoalTerrain;

        MEInputManager inputManager;
        SpriteRenderer backgroundSprite;
        SpriteRenderer borderSprite;
        SpriteRenderer terrainSprite;
        SpriteRenderer highlightSprite;
        MEHexEntry hexEntry;

        private Deft.Terrain _terrain;

        private bool initialized = false;

        private static IDictionary<Deft.Terrain, TerrainVisuals> terrainToSprite;
        class TerrainVisuals {
            public Color c;
            public Sprite s;

            public TerrainVisuals(Color c, Sprite s) {
                this.c = c;
                this.s = s;
            }
        }

        void Awake() {

            // Only happens once, generates static members
            if (terrainToSprite == null) {
                terrainToSprite = new Dictionary<Deft.Terrain, TerrainVisuals>();

                terrainToSprite.Add(Deft.Terrain.Normal, new TerrainVisuals(Config.Palette.normalTerrain, NormalTerrain));
                terrainToSprite.Add(Deft.Terrain.Road, new TerrainVisuals(Config.Palette.roadTerrain, NormalTerrain));
                terrainToSprite.Add(Deft.Terrain.Forest, new TerrainVisuals(Config.Palette.forestTerrain, ForestTerrain));
                terrainToSprite.Add(Deft.Terrain.Hills, new TerrainVisuals(Config.Palette.hillsTerrain, HillsTerrain));
                terrainToSprite.Add(Deft.Terrain.Pit, new TerrainVisuals(Config.Palette.wallTerrain, PitTerrain));
                terrainToSprite.Add(Deft.Terrain.Wall, new TerrainVisuals(Config.Palette.wallTerrain, WallTerrain));
                terrainToSprite.Add(Deft.Terrain.Goal, new TerrainVisuals(Config.Palette.goalTerrain, GoalTerrain));
            }
        }

        void Start() {
            inputManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MEInputManager>();
            backgroundSprite = GetComponent<SpriteRenderer>();
            borderSprite = transform.FindChild("Hex Border").GetComponent<SpriteRenderer>();
            terrainSprite = transform.FindChild("Terrain Sprite").GetComponent<SpriteRenderer>();
            highlightSprite = transform.FindChild("Highlight").GetComponent<SpriteRenderer>();

            initialized = true;
            TerrainSprite = _terrain;
        }

        public MEHexEntry HexEntry {
            set { hexEntry = value; }
        }

        public Deft.Terrain TerrainSprite {
            set {
                _terrain = value;
                if (initialized) {
                    backgroundSprite.color = terrainToSprite[value].c;
                    terrainSprite.sprite = terrainToSprite[value].s;
                }
            }
        }

        // Exposed functions
        public Color BackgroundColor {
            get { return backgroundSprite.color; }
            set { backgroundSprite.color = value; }
        }

        public Color BorderColor {
            get { return borderSprite.color; }
            set {
                borderSprite.color = value;
                if (value == Config.Palette.attack)
                    transform.position -= Vector3.forward;
                if (value == Config.Palette.border)
                    transform.position += Vector3.forward;
            }
        }

        public Color HighlightColor {
            get { return highlightSprite.color; }
            set { highlightSprite.color = value; }
        }

        // Mouse-related visual behaviors, plus InputManager callback on click
        void OnMouseDown() {
            inputManager.OnHexDown(hexEntry);

            backgroundSprite.color = new Color(backgroundSprite.color.r, backgroundSprite.color.g, backgroundSprite.color.b, 1);
        }

        void OnMouseUpAsButton() {
            inputManager.OnHexClick(hexEntry);

            backgroundSprite.color = new Color(backgroundSprite.color.r, backgroundSprite.color.g, backgroundSprite.color.b, 0.5f);
        }

        void OnMouseEnter() {
            inputManager.OnHexMouseEnter(hexEntry);

            backgroundSprite.color = new Color(backgroundSprite.color.r, backgroundSprite.color.g, backgroundSprite.color.b, 0.5f);
        }

        void OnMouseExit() {
            backgroundSprite.color = new Color(backgroundSprite.color.r, backgroundSprite.color.g, backgroundSprite.color.b, 1);
        }

        void OnMouseOver() {
            if (Input.GetMouseButtonDown(1)) {
                inputManager.OnUnitDown(hexEntry);
            }
            if (Input.GetMouseButton(4)) {
                inputManager.OnHexMouseFour(hexEntry);
            }
        }
    }
}