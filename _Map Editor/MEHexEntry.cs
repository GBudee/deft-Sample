using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MapEditor {
    public enum Bearing { E, NNE, NNW, W, SSW, SSE }

    public class MEHexEntry {
        public MEHexManager HexManager { get; set; }
        public Vector2 BoardPos { get; set; }


        // Unit, if any, on this hex
        private MEUnitManager _occupant;
        public MEUnitManager Occupant {
            get { return _occupant; }
            set {
                if (_occupant != null && value != null) {
                    throw new System.Exception("Hex at " + BoardPos + " already has an occupant.");
                }
                _occupant = value;
            }
        }

        private Deft.Terrain _terrain;
        public Deft.Terrain Terrain {
            get { return _terrain; }
            set {
                _terrain = value;
                HexManager.TerrainSprite = value;
            }
        }
        
        public MEHexEntry(GameObject hex, Vector2 boardPos, Deft.Terrain terrain = Deft.Terrain.Normal) {
            this.BoardPos = boardPos;

            this.HexManager = hex.GetComponent<MEHexManager>();
            this.HexManager.HexEntry = this;

            Terrain = terrain;

            //this.Occupant = null;
        }
    }
}
