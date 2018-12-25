using GridLib.Hex;
using System.Collections;
using System.Collections.Generic;

namespace StrategyGame.Battle.Game.Abilities
{
    public abstract class ModularEffect : ModularAttribute
    {
        public abstract IEnumerator Proc(IEnumerable<HexCoords> targets);
    }
}
