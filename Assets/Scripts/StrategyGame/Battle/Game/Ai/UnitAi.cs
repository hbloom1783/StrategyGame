using Athanor.Collections;
using GridLib.Hex;
using StrategyGame.Battle.Game.Abilities;
using StrategyGame.Battle.Map;
using StrategyGame.Game;
using System.Linq;
using UnityEngine;

namespace StrategyGame.Battle.Game.Ai
{
    public class UnitAi : MonoBehaviour
    {
        protected MapUnit unit { get { return GetComponentInParent<MapUnit>(); } }

        protected GameController game { get { return GameController.instance; } }
        protected MapController map { get { return MapController.instance; } }

        protected void WasteAp(string msg)
        {
            Debug.Log(msg);
            unit.ap = 0;
        }

        public virtual IUnitAbility ChooseAbilityAndTargets()
        {
            IUnitAbility ability = unit.abilities
                .FirstOrDefault(UnitAbilityExt.CanUse);

            if (ability == default(IUnitAbility))
            {
                WasteAp(unit.name + " has no viable abilities; wasting " + unit.ap + " AP.");
            }
            else
            {
                while (ability.CanTarget() && !ability.HasMaxTargets())
                {
                    HexCoords target = ability.GetRange()
                        .ToList()
                        .RandomPick();

                    ability.SelectTarget(target);
                }
            }
            
            return ability;
        }
    }
}
