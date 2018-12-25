using GridLib.Hex;
using System.Collections.Generic;

namespace StrategyGame.Battle.Game.Abilities
{
    public abstract class ModularAoE : ModularAttribute
    {
        public abstract IEnumerable<HexCoords> GetAoE(HexCoords loc);
    }
}
