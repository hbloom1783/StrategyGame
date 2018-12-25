using GridLib.Hex;
using GridLib.Pathing;
using System.Collections.Generic;

namespace StrategyGame.Battle.Game.Abilities
{
    class DijkstraRangeFromUnit : ModularRange
    {
        public override IEnumerable<HexCoords> rangeArea
        {
            get
            {
                return unit.Dijkstra(unit.speed).Keys;
            }
        }
    }
}
