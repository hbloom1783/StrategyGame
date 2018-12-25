using GridLib.Hex;
using System.Collections.Generic;
using System;

namespace StrategyGame.Battle.Game.Abilities
{
    public abstract class ModularRange : ModularAttribute
    {
        public abstract IEnumerable<HexCoords> rangeArea { get; }

        internal int Count()
        {
            throw new NotImplementedException();
        }
    }
}
