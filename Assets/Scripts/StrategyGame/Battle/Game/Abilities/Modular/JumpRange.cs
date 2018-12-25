using System.Collections.Generic;
using System.Linq;
using GridLib.Hex;

namespace StrategyGame.Battle.Game.Abilities
{
    class JumpRange : ModularRange
    {
        public override IEnumerable<HexCoords> rangeArea
        {
            get
            {
                return unit.loc.CompoundRing(1, (uint)unit.jump)
                    .Where(map.InBounds)
                    .Where(unit.CanEnter);
            }
        }
    }
}
