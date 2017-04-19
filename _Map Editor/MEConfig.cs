using UnityEngine;
using System.Collections;

namespace MapEditor {
    public class MEConfig {
        
        // Dimensions of the hex grid
        public static int gridRectWidth = 14;
        public static int gridRectHeight = 19;

        // Terrain enum stuff
        public readonly static int maxTerrainValue = 6;

        // Where to put the maps
        public static string mapLocation = "Assets/Resources/Maps/";
        public static string mapName = "exampleHexMap";
        public static string unitMapExtension = "_Units";
        public static string mapFileType = ".csv";

        public static int edgePanBoundary = 50;
        public static float screenScrollSpeed = 5;
        public static float hexToEdgeBound = 1;
    }
}
