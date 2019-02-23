using Athanor.Tweening;
using GridLib.Hex;
using StrategyGame.Battle.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StrategyGame.Battle.Game.Abilities
{
    public sealed class WalkEffect : ModularEffect
    {
        public float stepTime = 0.25f;

        private SmoothCam smoothCam { get { return UI.BattleUi.instance.smoothCam; } }

        public override IEnumerator Proc(IEnumerable<HexCoords> targets)
        {
            smoothCam.PushLock(unit.transform);

            // Play sound effect?

            // Animate walking
            foreach (HexCoords newLoc in targets)
            {
                yield return unit.transform.LinearTween(
                    map.MapCellAt(newLoc).unitFooting.position,
                    stepTime);
            }

            map.PlaceUnit(unit, targets.Last());

            smoothCam.PopLock();
        }
    }
}
