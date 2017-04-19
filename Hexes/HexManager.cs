using UnityEngine;
using System.Collections.Generic;


// Attached to every Hexagon prefab
namespace Deft {

    public class HexManager : MonoBehaviour {

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

        UIHandler uiHandler;
        SpriteRenderer backgroundSprite;
        SpriteRenderer borderSprite;
        SpriteRenderer terrainSprite;
        SpriteRenderer selectionHighlightSprite;
        SpriteRenderer enemyHighlightSprite;
        SpriteRenderer hoverHighlightSprite;
        HexEntry hexEntry;

        private Terrain _terrain;

        private bool initialized = false;

        private static IDictionary<Terrain, TerrainVisuals> terrainToSprite;
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
                terrainToSprite = new Dictionary<Terrain, TerrainVisuals>();

                terrainToSprite.Add(Terrain.Normal, new TerrainVisuals(Config.Palette.normalTerrain, NormalTerrain));
                terrainToSprite.Add(Terrain.Road, new TerrainVisuals(Config.Palette.roadTerrain, NormalTerrain));
                terrainToSprite.Add(Terrain.Forest, new TerrainVisuals(Config.Palette.forestTerrain, ForestTerrain));
                terrainToSprite.Add(Terrain.Hills, new TerrainVisuals(Config.Palette.hillsTerrain, HillsTerrain));
                terrainToSprite.Add(Terrain.Pit, new TerrainVisuals(Config.Palette.wallTerrain, PitTerrain));
                terrainToSprite.Add(Terrain.Wall, new TerrainVisuals(Config.Palette.wallTerrain, WallTerrain));
                terrainToSprite.Add(Terrain.Goal, new TerrainVisuals(Config.Palette.goalTerrain, GoalTerrain));
            }
        }

        void Start() {
            uiHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIHandler>();
            backgroundSprite = GetComponent<SpriteRenderer>();
            borderSprite = transform.FindChild("Hex Border").GetComponent<SpriteRenderer>();
            terrainSprite = transform.FindChild("Terrain Sprite").GetComponent<SpriteRenderer>();
            selectionHighlightSprite = transform.FindChild("Selection Highlight").GetComponent<SpriteRenderer>();
            enemyHighlightSprite = transform.FindChild("Enemy Highlight").GetComponent<SpriteRenderer>();
            hoverHighlightSprite = transform.FindChild("Hover Highlight").GetComponent<SpriteRenderer>();

            initialized = true;
            TerrainSprite = _terrain;
        }

        public HexEntry HexEntry {
            set { hexEntry = value; }
        }

        public Terrain TerrainSprite {
            set {
                _terrain = value;
                if (initialized) {
                    backgroundSprite.color = terrainToSprite[value].c;
                    terrainSprite.sprite = terrainToSprite[value].s;
                    terrainSprite.color = Config.Palette.terrainIconFill;
                }
            }
        }

        // Exposed values
        public Color BackgroundColor {
            get { return backgroundSprite.color; }
            set { backgroundSprite.color = value; }
        }

        public Color BorderColor {
            get { return borderSprite.color; }
            set {
                borderSprite.color = value;
                if (value == Config.Palette.attack)
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                if (value == Config.Palette.border)
                    transform.position = new Vector3(transform.position.x, transform.position.y, 1);
            }
        }

        public Color SelectionHighlightColor {
            get { return selectionHighlightSprite.color; }
            set { selectionHighlightSprite.color = value; }
        }

        public Color EnemyHighlightColor {
            get { return enemyHighlightSprite.color; }
            set { enemyHighlightSprite.color = value; }
        }

        public void FaintBackground() {
            //backgroundSprite.color = new Color(backgroundSprite.color.r, backgroundSprite.color.g, backgroundSprite.color.b, 0.5f);
            hoverHighlightSprite.color = Config.Palette.hexHoverColor;
        }

        public void FullBackground() {
            //backgroundSprite.color = new Color(backgroundSprite.color.r, backgroundSprite.color.g, backgroundSprite.color.b, 1);
            hoverHighlightSprite.color = Color.clear;
        }

        // uiHandler callbacks
        void OnMouseEnter() {
            if (uiHandler == null) {
                Debug.Log("uiHandler not yet initialized for mouse callback on hex");
                return;
            }
            uiHandler.HexMouseEnter(hexEntry);
        }

        void OnMouseExit() {
            if (uiHandler == null) {
                Debug.Log("uiHandler not yet initialized for mouse callback on hex");
                return;
            }
            uiHandler.HexMouseExit(hexEntry);
        }
    }
}