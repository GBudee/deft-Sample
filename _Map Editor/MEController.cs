using UnityEngine;
using System.Collections;
using Deft;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MapEditor {
    public class MEController : MonoBehaviour {

        [SerializeField]
        GameObject meHex;
        [SerializeField]
        GameObject unit;

        public IDictionary<Vector2, MEHexEntry> HexGrid = new Dictionary<Vector2, MEHexEntry>();

        Deft.Terrain extensionTerrainType;

        // Use this for initialization
        void Start() {
            if (System.IO.File.Exists(MEConfig.mapLocation + MEConfig.mapName + MEConfig.mapFileType)) {
                LoadGrid();
            }
            else {
                CreateGrid();
            }

            if (System.IO.File.Exists(MEConfig.mapLocation + MEConfig.mapName + MEConfig.unitMapExtension + MEConfig.mapFileType)) {
                LoadUnitPositions();
            }
        }

        private void CreateGrid() {

            for (int i = -MEConfig.gridRectHeight / 2; i <= MEConfig.gridRectHeight / 2; i++) {

                for (int j = -(MEConfig.gridRectWidth) / 2 + 1; j <= MEConfig.gridRectWidth / 2 + (Math.Abs(i) % 2 - 1); j++) {

                    int q = j - (i + Math.Abs(i) % 2) / 2;
                    int r = i;

                    Vector2 hexCoords = new Vector2(q, r);
                    Vector2 worldCoords = HexVectorUtil.worldPositionOfHexCoord(hexCoords);
                    GameObject hex = (GameObject)Instantiate(meHex, new Vector3(worldCoords.x, worldCoords.y, 1), Quaternion.identity);
                    hex.transform.SetParent(transform);

                    HexGrid.Add(hexCoords, new MEHexEntry(hex, hexCoords));
                }
            }
        }

        private void LoadGrid() {

            string[] hexDescriptions = System.IO.File.ReadAllLines(MEConfig.mapLocation + MEConfig.mapName + MEConfig.mapFileType);

            foreach (string description in hexDescriptions) {
                //Parse file input
                string desc;
                string q = description.Substring(0, description.IndexOf(','));
                desc = description.Substring(description.IndexOf(',') + 1, description.Length - description.IndexOf(',') - 1);
                string r = desc.Substring(0, desc.IndexOf(','));
                desc = desc.Substring(desc.IndexOf(',') + 1, desc.Length - desc.IndexOf(',') - 1);
                string t = desc;

                int qVal;
                int rVal;
                if (int.TryParse(q, out qVal) && int.TryParse(r, out rVal)) {
                    try {
                        Deft.Terrain terrain = (Deft.Terrain)Enum.Parse(typeof(Deft.Terrain), t);

                        //Instantiate in-game hexes
                        Vector2 hexCoords = new Vector2(qVal, rVal);
                        Vector2 worldCoords = HexVectorUtil.worldPositionOfHexCoord(hexCoords);
                        GameObject hex = (GameObject)Instantiate(meHex, new Vector3(worldCoords.x, worldCoords.y, 1), Quaternion.identity);
                        hex.transform.SetParent(transform);

                        HexGrid.Add(hexCoords, new MEHexEntry(hex, hexCoords, terrain));

                    } catch { }
                }
            }
        }

        private void LoadUnitPositions() {

            string[] unitDescriptions = System.IO.File.ReadAllLines(MEConfig.mapLocation + MEConfig.mapName + MEConfig.unitMapExtension + MEConfig.mapFileType);

            foreach (string description in unitDescriptions) {

                try {
                    //Parse file input
                    string desc;
                    string q = description.Substring(0, description.IndexOf(','));
                    desc = description.Substring(description.IndexOf(',') + 1, description.Length - description.IndexOf(',') - 1);
                    string r = desc.Substring(0, desc.IndexOf(','));
                    desc = desc.Substring(desc.IndexOf(',') + 1, desc.Length - desc.IndexOf(',') - 1);
                    string t = desc;

                    int qVal;
                    int rVal;
                    qVal = int.Parse(q);
                    rVal = int.Parse(r);

                    int player = int.Parse(t);

                    Vector2 hexCoords = new Vector2(qVal, rVal);
                    HexGrid[hexCoords].Occupant = ((GameObject)Instantiate(unit, HexVectorUtil.worldPositionOfHexCoord(hexCoords), Quaternion.identity)).GetComponent<MEUnitManager>();
                    HexGrid[hexCoords].Occupant.Player = player;
                    HexGrid[hexCoords].Occupant.myHex = HexGrid[hexCoords];

                } catch { }
            }
        }


        //Cycle terrain values
        public void TerrainCycle(MEHexEntry hex) {

            int i = (int)hex.Terrain;
            i++;
            if (i > MEConfig.maxTerrainValue)
                i = 0;

            hex.Terrain = (Deft.Terrain)i;
        }

        public void UnitCycle(MEHexEntry hex) {
            if (hex.Occupant == null) {
                hex.Occupant = Instantiate(unit, HexVectorUtil.worldPositionOfHexCoord(hex.BoardPos), Quaternion.identity).GetComponent<MEUnitManager>();
                hex.Occupant.Player = 1;
                hex.Occupant.myHex = hex;
            }
            else if (hex.Occupant.Player == 1) {
                hex.Occupant.Player = 2;
            }
            else if (hex.Occupant.Player == 2) {
                Destroy(hex.Occupant.gameObject);
                hex.Occupant = null;
            }
        }

        public void SetExtensionTerrain(MEHexEntry hex) {
            extensionTerrainType = hex.Terrain;
        }

        public void TerrainExtend(MEHexEntry hex) {
            hex.Terrain = extensionTerrainType;
        }

        public void DeleteHex(MEHexEntry hex) {
            Destroy(hex.HexManager.gameObject);
            HexGrid.Remove(hex.BoardPos);
        }

        public void OutputMapToFile() {

            if (System.IO.File.Exists(MEConfig.mapLocation + MEConfig.mapName + MEConfig.mapFileType)) {
                System.IO.File.Delete(MEConfig.mapLocation + MEConfig.mapName + MEConfig.mapFileType);
                Debug.Log("Deleted existing map file");
            }
            if (System.IO.File.Exists(MEConfig.mapLocation + MEConfig.mapName + MEConfig.unitMapExtension + MEConfig.mapFileType)) {
                System.IO.File.Delete(MEConfig.mapLocation + MEConfig.mapName + MEConfig.unitMapExtension + MEConfig.mapFileType);
                Debug.Log("Deleted existing unit file");
            }

            List<string> hexOutputs = new List<string>();
            List<string> unitPositions = new List<string>();

            foreach (MEHexEntry entry in HexGrid.Values) {
                hexOutputs.Add(entry.BoardPos.x.ToString() + ","
                    + entry.BoardPos.y.ToString() + ","
                    + entry.Terrain.ToString());
                if (entry.Occupant != null) {
                    unitPositions.Add(entry.BoardPos.x.ToString() + ","
                    + entry.BoardPos.y.ToString() + ","
                    + entry.Occupant.Player.ToString());
                }
            }

            System.IO.File.WriteAllLines(MEConfig.mapLocation + MEConfig.mapName + MEConfig.mapFileType, hexOutputs.ToArray());
            System.IO.File.WriteAllLines(MEConfig.mapLocation + MEConfig.mapName + MEConfig.unitMapExtension + MEConfig.mapFileType, unitPositions.ToArray());

            Debug.Log("Saved");
        }
    }
}