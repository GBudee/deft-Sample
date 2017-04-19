using UnityEngine;
using System.Collections.Generic;
using Deft;


namespace Deft {

    public class DefaultUnitController : IUnitController {

        UnitSpriteManager _unitSpriteManager;
        public UnitSpriteManager SpriteManager {
            get { return _unitSpriteManager; }
        }
        IMover _mover;
        public IMover Mover {
            get { return _mover; }
        }
        IWeapon _weapon;
        public IWeapon Weapon {
            get { return _weapon; }
            set { _weapon = value; }//Temp
        }
        OutcomeAnimator _outcomeAnimator;
        public OutcomeAnimator OutcomeAnimator {
            get { return _outcomeAnimator; }
        }

        //<State variables>
        public HexEntry _pos;
        public List<HexEntry> _recentPath;
        public int _movesRemaining;
        public int _zoCMovesRemaining;
        public int _hp;
        public int _shots;
        //</End state variables>

        //<Sim state>
        private HexEntry _simPos;
        private List<HexEntry> _simRecentPath;
        private int _simHP;
        private int simShots;
        //</End sim state>

        //<Scenario properties>
        private UnitBaseType _unitType;
        private WeaponType _weaponType;
        private int _playerOwner;

        public UnitBaseType UnitType {
            get { return _unitType; }
            set {
                _unitType = value;
                SpriteManager.UnitType = value;
            }
        }
        public WeaponType WeaponType {
            get { return _weaponType; }
            set {
                _weaponType = value;
                SpriteManager.WeaponType = value;
            }
        }
        public int PlayerOwner {
            get { return _playerOwner; }
            set {
                _playerOwner = value;
                SpriteManager.PlayerOwner = value;
            }
        }
        //</End scenario properties>

        //<Stats>
        public int MoveSpeed {
            get { return UnitBaseStats.MoveSpeed(UnitType); }
        }

        public int DamageOutput {
            get {
                if (_simHP > 0) {
                    if (WeaponType == WeaponType.Longbow || WeaponType == WeaponType.Crossbow) {
                        if (simShots > 0) {
                            simShots--;
                            return UnitBaseStats.Damage(UnitType, WeaponType);
                        }
                        else {
                            return 0;
                        }
                    }
                    return UnitBaseStats.Damage(UnitType, WeaponType);
                }
                else {
                    return 0;
                }
            }
        }
        //</Stats>

        public DefaultUnitController(UnitBaseType unitType, WeaponType weaponType, int playerOwner) {
            this._unitType = unitType;
            this._weaponType = weaponType;
            this._playerOwner = playerOwner;
        }
        public void Initialize(IMover mover, OutcomeAnimator outcomeAnimator, UnitSpriteManager unitSpriteManager,
                HexEntry startPosition) {
            this._mover = mover;
            this._outcomeAnimator = outcomeAnimator;
            this._unitSpriteManager = unitSpriteManager;

            RecentPath = new List<HexEntry>();
            Position = startPosition;
            unitSpriteManager.VisiblePosition = Position;

            HP = UnitBaseStats.HP(UnitType);
            SpriteManager.MaxHealthDisplay = HP; // Instantiate the health bar sizing for the unit, maxHP must be first
            SpriteManager.CurrentHealthDisplay = HP;

            TurnEndUpkeep();
            TurnStartUpkeep();

            UnitType = _unitType; // Gives spritemanager unit-type data
            WeaponType = _weaponType;
            PlayerOwner = _playerOwner;
        }

        //<Public state access>
        public HexEntry Position {
            get { return _pos; }
            set {
                if (_pos != null && _pos.Occupant == this) {
                    _pos.Occupant = null;
                }
                _pos = value;
                if (_pos != null) {
                    _pos.Occupant = this;
                }
                SimPosition = _pos;

                RecentPath.Add(_pos);
                RecentPath = RecentPath;
            }
        }
        public List<HexEntry> RecentPath {
            get { return _recentPath; }
            set {
                _recentPath = value;
                SimRecentPath = _recentPath;
            }
        }
        public int MovesRemaining {
            get { return _movesRemaining; }
            set {
                _movesRemaining = value;
                //_unitSpriteManager.VisualMovesRemaining = _movesRemaining;
            }
        }
        public int ZoCRemaining {
            get { return _zoCMovesRemaining; }
            set { _zoCMovesRemaining = value; }
        }
        public int HP {
            get { return _hp; }
            set {
                _hp = value;
                _simHP = _hp;
            }
        }
        private int Shots {
            get { return _shots; }
            set {
                _shots = value;
                simShots = _shots;
            }
        }

