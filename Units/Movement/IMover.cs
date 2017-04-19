using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Deft {
    public interface IMover {
        List<HexEntry> CalculatePaths();
        
        List<Outcome> MoveOutcomes(HexEntry dest);

        List<Outcome> SimulatePush(IUnitController target, Vector2 pushDirection, int pushAmount);
    }
}
