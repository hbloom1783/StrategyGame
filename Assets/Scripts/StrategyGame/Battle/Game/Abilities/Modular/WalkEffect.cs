using Athanor.Tweening;
using GridLib.Hex;
using System.Collections;
using System.Collections.Generic;

namespace StrategyGame.Battle.Game.Abilities
{
    public sealed class WalkEffect : ModularEffect
    {
        public float stepTime = 0.25f;

        public override IEnumerator Proc(IEnumerable<HexCoords> targets)
        {
            // Play sound effect?

            // Animate walking
            foreach(HexCoords newLoc in targets)
            {
                yield return unit.transform.LinearTween(map.GridToWorld(newLoc), stepTime);
                map.PlaceUnit(unit, newLoc);
            }
        }
    }
}
