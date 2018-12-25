using System.Collections.Generic;
using GridLib.Hex;
using GridLib.Pathing;

namespace StrategyGame.Battle.Game.Abilities
{
    public sealed class AStarAoE : ModularAoE
    {
        public override IEnumerable<HexCoords> GetAoE(HexCoords loc)
        {
            return unit.AStar(loc);
        }
    }
}
