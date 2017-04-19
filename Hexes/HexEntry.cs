using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Deft {
    public class HexEntry {

        public UnityEngine.UI.Text debugText;

        public HexManager HexManager { get; set; }
        public Vector2 BoardPos { get; set; }

        // Basic property of hex
        private Terrain _terrain;
        public Terrain Terrain {
            get { return _terrain; }
            set {
                _terrain = value;
                HexManager.TerrainSprite = value;
            }
        }

        // Unit, if any, on this hex
        private IUnitController _occupant;
        public IUnitController Occupant {
            get { return _occupant; }
            set {
                if (_occupant != null && value != null) {
                    throw new System.Exception("Hex at " + BoardPos + " already has an occupant.");
                }
                _occupant = value;

                /*if (debugText != null) {
                    debugText.text = "[" + (_occupant != null ? "X" : "_") + " " + (_simOccupant != null ? "X" : "_") + "]";
                }*/
            }
        }

        // Unit, if any, on this hex
        private IUnitController _simOccupant;
        public IUnitController SimOccupant {
            get { return _simOccupant; }
            set {
                if (_simOccupant != null && value != null && _simOccupant != value) {
                    
                    Debug.Log("The latter tried to move onto the place the former was occupying");
                    Debug.Log(_simOccupant.SimPosition.BoardPos.ToString());
                    Debug.Log(value.SimPosition.BoardPos.ToString());

                    throw new System.Exception("Hex at " + BoardPos + " already has a simulated occupant.");
                }
                _simOccupant = value;
                
                //debugText.text = "[" + (_occupant != null ? "X" : "_") + " " + (_simOccupant != null ? "X" : "_") + "]";
            }
        }

        private IDictionary<Bearing, HexEntry> _neighbors;
        public IDictionary<Bearing, HexEntry> Neighbors {
            get { return _neighbors; }
        }

        // Stored in number of hexes, player number is key, value is units listed with distance
        private int _aiDistanceToEnemy;
        public int AIDistanceToEnemy {
            get { return _aiDistanceToEnemy; }
            set {
                //debugText.text = value.ToString();
                _aiDistanceToEnemy = value;
            }
        }
        public int aiAdjacentEnemies;
        public int aiDistanceToGoal;

        #region Visual Features
        private bool _selectionHighlighted;
        public bool SelectionHighlighted {
            get { return _selectionHighlighted; }
            set {
                _selectionHighlighted = value;
                if (_selectionHighlighted) {
                    HexManager.SelectionHighlightColor = Config.Palette.hexReachableColor;
                }
                else {
                    HexManager.SelectionHighlightColor = Color.clear;
                }
            }
        }

        private bool _enemyHighlighted;
        public bool EnemyHighlighted {
            get { return _enemyHighlighted; }
            set {
                _enemyHighlighted = value;
                if (_enemyHighlighted) {
                    HexManager.EnemyHighlightColor = Config.Palette.hexReachableColor;
                }
                else {
                    HexManager.EnemyHighlightColor = Color.clear;
                }
            }
        }
        #endregion

        public HexEntry(GameObject hex, Vector2 boardPos, Terrain terrain = Terrain.Normal) {
            this.BoardPos = boardPos;

            this.HexManager = hex.GetComponent<HexManager>();
            this.HexManager.HexEntry = this;

            Terrain = terrain;

            this.Occupant = null;
        }

        // Not in constructor because it needs to be made after all hexes have been created
        public void SetNeighbors(IDictionary<Bearing, HexEntry> neighbors) {
            this._neighbors = new Dictionary<Bearing, HexEntry>();
            foreach (KeyValuePair<Bearing, HexEntry> entry in neighbors) {
                this._neighbors.Add(entry.Key, entry.Value);
            }
        }
    }
}
