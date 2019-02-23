using System.Collections;
using System.Collections.Generic;
using GridLib.Hex;
using Athanor.Tweening;
using StrategyGame.Battle.UI;

namespace StrategyGame.Battle.Game.Abilities
{
    class JumpEffect : ModularEffect
    {
        public float airTime = 1.00f;

        private SmoothCam smoothCam { get { return UI.BattleUi.instance.smoothCam; } }

        public override IEnumerator Proc(IEnumerable<HexCoords> targets)
        {
            smoothCam.PushLock(unit.transform);

            foreach (HexCoords newLoc in targets)
            {
                yield return unit.transform.ParabolicTween(
                    map.MapCellAt(newLoc).unitFooting.position,
                    newLoc.DistanceTo(unit.loc) / 2.0f,
                    airTime);

                map.PlaceUnit(unit, newLoc);
            }

            smoothCam.PopLock();
        }
    }
}
