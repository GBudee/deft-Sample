using System.Collections.Generic;

namespace Deft {
    public interface IComputerPlayer {
        List<Outcome> GenerateTurn(int player);
    }
}