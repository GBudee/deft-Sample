using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {

    public enum StatOrder { MoveSpeed, HP, Spear, Sword, Axe, Quarterstaff, Flail, Dagger, Longbow, Crossbow, Shield, Normal, Road, Forest, Hills, Pit, Wall, Goal }

    public enum UnitBaseType { Simple, Swift, Sturdy, Cavalry, Monk, Nightwing, Ghost, Flameshade, Telekine }
    public enum WeaponType { None, Spear, Sword, Axe, Quarterstaff, Flail, Dagger, Longbow, Crossbow, Shield } // This should be in order for which weapon should attack first in a tie
    public enum Terrain { Normal, Road, Forest, Hills, Pit, Wall, Goal }

    static class UnitBaseStats {
        private static bool initialized = false;
        private static int[,] _stats;
        private static int[,] Stats {
            get {
                if (!initialized) {
                    initialized = true;
                    Initialize();
                }
                return _stats;
            }
        }

        public static int MoveSpeed(UnitBaseType unitType) {
            return Stats[(int)unitType, (int)StatOrder.MoveSpeed];
        }

        public static int HP(UnitBaseType unitType) {
            return Stats[(int)unitType, (int)StatOrder.HP];
        }

        public static int Damage(UnitBaseType unitType, WeaponType weapon) {
            return Stats[(int)unitType, (int)weapon - 1 + (int)StatOrder.Spear];
        }

        public static int TerrainCost(UnitBaseType unitType, Terrain terrain) {
            return Stats[(int)unitType, (int)terrain + (int)StatOrder.Normal];
        }

        static void Initialize() {
            _stats = new int[Enum.GetNames(typeof(UnitBaseType)).Length,Enum.GetNames(typeof(StatOrder)).Length];

            TextAsset unitStatText = (TextAsset)Resources.Load("Stats/UnitStats", typeof(TextAsset));

            string[] unitStatStrings = unitStatText.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < Enum.GetNames(typeof(UnitBaseType)).Length; i++) {

                string[] splits = unitStatStrings[i + 1].Split(',');

                for (int j = 0; j < Enum.GetNames(typeof(StatOrder)).Length; j++) {
                    _stats[i, j] = int.Parse(splits[j + 1]);
                }
            }
        }
    }
}
