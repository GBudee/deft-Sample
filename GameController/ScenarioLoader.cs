using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Deft;
using System;

namespace Deft {
    public class ScenarioLoader : MonoBehaviour {
        [SerializeField]
        GameObject basicHex;
        [SerializeField]
        Transform hexContainer;

        //Text stuff
        [SerializeField]
        GameObject worldCanvas;//To attach text to, thereby displaying hex properties for testing purposes
        [SerializeField]
        UnityEngine.UI.Text turnsRemainingText;
        [SerializeField]
        GameObject textPrefab;//Prefab so we can create text to label hexes

        public IDictionary<Vector2, HexEntry> HexGrid = new Dictionary<Vector2, HexEntry>();

        private List<HexEntry> hexesByUniqueID = new List<HexEntry>();//Used for serialization

        public void Initialize() {
            LoadGrid();
            ConnectNeighbors();
        }

        private void LoadGrid() {

            string mapName;
            if (Vaults.mapName != null) {
                mapName = Vaults.mapName;
            }
            else {
                mapName = Config.defaultMapName;
            }

            TextAsset map = (TextAsset)Resources.Load(Config.mapLocation + mapName, typeof(TextAsset));
            string[] hexDescriptions = map.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string description in hexDescriptions) {

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
                    Terrain terrain = (Terrain)Enum.Parse(typeof(Terrain), t);

                    //Instantiate in-game hexes
                    Vector2 hexCoords = new Vector2(qVal, rVal);
                    Vector2 worldCoords = HexVectorUtil.worldPositionOfHexCoord(hexCoords);
                    GameObject hex = Instantiate(basicHex, new Vector3(worldCoords.x, worldCoords.y, 1), Quaternion.identity);
                    hex.transform.SetParent(hexContainer);

                    HexEntry newHex = new HexEntry(hex, hexCoords, terrain);
                    HexGrid.Add(hexCoords, newHex);
                    hexesByUniqueID.Add(newHex);

                    //Text stuff:
                    GameObject hexText = Instantiate(textPrefab, Vector2.Scale(worldCoords, Config.worldToCanvasCoordScaler), Quaternion.identity);
                    hexText.GetComponent<UnityEngine.UI.Text>().text = hexCoords.ToString();
                    //hexText.GetComponent<UnityEngine.UI.Text>().text = "[_ _]";
                    hexText.transform.SetParent(worldCanvas.transform, false);
                    newHex.debugText = hexText.GetComponent<UnityEngine.UI.Text>();

                    if (terrain == Terrain.Goal) {
                        turnsRemainingText.transform.position = new Vector3(worldCoords.x, worldCoords.y + 0.23f, 0);
                        turnsRemainingText.gameObject.SetActive(true);
                    }

                } catch { }
            }
        }

        public void SetTurnsRemainingText(int turnsRemaining) {
            turnsRemainingText.text = turnsRemaining.ToString();
        }

        public HexEntry GetHexByID(int id) {
            return hexesByUniqueID[id];
        }
        public int GetIDByHex(HexEntry hex) {
            return hexesByUniqueID.IndexOf(hex);
        }
        
        // Tell each HexEntry about the HexEntries that are immediately adjacent
        public void ConnectNeighbors() {
            foreach (KeyValuePair<Vector2, HexEntry> entry in HexGrid) {
                Vector2 loc = entry.Key;
                HexEntry hexEntry = entry.Value;
                IDictionary<Bearing, HexEntry> neighbors = new Dictionary<Bearing, HexEntry>();

                foreach (Bearing b in Enum.GetValues(typeof(Bearing))) {
                    Vector2 neighborLoc = loc + HexVectorUtil.NeighborOffsetFromBearing(b);
                    if (HexGrid.ContainsKey(neighborLoc)) {
                        neighbors.Add(b, HexGrid[neighborLoc]);
                    }
                }

                hexEntry.SetNeighbors(neighbors);
            }
        }
    }
}



