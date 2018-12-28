using System.Collections;
using System.Collections.Generic;
using GridLib.Hex;
using Athanor.Tweening;

namespace StrategyGame.Battle.Game.Abilities
{
    class JumpEffect : ModularEffect
    {
        public float airTime = 1.00f;

        public override IEnumerator Proc(IEnumerable<HexCoords> targets)
        {
            foreach (HexCoords target in targets)
            {
                yield return unit.transform.ParabolicTween(
                    map.GridToWorld(target),
                    target.DistanceTo(unit.loc) / 2.0f,
                    airTime);

                map.PlaceUnit(unit, target);
            }
        }
    }
}