        public UnitState UnitCurrentState {
            get { return new UnitState(this, Position, RecentPath, MovesRemaining, ZoCRemaining, HP, Shots); }
            set {
                Position = value.pos;
                RecentPath = value.recentPath;
                MovesRemaining = value.movesRemaining;
                ZoCRemaining = value.zoCMovesRemaining;
                HP = value.hp;
                Shots = value.shots;
            }
        }
        //<\End public state access>

        //<Public sim state access>
        public HexEntry SimPosition {
            get { return _simPos; }
            set {
                if (_simPos != null && _simPos.SimOccupant == this) {
                    _simPos.SimOccupant = null;
                }
                _simPos = value;
                if (_simPos != null) {
                    _simPos.SimOccupant = this;
                }

                SimRecentPath.Add(_simPos);
                SimRecentPath = SimRecentPath;
            }
        }
        public List<HexEntry> SimRecentPath {
            get { return _simRecentPath; }
            set { _simRecentPath = new List<HexEntry>(value); }
        }
        public int SimHealthAfterXDamage(int receiveXDamage) {
            _simHP -= receiveXDamage;
            return _simHP;
        }
        public int SimHealth() {
            return _simHP;
        }
        public void ResetSimulationState() {
            SimPosition = _pos;
            SimRecentPath = RecentPath;
            _simHP = _hp;
            simShots = _shots;
        }
        //<\End public sim state access>

        public void ExecuteAttack() {
            if (WeaponType == WeaponType.Longbow || WeaponType == WeaponType.Crossbow) {
                Shots--;
            }
        }

        //Called at end of turn
        public void TurnEndUpkeep() {
            MovesRemaining = MoveSpeed;
            _unitSpriteManager.VisualMovesRemaining = _movesRemaining;
            _zoCMovesRemaining = 1; // 1 is deft style -- Not sure if 0 is actually wesnoth style
        }

        //Called at start of turn
        public void TurnStartUpkeep() {
            
            // Movement
            _recentPath.Clear();
            _recentPath.Add(_pos);

            if (Position != null) {
                if (Position.Terrain == Terrain.Goal) {
                    if (PlayerOwner == 2) {
                        GameObject.Find("GameController").GetComponent<SelectionManager>().Player2Win();
                    }
                }
            }

            // Combat
            if (WeaponType == WeaponType.Longbow || WeaponType == WeaponType.Crossbow) {
                Shots = 1;
            }
        }

        //Receive action command from selectionmanager/input
        public bool PerformAction(HexEntry loc) {
            return false;
        }
    }

    public class UnitState {
        public HexEntry pos;
        public List<HexEntry> recentPath;
        public int movesRemaining;
        public int zoCMovesRemaining;
        public int hp;
        public int shots;

        private IUnitController _unitController;
        public IUnitController UnitController {
            get { return _unitController; }
        }

        public UnitState(IUnitController unitController, HexEntry pos, List<HexEntry> recentPath, int movesRemaining, int zoCMovesRemaining, int hp, int shots) {
            this._unitController = unitController;
            this.pos = pos;
            this.recentPath = recentPath;
            this.movesRemaining = movesRemaining;
            this.zoCMovesRemaining = zoCMovesRemaining;
            this.hp = hp;
            this.shots = shots;
        }

        /*
        public UnitState(UnitState stateToCopy) {
            this._unitController = stateToCopy.UnitController;
            this.pos = stateToCopy.pos;
            this.recentPath = stateToCopy.recentPath;
            this.movesRemaining = stateToCopy.movesRemaining;
            this.zoCMovesRemaining = stateToCopy.zoCMovesRemaining;
            this.hp = stateToCopy.hp;
            this.shots = stateToCopy.shots;
        }*/
    }
}