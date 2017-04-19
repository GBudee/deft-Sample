using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deft {
    // The UnitController script will perform the special actions of a unit, receiving a hex from the SelectionManager
    // It may also be responsible for passive unit abilities or possibly unit instantiation procedures
    public interface IUnitController {
        // All the parts of the unit!
        UnitSpriteManager SpriteManager { get; }
        IMover Mover { get; }
        IWeapon Weapon { get; set; }// set is Temp
        OutcomeAnimator OutcomeAnimator { get; }
        UnitBaseType UnitType { get; set; }
        WeaponType WeaponType { get; set; }
        int PlayerOwner { get; set; }

        void Initialize(IMover mover, OutcomeAnimator outcomeAnimator, UnitSpriteManager unitSpriteManager, HexEntry startPosition);

        // Stats
        int DamageOutput { get; }
        int MoveSpeed { get; }

        // State
        HexEntry Position { get; set; }
        List<HexEntry> RecentPath { get; set; }
        int MovesRemaining { get; set; }
        int ZoCRemaining { get; set; }
        int HP { get; set; }
        UnitState UnitCurrentState { get; set; }

        // Sim state        
        HexEntry SimPosition { get; set; }
        List<HexEntry> SimRecentPath { get; set; }
        int SimHealthAfterXDamage(int receiveXDamage);
        int SimHealth();
        void ResetSimulationState();

        // Actual combat state
        void ExecuteAttack();

        void TurnEndUpkeep();
        void TurnStartUpkeep(); // Called at start of turn
        bool PerformAction(HexEntry loc); // true if action successful, false if not
    }
}
