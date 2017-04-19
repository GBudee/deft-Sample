using UnityEngine;

namespace Deft {
    static class Config {
        // Name of map to load for this skirmish
        public static string mapLocation = "Maps/";
        public static string defaultMapName = "tutorialMapA";

        // Play variables
        public static int turnsBeforePlayer1Wins = 9;

        // Camera variables
        public static int edgePanBoundary = 15;
        public static float screenScrollSpeed = 5;
        public static float hexToEdgeBuffer = .8f;
        public readonly static float cameraPosLerpRate = 1.5f;
        public readonly static float maxMouseOrCameraMovementInWorldForClick = .1f;

        // Offset of the hex grid from the scene's world space origin
        public readonly static Vector2 worldCoordOffsetOfGridZero = new Vector2(0, 0);

        // Magic number for converting from in-game canvas coordinates to world space coordinates
        public readonly static Vector2 worldToCanvasCoordScaler = new Vector2(1600.0f / 18.0f, 900.0f / 10.0f);

        // Space between hexes: 0.65f would be no space between hexes
        public static float hexSize = 0.66f;

        // Health bar size constants
        public readonly static int healthBarWidth = 6;
        public readonly static Vector3 healthBarOffsetWorldCoords = new Vector3(0, .6f, 0);
        public static bool alwaysShowHealthBars = false;

        // Hit arrow distance from hex center
        public readonly static float hitArrowPosScaler = .575f;

        // Movement and attack animation timers
        public readonly static float unitStepSpeed = 0.4f;//Time in seconds between unit animated steps
        public readonly static float attackStepSpeed = 0.4f;

        // Unit damage shake and attack bump animation constants
        public readonly static float baseShakeSize = .06f;
        public readonly static float baseShakeRate = 10;
        public readonly static float shakeTime = .2f;
        public readonly static float bumpTimer = .1f;
        public readonly static float bumpDistance = .2f;
        public readonly static float unitPosLerpRate = 20;

        // Unit Shoot animation constant
        public readonly static float shotAccel = 1.5f;
        public readonly static float shotDestroyRange = .2f;
        public readonly static float shotTimer = .4f;

        // Unit death animation constants
        public readonly static float flashFadeLerpRate = 1;
        public readonly static float flashFadeSlowLerpRate = .5f;
        public readonly static float flashFadeDestroyTimer = 1;

        // Palette
        public class Palette {
            
            public static Color background = new Color(.271f, .271f, .271f);
            private static Color playerOne = new Color(0, .5f, .5f);
            private static Color playerTwo = new Color(.835f, .557f, 0);
            public static Color PlayerColor(int player) {
                if (player == 1) {
                    return playerOne;
                }
                else { // if (player == 2) {
                    return playerTwo;
                }
            }
            public static void SetPlayerColor(int player, Color c) {
                if (player == 1) {
                    playerOne = c;
                }
                else {
                    playerTwo = c;
                }
            }

            public static Color attack = new Color(.682f, 0, 0);
            public static Color unitIconFill = Color.white;
            public static Color border = new Color(.278f, .278f, .278f);
            public static Color terrainIconFill = new Color(.235f, .235f, .235f);
            public static Color normalTerrain = new Color(.584f, .584f, .584f);
            public static Color roadTerrain = new Color(.584f, .584f, .584f);
            public static Color forestTerrain = new Color(.467f, .467f, .467f);
            public static Color hillsTerrain = new Color(.467f, .467f, .467f);
            public static Color wallTerrain = new Color(.467f, .467f, .467f);
            public static Color goalTerrain = new Color(.827f, .827f, .827f);
            public static Color pathArrow = Color.white;
            public static Color pathFootstep = Color.white;
            public static Color pushAttack = new Color(.9f, .9f, .9f);
            public static Color healthBar = Color.white;
            public static Color tooltipDark = new Color(.7f, .7f, .7f);
            public static Color tooltipColor = new Color(.8f, .8f, .8f);
            public static Color tooltipHighlight = new Color(.9f, .9f, .9f);
            public static Color hexReachableColor = new Color(1,1,1,.392f);
            public static Color hexHoverColor = new Color(1, 1, 1, .2f);
        }
    }
}