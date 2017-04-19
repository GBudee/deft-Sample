using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Deft;
using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WeaponSelect {

    public class WSController : MonoBehaviour {

        [SerializeField]
        GameObject unitPrefab;
        [SerializeField]
        GameObject hexPrefab;
        [SerializeField]
        Transform hexContainer;

        WSUIHandler uiHandler;
        [SerializeField]
        UnityEngine.UI.Button playerTurnDisplay;
        [SerializeField]
        UnityEngine.UI.Image popupPanel;

        //Option ghost tooltip information
        float hoverStartTime;
        bool amHovering;
        UnitOrWeapon hoveredOptionType;
        UnitBaseType hoveredBaseType;
        WeaponType hoveredWeaponType;
        [SerializeField]
        GameObject lightTooltip;
        [SerializeField]
        GameObject mediumTooltip;
        [SerializeField]
        GameObject heavyTooltip;
        [SerializeField]
        GameObject spearTooltip;
        [SerializeField]
        GameObject swordTooltip;
        [SerializeField]
        GameObject flailTooltip;
        [SerializeField]
        GameObject quarterstaffTooltip;
        [SerializeField]
        GameObject longbowTooltip;
        [SerializeField]
        GameObject daggerTooltip;
        [SerializeField]
        GameObject shieldTooltip;

        private IDictionary<Vector2, WSHexManager> _hexGrid;
        public IDictionary<Vector2, WSHexManager> HexGrid {
            get { return _hexGrid; }
        }

        WSUnitManager _hoveredUnit;
        int _selectedUnitIter;

        int activePlayer;
        IDictionary<int, PlayerType> playerDirectory;
        int draftPhase;
        private IDictionary<int, List<WSUnitManager>> _units;
        public IDictionary<int, List<WSUnitManager>> Units {
            get { return _units; }
        }


        private Deft.Netcode.MatchController matchController;
        private List<int> netPlayersInVault;

        void Start() {
            
            //Netcode and playerdirectory
            Vaults.wsUnitList = new List<WSUnitDescriptor>();
            netPlayersInVault = new List<int>();
            
            GameObject matchControllerObject = GameObject.Find("MatchController");
            if (matchControllerObject != null) {
                matchController = matchControllerObject.GetComponent<Deft.Netcode.MatchController>();
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
                playerDirectory = new Dictionary<int, PlayerType>();
                playerDirectory.Add(1, PlayerType.Local);
                playerDirectory.Add(2, PlayerType.AI);
            }

            //Variable instantiation
            uiHandler = GetComponent<WSUIHandler>();
            _hexGrid = new Dictionary<Vector2, WSHexManager>();
            _units = new Dictionary<int, List<WSUnitManager>>();

            //Load inert hexgrid for visual aid to players
            LoadGrid();
            uiHandler.Initialize();

            //Choose the player whose turn it is
            if (matchController != null) {
                activePlayer = matchController.whoAmI;
                if (activePlayer == 2) { // Have the turn button show correctly in network mode

                    UnityEngine.UI.ColorBlock colorBlock = UnityEngine.UI.ColorBlock.defaultColorBlock;
                    colorBlock.normalColor = Config.Palette.PlayerColor(2);
                    playerTurnDisplay.colors = colorBlock;
                    playerTurnDisplay.transform.FindChild("PlayerTurnText").GetComponent<UnityEngine.UI.Text>().text = "Player 2 Turn";
                }
            }
            else {
                activePlayer = 1;
            }
            draftPhase = 1;

            //Load units to have weapons and base types modified by players
            _units[1] = new List<WSUnitManager>();
            _units[2] = new List<WSUnitManager>();
            LoadUnitPositions();
            EstablishDraftVisibility(activePlayer);
            uiHandler.CenterCameraOnUnits(activePlayer);

            SelectedUnitIter = 0;
        }

        #region Map Loading Region
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
                    Deft.Terrain terrain = (Deft.Terrain)Enum.Parse(typeof(Deft.Terrain), t);

                    //Instantiate in-game hexes
                    Vector2 hexCoords = new Vector2(qVal, rVal);
                    Vector2 worldCoords = HexVectorUtil.worldPositionOfHexCoord(hexCoords);
                    GameObject hex = Instantiate(hexPrefab, new Vector3(worldCoords.x, worldCoords.y, 1), Quaternion.identity);
                    hex.transform.SetParent(hexContainer);
                    WSHexManager hexManager = hex.GetComponent<WSHexManager>();
                    hexManager.TerrainSprite = terrain;

                    _hexGrid.Add(hexCoords, hexManager);

                } catch { }
            }
        }

        private void LoadUnitPositions() {

            string mapName;
            if (Vaults.mapName != null) {
                mapName = Vaults.mapName;
            }
            else {
                mapName = Config.defaultMapName;
            }

            TextAsset unitLoader = (TextAsset)Resources.Load(Config.mapLocation + mapName + MapEditor.MEConfig.unitMapExtension, typeof(TextAsset));
            string[] unitDescriptions = unitLoader.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

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

                    int p = int.Parse(t);

                    Vector2 hexCoords = new Vector2(qVal, rVal);
                    GameObject unitObject = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity);
                    WSUnitManager unit = unitObject.GetComponent<WSUnitManager>();
                    unit.VisiblePosition = hexCoords;
                    unit.PlayerOwner = p;

                    _units[p].Add(unit);

                } catch { }
            }

            foreach (int p in _units.Keys.ToList()) {
                _units[p] = _units[p].OrderBy(unit => unit.VisiblePosition.x)
                                     .ThenBy(unit => unit.VisiblePosition.y)
                                     .ToList();
            }
        }

        private void EstablishDraftVisibility(int player) {

            foreach (int p in _units.Keys.ToList()) {
                foreach (WSUnitManager unit in _units[p]) {
                    if (p == player || (draftPhase == 2 && unit.WeaponType != WeaponType.None)) {
                        unit.Active = true;
                    }
                    else {
                        unit.Active = false;
                    }
                }
            }
        }
        #endregion

        private int SelectedUnitIter {
            get { return _selectedUnitIter; }
            set {
                if (_units[activePlayer].Count > _selectedUnitIter) {
                    _units[activePlayer][_selectedUnitIter].LitBorder = false;
                }

                _selectedUnitIter = value;

                if (_selectedUnitIter >= _units[activePlayer].Count) {
                    _selectedUnitIter = 0;
                }

                _units[activePlayer][_selectedUnitIter].LitBorder = true;
            }
        }

        void Update() {
            if (amHovering && Time.time - hoverStartTime > .5f) {
                if (hoveredOptionType == UnitOrWeapon.Unit) {
                    if (hoveredBaseType == UnitBaseType.Swift) {
                        lightTooltip.SetActive(true);
                    }
                    if (hoveredBaseType == UnitBaseType.Simple) {
                        mediumTooltip.SetActive(true);
                    }
                    if (hoveredBaseType == UnitBaseType.Sturdy) {
                        heavyTooltip.SetActive(true);
                    }
                }
                if (hoveredOptionType == UnitOrWeapon.Weapon) {
                    if (hoveredWeaponType == WeaponType.Spear) {
                        spearTooltip.SetActive(true);
                    }
                    if (hoveredWeaponType == WeaponType.Sword) {
                        swordTooltip.SetActive(true);
                    }
                    if (hoveredWeaponType == WeaponType.Flail) {
                        flailTooltip.SetActive(true);
                    }
                    if (hoveredWeaponType == WeaponType.Quarterstaff) {
                        quarterstaffTooltip.SetActive(true);
                    }
                    if (hoveredWeaponType == WeaponType.Longbow) {
                        longbowTooltip.SetActive(true);
                    }
                    if (hoveredWeaponType == WeaponType.Dagger) {
                        daggerTooltip.SetActive(true);
                    }
                    if (hoveredWeaponType == WeaponType.Shield) {
                        shieldTooltip.SetActive(true);
                    }
                }
            }
        }

        public void MenuOptionClicked(UnitOrWeapon optionType, UnitBaseType unitType, WeaponType weapon) {

            MenuOptionEndHover();

            if (optionType == UnitOrWeapon.Unit) {

                _units[activePlayer][SelectedUnitIter].UnitType = unitType;
                SelectedUnitIter++;
            }
            if (optionType == UnitOrWeapon.Weapon) {

                _units[activePlayer][SelectedUnitIter].WeaponType = weapon;

                bool foundEmpty = false;
                /*
                for (int i = 0; i < _units[turn].Count; i++) {
                    if (_units[turn][i].WeaponType == WeaponType.None) {
                        SelectedUnitIter = i;
                        foundEmpty = true;
                        break;
                    }
                }*/
                if (!foundEmpty) {
                    SelectedUnitIter++;
                }
            }
        }

        public void MenuOptionHover(UnitOrWeapon optiontype, UnitBaseType unitType, WeaponType weaponType) {
            hoverStartTime = Time.time;
            amHovering = true;
            hoveredOptionType = optiontype;
            hoveredBaseType = unitType;
            hoveredWeaponType = weaponType;
        }

        public void MenuOptionEndHover() {
            amHovering = false;
            lightTooltip.SetActive(false);
            mediumTooltip.SetActive(false);
            heavyTooltip.SetActive(false);
            spearTooltip.SetActive(false);
            swordTooltip.SetActive(false);
            flailTooltip.SetActive(false);
            quarterstaffTooltip.SetActive(false);
            longbowTooltip.SetActive(false);
            daggerTooltip.SetActive(false);
            shieldTooltip.SetActive(false);
        }

        public void OptionGhostReleased(UnitOrWeapon optionType, UnitBaseType unitType, WeaponType weapon) {
            if (_hoveredUnit != null) {
                if (_units[activePlayer][SelectedUnitIter] == _hoveredUnit) {
                    SelectedUnitIter++;
                }

                if (optionType == UnitOrWeapon.Unit) {
                    _hoveredUnit.UnitType = unitType;
                }
                else if (optionType == UnitOrWeapon.Weapon) {
                    _hoveredUnit.WeaponType = weapon;
                }
            }
        }

        public void UnitGhostReleased(WSUnitManager unit) {
            if (_hoveredUnit == null) {
                unit.EnableUnitVisual();
            }
            else {
                UnitBaseType tempUnit = unit.UnitType;
                WeaponType tempWeapon = unit.WeaponType;
                unit.UnitType = _hoveredUnit.UnitType;
                unit.WeaponType = _hoveredUnit.WeaponType;
                _hoveredUnit.UnitType = tempUnit;
                _hoveredUnit.WeaponType = tempWeapon;
                unit.EnableUnitVisual();
            }
        }

        public void SelectUnit(WSUnitManager unit) {
            SelectedUnitIter = Units[activePlayer].IndexOf(unit);
        }

        public void UnitHover(WSUnitManager unit) {
            _hoveredUnit = unit;
        }

        public void UnitEndHover(WSUnitManager unit) {
            if (_hoveredUnit == unit) {
                _hoveredUnit = null;
            }
        }

        #region Commented out draft code
        /*
        // Should be called from unit weapon assignment as well as turn end__
        private bool IsDraftLimitReached(WSUnitManager unitReceivingWeapon) { // Null works for no new unit receiving a weapon
            bool isUnitReceivingIncluded = false;

            int weaponsTally = 0;
            foreach (WSUnitManager unit in _units[activePlayer]) {
                if (unit.WeaponType != WeaponType.None) {
                    weaponsTally++;
                }
                if (unit == unitReceivingWeapon) {
                    isUnitReceivingIncluded = true;
                }
            }

            Debug.Log(draftPhase);

            if (isUnitReceivingIncluded) {
                return true;
            }
            if ((draftPhase == 1 && weaponsTally == 3) || (draftPhase == 2 && weaponsTally == _units[activePlayer].Count)) {
                return true;
            }
            return false;
        }*/
        #endregion

        public void EndTurn() {
            if (!uiHandler.inputEnabled) {
                return;
            }

            if (matchController != null) {
                EndWeaponSelection();//Use netcode version of turnend
                return;
            }

            _units[activePlayer][_selectedUnitIter].LitBorder = false; // Maybe there's a better way to organize this?

            switch (activePlayer) {
                case 1:
                    activePlayer = 2;
                    break;
                case 2:
                    EndWeaponSelection();
                    return;
            }
            UnityEngine.UI.ColorBlock colorBlock = UnityEngine.UI.ColorBlock.defaultColorBlock;
            colorBlock.normalColor = Config.Palette.PlayerColor(activePlayer);
            playerTurnDisplay.colors = colorBlock;
            playerTurnDisplay.transform.FindChild("PlayerTurnText").GetComponent<UnityEngine.UI.Text>().text = "Player " + activePlayer + " Turn";


            EstablishDraftVisibility(activePlayer);
            uiHandler.CenterCameraOnUnits(activePlayer);
            SelectedUnitIter = 0;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);//Prevents button from remaining highlighted after use

            if (playerDirectory[activePlayer] == PlayerType.AI) {
                EndTurn();
            }
        }

        private void EndWeaponSelection() {

            List<WSUnitDescriptor> myUnits = new List<WSUnitDescriptor>();

            foreach (int player in playerDirectory.Keys) {
                if (playerDirectory[player] == PlayerType.Local) {
                    foreach (WSUnitManager unit in _units[player]) {
                        myUnits.Add(new WSUnitDescriptor(unit.VisiblePosition, unit.UnitType, unit.WeaponType, player));
                    }
                }
                if (playerDirectory[player] == PlayerType.AI) {
                    List<Vector2> unitPositions = new List<Vector2>();
                    foreach (WSUnitManager unit in _units[player]) {
                        unitPositions.Add(unit.VisiblePosition);
                    }
                    myUnits = myUnits.Concat(DefaultAI.DefaultUnitPicker(unitPositions, player)).ToList();
                }
            }
            Vaults.wsUnitList = Vaults.wsUnitList.Concat(myUnits).ToList();

            if (matchController != null) {
                NetSendUnits(myUnits);
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Skirmish");
        }

        #region Netcode
        private void NetSendUnits(List<WSUnitDescriptor> myUnits) {

            switch (activePlayer) { // Show the popup panel in fun colors to say you're waiting!
                case 1:
                    popupPanel.color = new Color(Config.Palette.PlayerColor(2).r, Config.Palette.PlayerColor(2).g, Config.Palette.PlayerColor(2).b, 1);
                    break;
                case 2:
                    popupPanel.color = new Color(Config.Palette.PlayerColor(1).r, Config.Palette.PlayerColor(1).g, Config.Palette.PlayerColor(1).b, 1);
                    break;
            }
            popupPanel.transform.FindChild("PopupText").GetComponent<UnityEngine.UI.Text>().text = "Waiting for Opponent";
            popupPanel.gameObject.SetActive(true);
            GetComponent<WSUIHandler>().inputEnabled = false;

            //Now send the units and check to see if everyone's submitted
            matchController.netRepPlayers[matchController.whoAmI].CmdSendUnits(matchController.whoAmI, SerializeUnits(myUnits));

            NetAddPlayerToVault(matchController.whoAmI);
        }

        public void NetReceiveUnits(int whoseUnits, byte[] serializedUnits) {

            Vaults.wsUnitList = Vaults.wsUnitList.Concat(DeserializeUnits(serializedUnits)).ToList();

            NetAddPlayerToVault(whoseUnits);
        }

        private void NetAddPlayerToVault(int player) {
            netPlayersInVault.Add(player);

            if (netPlayersInVault.Count == playerDirectory.Keys.Count) {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Skirmish");
            }
        }

        private byte[] SerializeUnits(List<WSUnitDescriptor> unitsAsList) {

            List<SZWSUnitDescriptor> szUnits = new List<SZWSUnitDescriptor>();
            foreach (WSUnitDescriptor unit in unitsAsList) {
                szUnits.Add(new SZWSUnitDescriptor(unit));
            }

            MemoryStream stream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, szUnits);
            return stream.GetBuffer();
        }

        private List<WSUnitDescriptor> DeserializeUnits(byte[] bytes) {

            MemoryStream stream = new MemoryStream(bytes);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            List<SZWSUnitDescriptor> szUnits = (List<SZWSUnitDescriptor>)binaryFormatter.Deserialize(stream);

            List<WSUnitDescriptor> unitsAsList = new List<WSUnitDescriptor>();
            foreach (SZWSUnitDescriptor szUnit in szUnits) {
                unitsAsList.Add(new WSUnitDescriptor(szUnit));
            }
            return unitsAsList;
        }
        #endregion

        public void Quit() {
            //Clean up netcode
            foreach (GameObject playerNetworkRep in GameObject.FindGameObjectsWithTag("Player")) {
                Destroy(playerNetworkRep);
            }
            Destroy(GameObject.Find("NetworkManager"));
            Destroy(GameObject.Find("MatchController"));

            //Return to main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }


        public class WSUnitDescriptor {
            public readonly Vector2 position;
            public readonly UnitBaseType unitType;
            public readonly WeaponType weaponType;
            public readonly int player;

            public WSUnitDescriptor(Vector2 position, UnitBaseType unitType, WeaponType weaponType, int player) {
                this.position = position;
                this.unitType = unitType;
                this.weaponType = weaponType;
                this.player = player;
            }

            public WSUnitDescriptor(SZWSUnitDescriptor toCopy) {
                position = new Vector2(toCopy.positionX, toCopy.positionY);
                unitType = toCopy.unitType;
                weaponType = toCopy.weaponType;
                player = toCopy.player;
            }
        }
        [Serializable]
        public class SZWSUnitDescriptor { // Serializable version of WSUnitDescriptor for sending over the internet
            public readonly float positionX;
            public readonly float positionY;
            public readonly UnitBaseType unitType;
            public readonly WeaponType weaponType;
            public readonly int player;

            public SZWSUnitDescriptor(WSUnitDescriptor toCopy) {
                positionX = toCopy.position.x;
                positionY = toCopy.position.y;
                unitType = toCopy.unitType;
                weaponType = toCopy.weaponType;
                player = toCopy.player;
            }
        }
    }
}