using System.Collections.Generic;
using GridLib.Hex;
using GridLib.Pathing;

namespace StrategyGame.Battle.Game.Abilities
{
    public sealed class DijkstraRange : ModularRange
    {
        public int walkingSpeed = 0;

        public override IEnumerable<HexCoords> rangeArea
        {
            get
            {
                return unit.Dijkstra(walkingSpeed).Keys;
            }
        }
    }
}
