using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Deft {
    public enum PlayerType { Local, Online, AI }

    public class GameManager : MonoBehaviour {

        private ScenarioLoader scenarioLoader;
        private UIHandler uiHandler;
        private SelectionManager selectionManager;
        private UnitFactory unitFactory;

        private Netcode.MatchController matchController;
        private IComputerPlayer computerPlayer;

        public IDictionary<int, PlayerType> playerDirectory;
        public int ActivePlayer { get; set; } // The player whose turn it currently is

        void Start() {


            // Cache major scripts to be initialized
            scenarioLoader = GetComponent<ScenarioLoader>();
            uiHandler = GetComponent<UIHandler>();
            selectionManager = GetComponent<SelectionManager>();
            unitFactory = GetComponent<UnitFactory>();

            // Load map, then camera
            scenarioLoader.Initialize();
            uiHandler.Initialize();


            // Establish player types, network information
            GameObject matchControllerObject = GameObject.Find("MatchController");
            if (matchControllerObject != null) {
                matchController = matchControllerObject.GetComponent<Netcode.MatchController>();
            }
            if (matchController != null) {
                playerDirectory = new Dictionary<int, PlayerType>();
                foreach (int player in matchController.netRepPlayers.Keys) {
                    if (matchController.whoAmI == player) {
                        playerDirectory.Add(player, PlayerType.Local);
                    }
                    else {
                        playerDirectory.Add(player, PlayerType.Online);
                    }
                }
            }
            else if (Vaults.playerDirectory != null) {
                playerDirectory = Vaults.playerDirectory;
            }
            else {
                Debug.Log("Using default playerDirectory");
                playerDirectory = new Dictionary<int, PlayerType>();
                playerDirectory.Add(1, PlayerType.Local);
                playerDirectory.Add(2, PlayerType.AI); // FOR TESTING__, these defaults shouldn't be reached
            }

            Tutorial.TutorialManager tutorialManager = GetComponent<Tutorial.TutorialManager>();
            if (tutorialManager != null) {
                computerPlayer = new Tutorial.TutorialAI(tutorialManager, selectionManager, scenarioLoader);
            }
            else if (playerDirectory.Values.Contains(PlayerType.AI)) {
                computerPlayer = new DefaultAI(selectionManager, scenarioLoader);
            }
            ActivePlayer = 1;

            // Build & load units onto the map
            if (Vaults.wsUnitList != null) {
                foreach (WeaponSelect.WSController.WSUnitDescriptor unit in Vaults.wsUnitList) {
                    unitFactory.Create(unit.player, unit.unitType, unit.weaponType, scenarioLoader.HexGrid[unit.position]);
                }
            }
            else { // FOR TESTING
                /*
                unitFactory.Create(1, UnitBaseType.Simple, WeaponType.Longbow, GetComponent<ScenarioLoader>().HexGrid[new Vector2(-5, 5)]);
                unitFactory.Create(1, UnitBaseType.Simple, WeaponType.Shield, GetComponent<ScenarioLoader>().HexGrid[new Vector2(-3, 5)]);
                unitFactory.Create(2, UnitBaseType.Simple, WeaponType.Spear, GetComponent<ScenarioLoader>().HexGrid[new Vector2(0, -5)]);
                unitFactory.Create(2, UnitBaseType.Simple, WeaponType.Flail, GetComponent<ScenarioLoader>().HexGrid[new Vector2(-3, -5)]);*/
            }
            
            // Initialize the first turn
            selectionManager.Initialize();

            if (tutorialManager != null) {
                tutorialManager.Initialize();
            }
        }

        // The somewhat hacky system used by the tutorial to change which units are in play
        public void ReloadUnits(List<WeaponSelect.WSController.WSUnitDescriptor> newUnits) {

            selectionManager.DestroyAllUnits();

            foreach (WeaponSelect.WSController.WSUnitDescriptor unit in newUnits) {
                unitFactory.Create(unit.player, unit.unitType, unit.weaponType, scenarioLoader.HexGrid[unit.position]);
            }
            selectionManager.UpdateUnitList();
        }

        #region AI
        // Try means its safe to use every turn, and will only run when appropriate
        public void AITryAITurn() {
            if (playerDirectory[ActivePlayer] != PlayerType.AI) {
                return;
            }

            List<Outcome> moves = computerPlayer.GenerateTurn(ActivePlayer); // Here the ai executes its moves as it goes for ease of simulation
            selectionManager.EndTurn();
            selectionManager.ImplementAITurn(moves);
        }
        #endregion

        #region Netcode
        public bool NetEnemyTurn() {

            if (playerDirectory[ActivePlayer] == PlayerType.Online) {
                return true;
            }
            else {
                return false;
            }
        }

        // Try means its safe to use every turn, and will only run when appropriate
        public void NetTrySendMoves(List<SelectionManager.BoardState> moves) {
            if (matchController == null) {
                return;
            }
            if (ActivePlayer != matchController.whoAmI) {
                return;
            }

            List<Outcome> movesAsList = new List<Outcome>();
            foreach (SelectionManager.BoardState boardState in moves) {
                foreach (Outcome outcome in boardState.movesSincePrevState) {
                    movesAsList.Add(outcome);
                }

            }
            matchController.netRepPlayers[matchController.whoAmI].CmdSendMoves(matchController.whoAmI, SerializeMoves(movesAsList));
        }

        public void NetReceiveMoves(byte[] serializedMoves) {
            selectionManager.NetStoreEnemyTurn(DeserializeMoves(serializedMoves));
            selectionManager.EndTurn();
        }

        private byte[] SerializeMoves(List<Outcome> movesAsList) {

            List<SZOutcome> szMoves = new List<SZOutcome>();
            foreach (Outcome outcome in movesAsList) {
                szMoves.Add(new SZOutcome(selectionManager, scenarioLoader, outcome));
            }

            MemoryStream stream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, szMoves);
            return stream.GetBuffer();
        }

        private List<Outcome> DeserializeMoves(byte[] bytes) {

            MemoryStream stream = new MemoryStream(bytes);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            List<SZOutcome> szMoves = (List<SZOutcome>)binaryFormatter.Deserialize(stream);

            List<Outcome> movesAsList = new List<Outcome>();
            foreach (SZOutcome szoutcome in szMoves) {
                movesAsList.Add(new Outcome(selectionManager, scenarioLoader, szoutcome));
            }
            return movesAsList;
        }
        #endregion

        public void QuitGame() {

            // Clean up netcode objects
            foreach (GameObject playerNetworkRep in GameObject.FindGameObjectsWithTag("Player")) {
                Destroy(playerNetworkRep);
            }
            Destroy(GameObject.Find("NetworkManager"));
            Destroy(GameObject.Find("MatchController"));

            // Return to main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }

        #region Palette Editor (commented out)
        // Unity Editor exposed functions for palette editor
        
        public void SetPalette() {
            /*
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = Config.Palette.background;

            if (GetComponent<SelectionManager>().UnitsByPlayer == null) {
                return;
            }
            foreach (int player in playerDirectory.Keys.ToList()) {
                foreach (IUnitController unit in GetComponent<SelectionManager>().UnitsByPlayer[player]) {
                    unit.SpriteManager.PlayerOwner = player;
                    unit.SpriteManager.transform.FindChild("SpriteHolder").FindChild("Base Type").GetComponent<SpriteRenderer>().color = Config.Palette.unitIconFill;
                    unit.SpriteManager.transform.FindChild("SpriteHolder").FindChild("Weapon").GetComponent<SpriteRenderer>().color = Config.Palette.unitIconFill;
                    unit.SpriteManager.SetBorderColor(Config.Palette.border);
                }
            }
            foreach (HexEntry hex in GetComponent<ScenarioLoader>().HexGrid.Values) {
                HexEntry hexEntry = hex;
                hexEntry.Terrain = hexEntry.Terrain;
                if (hexEntry.Terrain == Terrain.Normal)
                    hexEntry.HexManager.BackgroundColor = Config.Palette.normalTerrain;
                if (hexEntry.Terrain == Terrain.Road)
                    hexEntry.HexManager.BackgroundColor = Config.Palette.roadTerrain;
                if (hexEntry.Terrain == Terrain.Forest)
                    hexEntry.HexManager.BackgroundColor = Config.Palette.forestTerrain;
                if (hexEntry.Terrain == Terrain.Hills)
                    hexEntry.HexManager.BackgroundColor = Config.Palette.hillsTerrain;
                if (hexEntry.Terrain == Terrain.Wall)
                    hexEntry.HexManager.BackgroundColor = Config.Palette.wallTerrain;
                if (hexEntry.Terrain == Terrain.Pit)
                    hexEntry.HexManager.BackgroundColor = Config.Palette.wallTerrain;
                if (hexEntry.Terrain == Terrain.Goal)
                    hexEntry.HexManager.BackgroundColor = Config.Palette.goalTerrain;
                hexEntry.HexManager.BorderColor = Config.Palette.border;
            }*/
        }
        
        public void SetHexSpacing(float spacing) {
            /*
            Config.hexSize = spacing;

            if (GetComponent<SelectionManager>().UnitsByPlayer == null) {
                return;
            }
            foreach (int player in playerDirectory.Keys.ToList()) {
                foreach (IUnitController unit in GetComponent<SelectionManager>().UnitsByPlayer[player]) {
                    unit.SpriteManager.VisiblePosition = unit.Position;
                }
            }

            foreach (HexEntry hex in GetComponent<ScenarioLoader>().HexGrid.Values) {
                hex.HexManager.transform.position = HexVectorUtil.worldPositionOfHexCoord(hex.BoardPos);
            }*/
        }

        /*
    public void SetColorField(int fieldIndex) {
        string input;

        switch (fieldIndex) {
            case 1:
                input = GameObject.Find("BackgroundColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 2:
                input = GameObject.Find("PlayerOneColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 3:
                input = GameObject.Find("PlayerTwoColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 4:
                input = GameObject.Find("AttackColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 5:
                input = GameObject.Find("FootstepColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 6:
                input = GameObject.Find("BlackColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 7:
                input = GameObject.Find("NormalColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 8:
                input = GameObject.Find("RoadColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 9:
                input = GameObject.Find("ForestColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 10:
                input = GameObject.Find("HillsColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            case 11:
                input = GameObject.Find("WallColorText").GetComponent<UnityEngine.UI.Text>().text;
                break;
            default:
                input = "";
                break;
        }

        Debug.Log("Input received: " + input);

        string r = input.Substring(0, input.IndexOf(','));
        input = input.Substring(input.IndexOf(',') + 1, input.Length - input.IndexOf(',') - 1);
        string g = input.Substring(0, input.IndexOf(','));
        input = input.Substring(input.IndexOf(',') + 1, input.Length - input.IndexOf(',') - 1);
        string b = input;

        float red, green, blue;

        if (float.TryParse(r, out red) && float.TryParse(g, out green) && float.TryParse(b, out blue)) {

            Debug.Log("Parse succeeded: " + red + " " + green + " " + blue);
            Color c = new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
            switch (fieldIndex) {
                case 1:
                    Config.Palette.background = c;
                    break;
                case 2:
                    Config.Palette.playerOne = c;
                    break;
                case 3:
                    Config.Palette.playerTwo = c;
                    break;
                case 4:
                    Config.Palette.attack = c;
                    break;
                case 5:
                    Config.Palette.footsteps = c;
                    break;
                case 6:
                    Config.Palette.black = c;
                    break;
                case 7:
                    Config.Palette.normalTerrain = c;
                    break;
                case 8:
                    Config.Palette.roadTerrain = c;
                    break;
                case 9:
                    Config.Palette.forestTerrain = c;
                    break;
                case 10:
                    Config.Palette.hillsTerrain = c;
                    break;
                case 11:
                    Config.Palette.wallTerrain = c;
                    break;
            }
        }

        SetPalette(0);//Refresh all colors on objects
    }
    */
        #endregion
    }
}

